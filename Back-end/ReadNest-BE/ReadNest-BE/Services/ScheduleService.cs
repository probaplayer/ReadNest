using Microsoft.EntityFrameworkCore;
using ReadNest_BE.Infrastructure;
using ReadNest_BE.Interfaces.Repositories;
using ReadNest_BE.Interfaces.Services;
using ReadNest_BE.Models;
using ReadNest_Models;
using System.IO;

namespace ReadNest_BE.Services
{
    public class ScheduleService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ScheduleService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await DeleteTempImages();
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromHours(5), stoppingToken);
                    await DeleteTempImages();
                }
            }catch (Exception ex)
            {
                _logger.LogError($"Error in ScheduleService: {ex.Message}");
            }

        }

        private async Task DeleteTempImages()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var imageRepository = scope.ServiceProvider.GetRequiredService<IImageRepository>();
            var jwtRepository = scope.ServiceProvider.GetRequiredService<JwtService>();
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            jwtRepository.IsComputer = true;

            await RetryFailedDeletions(appDbContext, fileService, imageRepository, emailService);

            var listExpiredImages = await appDbContext.Database
                .SqlQueryRaw<ImagePathResult>(@"
                    SELECT i.Id, i.ImageName, i.ImagePath
                    FROM Images i
                    WHERE i.ExpiresAt IS NOT NULL 
                      AND i.ExpiresAt < datetime('now')
                ")
                .ToListAsync();

            if (listExpiredImages.Count == 0)
            {
                _logger.LogInformation("No expired images to delete");
                return;
            }

            List<string> listImageFailedToDelete = new List<string>();
            int successCount = 0;

            foreach (var image in listExpiredImages)
            {
                try
                {
                    var deleted = await fileService.DeleteImageAsync(image.ImagePath!);
                    if (deleted)
                    {
                        var imageExisting = await imageRepository.GetById(image.Id);
                        await imageRepository.Delete(imageExisting);
                        successCount++;
                    }
                    else
                    {
                        var imageExisting = await imageRepository.GetById(image.Id);
                        if (imageExisting != null)
                        {
                            await imageRepository.Delete(imageExisting);
                            successCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await SaveCleanupFailure(appDbContext, image, ex.Message);
                    listImageFailedToDelete.Add($"====Failed to delete" +
                        $"\n - id: {image.Id}" +
                        $"\n - Name: {image.ImageName}" +
                        $"\n - Path {image.ImagePath}" +
                        $"\n - Error Message: {ex.Message}");
                }
            }

            if (listImageFailedToDelete.Count > 0)
            {
                string message = string.Join("\n\n", listImageFailedToDelete);
                string emailBody = $@"
                    <h2>Image Cleanup Report</h2>
                    <p><strong>Total Expired Images Found:</strong> {listExpiredImages.Count}</p>
                    <p><strong>Successfully Deleted:</strong> {successCount}</p>
                    <p><strong>Failed to Delete:</strong> {listImageFailedToDelete.Count}</p>
                    <hr/>
                    <h3>Failed Deletions:</h3>
                    <pre>{message}</pre>
                    <p><em>Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</em></p>
                ";

                try
                {
                    await emailService.SendSystemMailAsync(
                        subject: $"ReadNest - Image Cleanup Failed ({listImageFailedToDelete.Count} errors)",
                        body: emailBody
                    );

                    _logger.LogWarning($"Sent email notification for {listImageFailedToDelete.Count} failed image deletions");
                }
                catch (Exception emailEx)
                {
                    _logger.LogError($"Failed to send email notification: {emailEx.Message}");
                }
            }
            else
            {
                _logger.LogInformation($"Successfully deleted {successCount} expired images");
            }
        }

        private async Task SaveCleanupFailure(AppDbContext appDbContext, ImagePathResult image, string errorMessage)
        {
            var existingFailure = await appDbContext.ImageCleanupFailures
                .FirstOrDefaultAsync(f => f.ImageId == image.Id);

            if (existingFailure == null)
            {
                var failure = new ImageCleanupFailure
                {
                    ImageId = image.Id,
                    ImageName = image.ImageName,
                    ImagePath = image.ImagePath,
                    ErrorMessage = errorMessage,
                    FirstAttempt = DateTime.Now
                };
                await appDbContext.ImageCleanupFailures.AddAsync(failure);
                await appDbContext.SaveChangesAsync();
            }
        }

        private async Task RetryFailedDeletions(AppDbContext appDbContext, IFileService fileService, IImageRepository imageRepository, IEmailService emailService)
        {
            var failedDeletions = await appDbContext.ImageCleanupFailures.ToListAsync();

            if (failedDeletions.Count == 0)
                return;

            List<string> retrySuccess = new List<string>();
            List<string> retryFailed = new List<string>();

            foreach (var failure in failedDeletions)
            {
                try
                {
                    if (File.Exists(Path.Combine(
                        Directory.GetCurrentDirectory(),
                        failure.ImagePath!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))))
                    {
                        await fileService.DeleteImageAsync(failure.ImagePath!);
                    }

                    var image = await imageRepository.GetById(failure.ImageId!);
                    if (image != null)
                    {
                        await imageRepository.Delete(image);
                    }

                    appDbContext.ImageCleanupFailures.Remove(failure);
                    await appDbContext.SaveChangesAsync();
                    retrySuccess.Add(failure.ImageId!);
                }
                catch (Exception ex)
                {
                    failure.ErrorMessage = ex.Message;
                    retryFailed.Add($"{failure.ImageId}: {ex.Message}");
                }
            }

            if (retrySuccess.Count > 0)
            {
                _logger.LogInformation($"Retry: Successfully deleted {retrySuccess.Count} previously failed images");
            }

            if (retryFailed.Count > 0)
            {
                _logger.LogWarning($"Retry: {retryFailed.Count} images still failed");
            }
        }
    }
}