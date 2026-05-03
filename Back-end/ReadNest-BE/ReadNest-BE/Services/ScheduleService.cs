using Microsoft.EntityFrameworkCore;
using ReadNest_BE.Infrastructure;
using ReadNest_BE.Interfaces.Repositories;
using ReadNest_BE.Interfaces.Services;
using ReadNest_BE.Models;
using ReadNest_Models;

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

            var listImageNotUsed = await appDbContext.Database
                .SqlQueryRaw<ImagePathResult>(@"
                    SELECT i.Id, i.ImageName, i.ImagePath
                    FROM Images i
                    WHERE i.Id NOT IN (
                        SELECT ImageId FROM Novels WHERE ImageId IS NOT NULL
                        UNION
                        SELECT ImageId FROM Volumns WHERE ImageId IS NOT NULL
                        UNION
                        SELECT ImageId FROM Contents WHERE ImageId IS NOT NULL
                    )
                ")
                .ToListAsync();

            if (listImageNotUsed.Count == 0)
            {
                return;
            }

            List<string> listImageFailedToDelete = new List<string>();
            int successCount = 0;

            foreach (var image in listImageNotUsed)
            {
                try
                {
                    await fileService.DeleteImageAsync(image.ImagePath!);
                    var imageExisting = await imageRepository.GetById(image.Id);
                    await imageRepository.Delete(imageExisting);
                    successCount++;
                }
                catch (Exception ex)
                {
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
                    <p><strong>Total Images Found:</strong> {listImageNotUsed.Count}</p>
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
                _logger.LogInformation($"Successfully deleted {successCount} unused images");
            }
        }
    }
}