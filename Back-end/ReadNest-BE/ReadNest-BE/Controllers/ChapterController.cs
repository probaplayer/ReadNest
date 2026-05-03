using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Distributed;
using ReadNest_BE.Interfaces.Repositories;
using ReadNest_BE.Repositories;
using ReadNest_BE.Services;
using ReadNest_Models;
using System.Text.Json;

namespace ReadNest_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : BaseController<Chapter>
    {
        IContentRepository _contentRepository;
        IImageRepository _imageRepository;

        public ChapterController(IChapterRepository repository, JwtService jwtService, IContentRepository contentRepository, IImageRepository imageRepository) : base(repository, jwtService)
        {
            _contentRepository = contentRepository;
            _imageRepository = imageRepository;
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetailById(string id)
        {
            if (!CanRead(out var errorMessage))
                return Unauthorized(errorMessage);
            try
            {
                DetailChapter result = await ((IChapterRepository)_repository).GetDetailChapterById(id);
                var response = new Response<DetailChapter>(result, "Get success", true);
                var json = JsonSerializer.Serialize(response);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("mutiple/chapter/{chapterId}/contents")]
        [EnableRateLimiting("upsert")]
        public async Task<IActionResult> PostMutiple([FromRoute] string chapterId, [FromBody] List<Content> contents)
        {
            var entity = await _repository.GetById(chapterId);
            if (entity == null)
                return NotFound($"chapter not found");

            if (!IsAuthorized(entity, false, out var errorMessage, out var userId))
                return Unauthorized(errorMessage);

            try
            {
                var oldContents = await _contentRepository.GetContentsByChapterId(chapterId);
                await _contentRepository.DeleteRange(oldContents);
                List<Content> newContent = contents.Adapt<List<Content>>();
                
                foreach (var content in newContent)
                {
                    if (!string.IsNullOrEmpty(content.ImageId))
                    {
                        var img = await _imageRepository.GetById(content.ImageId);
                        if (img != null)
                        {
                            img.ExpiresAt = null;
                            await _imageRepository.Update(img);
                        }
                    }
                }
                
                var updatedEntity = PrepareEntityForUpdate(entity, entity, userId);
                var createdContents = await _contentRepository.CreateOrUpdateMany(newContent, true);
                var responeseContents = createdContents.Adapt<List<Content>>();
                await ((IChapterRepository)_repository).Update(updatedEntity);
                return Ok(new Response<List<Content>>(responeseContents, "Update contents for chapter successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
