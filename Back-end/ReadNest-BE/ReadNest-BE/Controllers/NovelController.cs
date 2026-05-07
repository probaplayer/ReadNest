using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using ReadNest_BE.Exceptions;
using ReadNest_BE.Interfaces.Repositories;
using ReadNest_BE.Repositories;
using ReadNest_BE.Services;
using ReadNest_Enums;
using ReadNest_Models;

namespace ReadNest_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NovelController : BaseController<Novel>
    {
        IImageRepository _imageRepository;
        IReadingHistoryRepository _ReadingHistoryRepository;
        IChapterRepository _chapterRepository;
        ICategoryNovelRepository _categoryNovelRepository;
        IVolumnRepository _volumnRepository;
        public NovelController(INovelRepository repository, 
            JwtService jwtService, 
            IImageRepository imageRepository,
            IReadingHistoryRepository readingHistoryRepository,
            IChapterRepository chapterRepository,
            ICategoryNovelRepository categoryNovelRepository,
            IVolumnRepository volumnRepository
            ) : base(repository, jwtService)
        {
            _ReadingHistoryRepository = readingHistoryRepository;
            _imageRepository = imageRepository;
            _chapterRepository = chapterRepository;
            _categoryNovelRepository = categoryNovelRepository;
            _volumnRepository = volumnRepository;
        }
        [HttpGet]
        public override async Task<IActionResult> Get([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            if (!CanRead(out var errorMessage))
            {
                return Unauthorized(errorMessage);
            }
            try
            {
                var result = await ((INovelRepository)_repository).GetNovelResponse(keyword, page, pageSize);
                return Ok(new Response<PaginationData<List<NovelResponese>>>(result, "Get list of novel successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetNovelHomePage([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            if (!CanRead(out var errorMessage))
            {
                return Unauthorized(errorMessage);
            }
            try
            {
                var listNovel = await ((INovelRepository)_repository).GetNovelResponse(string.Empty, page, pageSize);
                var novelRandom = await ((INovelRepository)_repository).GetRandomNovels(4);
                return Ok(new Response<NovelHomePage>(new NovelHomePage()
                {
                    RandomNovel = novelRandom,
                    PaginationData = listNovel
                }, "Get list of novel successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetNovelWithFilter([FromBody] NovelFilter novelFilter)
        {
            if (!CanRead(out var errorMessage))
            {
                return Unauthorized(errorMessage);
            }
            try
            {
                var listNovel = await ((INovelRepository)_repository).GetNovelsFilter(novelFilter);
                return Ok(new Response<PaginationData<List<NovelResponese>>>(listNovel, "Get list of novel successfully", true));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> Get(string id)
        {
            if (!CanRead(out var errorMessage))
            {
                return Unauthorized(errorMessage);
            }
            try
            {
                var entity = await ((INovelRepository)_repository).GetDetailNovelById(id);
                if (entity == null)
                {
                    return NotFound(new Response<DetailNovel>(null, "Novel not found", false));
                }

                if (entity.IsLocked == true)
                {
                    string currentUserId = GetUserIdFromToken();
                    string userRoles = _jwtService.GetRoleFromToken() ?? "";
                    bool isAdmin = Utils.Utils.IsRole(userRoles, RoleType.ADMIN);
                    bool isAuthor = entity.CreateBy == currentUserId;

                    bool isShared = !string.IsNullOrEmpty(entity.SharedUserIds) &&
                        entity.SharedUserIds.Split(',').Any(uid => uid == currentUserId);

                    if (!isAdmin && !isAuthor && !isShared)
                    {
                        return StatusCode(403, new Response<DetailNovel>(null, "You do not have permission to read this novel", false));
                    }
                }

                string imgId = entity.ImageId!;
                if (!string.IsNullOrEmpty(imgId))
                {
                    var image = await _imageRepository.GetById(imgId);
                    if (image != null)
                    {
                        entity.ImageUrl = image.ImageUrl;
                    }
                    if(entity.VolumnVsChapters is not null)
                    {
                        foreach (var vol in entity.VolumnVsChapters)
                        {
                            string volImageId = vol.Volumn?.ImageId!;
                            if (!string.IsNullOrEmpty(volImageId))
                            {
                                var Volimage = await _imageRepository.GetById(volImageId);
                                if (image != null)
                                {
                                    string volImageUrl = Volimage.ImageUrl!;
                                    vol.Volumn!.ImageUrl = volImageUrl;
                                }
                            }
                        }
                    }    
                }
                return Ok(new Response<DetailNovel>(entity, "Get novel successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/share")]
        public async Task<IActionResult> ShareNovel(string id, [FromBody] ShareNovelRequest request)
        {
            if (!CanRead(out var errorMessage))
            {
                return Unauthorized(errorMessage);
            }
            try
            {
                var entity = await _repository.GetById(id);
                if (entity == null)
                {
                    return NotFound(new Response<bool>(false, "Novel not found", false));
                }

                string currentUserId = GetUserIdFromToken();
                string userRoles = _jwtService.GetRoleFromToken() ?? "";
                bool isAdmin = Utils.Utils.IsRole(userRoles, RoleType.ADMIN);
                bool isAuthor = entity.CreateBy == currentUserId;

                if (!isAdmin && !isAuthor)
                {
                    return StatusCode(403, new Response<bool>(false, "You do not have permission to share this novel", false));
                }

                var sharedIds = new List<string>();
                if (!string.IsNullOrEmpty(entity.SharedUserIds))
                {
                    sharedIds = entity.SharedUserIds.Split(',').ToList();
                }

                if (!sharedIds.Contains(request.UserId))
                {
                    sharedIds.Add(request.UserId);
                }

                entity.SharedUserIds = string.Join(",", sharedIds);
                await _repository.Update(entity);

                return Ok(new Response<bool>(true, "User added successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("detail")]
        [EnableRateLimiting("upsert")]
        public async Task<IActionResult> Post([FromBody] DetailNovel detailNovel)
        {
            if (!IsAuthorized(null!, true, out var errorMessage, out var userId))
                return Unauthorized(errorMessage);
            try
            {
                var novel = detailNovel.Adapt<Novel>();
                var prepareNovel = PrepareEntityForCreate(novel, userId);
                var createdNovel = await _repository.Create(prepareNovel);
                detailNovel.Id = createdNovel.Id;

                if (!string.IsNullOrEmpty(createdNovel.ImageId))
                {
                    var img = await _imageRepository.GetById(createdNovel.ImageId);
                    if (img != null)
                    {
                        img.ExpiresAt = null;
                        await _imageRepository.Update(img);
                    }
                }

                await _categoryNovelRepository.CreateRangeWithNovelId(detailNovel.Categories!, detailNovel.Id);

                if (detailNovel.VolumnVsChapters is not null && detailNovel.VolumnVsChapters.Count > 0)
                {
                    foreach (var vol in detailNovel.VolumnVsChapters)
                    {
                        var newVolumn = vol.Volumn ?? new Volumn();
                        newVolumn.NovelId = detailNovel.Id;
                        newVolumn = await _volumnRepository.CreateOrUpdate(newVolumn, newVolumn.Id);

                        if (!string.IsNullOrEmpty(newVolumn.ImageId))
                        {
                            var volImg = await _imageRepository.GetById(newVolumn.ImageId);
                            if (volImg != null)
                            {
                                volImg.ExpiresAt = null;
                                await _imageRepository.Update(volImg);
                            }
                        }

                        if (vol.Chapters is not null && vol.Chapters.Count > 0)
                        {
                            foreach (var chapter in vol.Chapters)
                            {
                                var newChapter = chapter;
                                newChapter.VolumnId = newVolumn.Id;
                                await _chapterRepository.CreateOrUpdate(newChapter, newChapter.Id);
                            }
                        }
                    }
                }

                var newDetailNovel = await ((INovelRepository)_repository).GetDetailNovelById(detailNovel.Id);
                if (!string.IsNullOrEmpty(newDetailNovel.ImageId))
                {
                    var img = await _imageRepository.GetById(newDetailNovel.ImageId);
                    if (img != null)
                    {
                        newDetailNovel.ImageUrl = img.ImageUrl;
                    }
                    if (newDetailNovel.VolumnVsChapters is not null && newDetailNovel.VolumnVsChapters.Count > 0)
                        foreach (var vol in newDetailNovel.VolumnVsChapters)
                        {
                            if (!string.IsNullOrEmpty(vol.Volumn!.ImageId!))
                            {
                                var volImg = await _imageRepository.GetById(vol.Volumn.ImageId);
                                if (volImg != null)
                                {
                                    vol.Volumn.ImageUrl = volImg.ImageUrl;
                                }
                            }
                        }
                }
                return Ok(new Response<DetailNovel>(newDetailNovel, "Novel created successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("detail")]
        [EnableRateLimiting("upsert")]
        public async Task<IActionResult> Put([FromBody] DetailNovel detailNovel)
        {
            var entity = await _repository.GetById(detailNovel.Id!);
            if (entity == null)
                return NotFound($"Novel not found");
            if (!IsAuthorized(entity, false, out var errorMessage, out var userId))
                return Unauthorized(errorMessage);
            try
            {
                entity = detailNovel.Adapt<Novel>();
                var prepareNovel = PrepareEntityForUpdate(entity, entity, userId);
                var updatedEntity = await _repository.Update(prepareNovel);

                if (!string.IsNullOrEmpty(updatedEntity.ImageId))
                {
                    var img = await _imageRepository.GetById(updatedEntity.ImageId);
                    if (img != null)
                    {
                        img.ExpiresAt = null;
                        await _imageRepository.Update(img);
                    }
                }

                await _categoryNovelRepository.CreateRangeWithNovelId(detailNovel.Categories!, updatedEntity.Id);

                var oldVolumes = await _volumnRepository.GetVolumesByNovelId(updatedEntity.Id);
                if (detailNovel.VolumnVsChapters is not null && detailNovel.VolumnVsChapters.Count > 0)
                {
                    var oldVolumeIds = oldVolumes.Select(v => v.Id).ToHashSet();

                    foreach (var vol in detailNovel.VolumnVsChapters)
                    {
                        var volumn = vol.Volumn ?? new Volumn();
                        volumn.NovelId = updatedEntity.Id;

                        var savedVolumn = await _volumnRepository.CreateOrUpdate(volumn, volumn.Id);

                        if (!string.IsNullOrEmpty(savedVolumn.ImageId))
                        {
                            var volImg = await _imageRepository.GetById(savedVolumn.ImageId);
                            if (volImg != null)
                            {
                                volImg.ExpiresAt = null;
                                await _imageRepository.Update(volImg);
                            }
                        }

                        var oldChapters = await _chapterRepository.GetChaptersByVolumnId(savedVolumn.Id);
                        if (vol.Chapters is not null && vol.Chapters.Count > 0)
                        {
                            var oldChapterIds = oldChapters.Select(c => c.Id).ToHashSet();

                            List<Chapter> chaptersToKeep = vol.Chapters
                                .Where(c => string.IsNullOrEmpty(c.CreateBy) || oldChapterIds.Contains(c.Id))
                                .ToList();

                            foreach (var chapter in chaptersToKeep)
                            {
                                chapter.VolumnId = savedVolumn.Id;
                            }

                            var chapterIdsToKeep = chaptersToKeep
                                .Where(c => !string.IsNullOrEmpty(c.Id))
                                .Select(c => c.Id)
                                .ToHashSet();

                            var chaptersToDelete = oldChapters
                                .Where(c => !chapterIdsToKeep.Contains(c.Id))
                                .ToList();

                            if (chaptersToDelete.Count > 0)
                            {
                                await _chapterRepository.DeleteRange(chaptersToDelete);
                            }

                            await _chapterRepository.CreateOrUpdateMany(chaptersToKeep, false);
                        }
                        else
                        {
                            await _chapterRepository.DeleteRange(oldChapters);
                        }
                    }

                    var volumeIdsToKeep = detailNovel.VolumnVsChapters
                        .Where(v => v.Volumn != null && !string.IsNullOrEmpty(v.Volumn.Id))
                        .Select(v => v.Volumn!.Id)
                        .ToHashSet();

                    var volumesToDelete = oldVolumes
                        .Where(v => !volumeIdsToKeep.Contains(v.Id))
                        .ToList();

                    if (volumesToDelete.Count > 0)
                    {
                        await _volumnRepository.DeleteRange(volumesToDelete);
                    }
                }
                else
                {
                    await _volumnRepository.DeleteRange(oldVolumes);
                }

                var newDetailNovel = await ((INovelRepository)_repository).GetDetailNovelById(updatedEntity.Id);
                if (!string.IsNullOrEmpty(newDetailNovel.ImageId))
                {
                    var img = await _imageRepository.GetById(newDetailNovel.ImageId);
                    if (img != null)
                    {
                        newDetailNovel.ImageUrl = img.ImageUrl;
                    }
                    if (newDetailNovel.VolumnVsChapters is not null && newDetailNovel.VolumnVsChapters.Count > 0)
                        foreach (var vol in newDetailNovel.VolumnVsChapters)
                        {
                            if (!string.IsNullOrEmpty(vol.Volumn!.ImageId))
                            {
                                var volImg = await _imageRepository.GetById(vol.Volumn.ImageId);
                                if (volImg != null)
                                {
                                    vol.Volumn.ImageUrl = volImg.ImageUrl;
                                }
                            }
                        }
                }

                return Ok(new Response<DetailNovel>(newDetailNovel, "Novel updated successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
