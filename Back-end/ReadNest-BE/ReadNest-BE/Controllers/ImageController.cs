using Microsoft.AspNetCore.Mvc;
using ReadNest_BE.Interfaces.Repositories;
using ReadNest_BE.Interfaces.Services;
using ReadNest_BE.Services;
using ReadNest_Models;
using static Dapper.SqlMapper;

namespace ReadNest_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : BaseController<Image>
    {
        private readonly IFileService _fileService;
        public ImageController(IImageRepository repository, JwtService jwtService, IFileService fileService) :
            base(repository, jwtService)
        {
            _fileService = fileService;
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
                string baseUrl = $"{Request.Scheme}://{Request.Host}";

                var entites = await _repository.GetAll();
                entites = entites.Where(e =>
                {
                    var valueName = e.GetType().GetProperty("Name")?.GetValue(e)?.ToString();
                    var valueP = e.GetType().GetProperty("P")?.GetValue(e)?.ToString();
                    return string.IsNullOrEmpty(keyword) ||
                           (!string.IsNullOrEmpty(valueName) && valueName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                           (!string.IsNullOrEmpty(valueP) && valueP.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                }).OrderByDescending(c => c.CreateDate).ToList();

                int totalRow = entites.Count;
                pageSize = pageSize < 1 ? 5 : pageSize;
                int pageCount = (int)Math.Ceiling(totalRow / (double)pageSize);

                if (page > pageCount)
                    page = pageCount;
                else if (page < 1)
                    page = 1;

                entites = entites.Select(i =>
                {
                    i.ImageUrl = $"{baseUrl}{i.ImagePath}";
                    return i;
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                var paginationData = new PaginationData<List<Image>>(entites, page, pageSize, totalRow);
                return Ok(new Response<PaginationData<List<Image>>>(paginationData, "Get list of images successfully", true));
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
                var entity = await _repository.GetById(id);
                return Ok(new Response<Image>(entity, "Get image successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("single")]
        public async Task<IActionResult> UploadSingleImage(
            IFormFile file,
            [FromQuery] string folder = "images")
        {
            if (!IsAuthorized(null!, true, out var errorMessage, out var userId))
                return Unauthorized(errorMessage);
            try
            {
                ImageResponse uploadResult = null!;

                if (file == null)
                {
                    return BadRequest("No file uploaded");
                }

                uploadResult = await _fileService.UploadImageAsync(file, folder);

                var imageEntity = new Image
                {
                    ImageName = uploadResult.ImageName,
                    ImagePath = uploadResult.ImagePath,
                    ImageUrl = uploadResult.ImageUrl,
                    ImageSize = uploadResult.ImageSize,
                    ContentType = uploadResult.ContentType,
                    CreateBy = GetUserIdFromToken(),
                    UpdateBy = GetUserIdFromToken(),
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(15)
                };

                var savedImage = await _repository.Create(imageEntity);

                return Ok(new Response<Image>(
                    savedImage,
                    "Image uploaded successfully",
                    true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> UploadMultipleImages(List<IFormFile> files, [FromQuery] string folder = "images")
        {
            if (!IsAuthorized(null!, true,out var errorMessage, out var userId))
                return Unauthorized(errorMessage);
            try
            {
                var imagesUploaded = new List<ImageResponse>();
                if (files == null || files.Count == 0)
                {
                    return BadRequest("No files uploaded");
                }

                imagesUploaded = await _fileService.UploadMultipleImagesAsync(files, folder);
                var imageToResponses = new List<Image>();
                foreach (var img in imagesUploaded)
                {
                    var i = await _repository.Create(new Image()
                    {
                        ImageName = img.ImageName,
                        ImagePath = img.ImagePath,
                        ImageUrl = img.ImageUrl,
                        ImageSize = img.ImageSize,
                        ContentType = img.ContentType,
                        CreateBy = GetUserIdFromToken(),
                        UpdateBy = GetUserIdFromToken(),
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        ExpiresAt = DateTime.Now.AddMinutes(15)
                    });
                    imageToResponses.Add(i);
                }
                return Ok(new Response<List<Image>>(imageToResponses, $"{imagesUploaded.Count} images uploaded successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public override async Task<IActionResult> Delete(string id)
        {
            var image = await _repository.GetById(id);
            if (image == null)
            {
                return NotFound("Image not found");
            }
            if (!IsAuthorized(image, false, out var errorMessage, out var userId))
                return Unauthorized(errorMessage);
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("File Id is required");
                }
                var result = await _repository.Delete(image);
                if (result)
                {
                    return Ok(new Response<bool>(true, "Image deleted successfully", true));
                }

                return BadRequest("Something wrong in delete image");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("mutiple")]
        public async Task<IActionResult> DeleteRange([FromBody] List<string> imageIds)
        {
            if (!IsAuthorized(null!, true, out var errorMessage, out var userId))
                return Unauthorized(errorMessage);
            try
            {
                string header = Request.Headers["Authorization"].ToString();
                _jwtService.Token = header;
                if (imageIds == null || imageIds.Count == 0)
                {
                    throw new Exception("Image Ids are required");
                }
                var images = new List<Image>();
                foreach (var id in imageIds)
                {
                    var image = await _repository.GetById(id);
                    if (image != null)
                    {
                        images.Add(image);
                    }
                }
                if (images.Count == 0)
                {
                    throw new Exception("No images found");
                }
                var result = await _repository.DeleteRange(images);
                return Ok(new Response<bool>(result, "Images deleted successfully", true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
