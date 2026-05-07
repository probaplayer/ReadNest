using Mapster;
using Microsoft.EntityFrameworkCore;
using ReadNest_BE.Exceptions;
using ReadNest_BE.Infrastructure;
using ReadNest_BE.Interfaces.Repositories;
using ReadNest_BE.Repositories.Shares;
using ReadNest_BE.Services;
using ReadNest_Models;
using System.Net.WebSockets;
using static Dapper.SqlMapper;

namespace ReadNest_BE.Repositories
{
    public class NovelRepository : ExtendRepository<Novel>, INovelRepository
    {
        IReadingHistoryRepository _readingHistoryRepository;
        ICategoryNovelRepository _categoryNovelRepository;
        IVolumnRepository _volumnRepository;
        IImageRepository _imageRepository;
        IHttpContextAccessor _httpContextAccessor;
        public NovelRepository(IReadingHistoryRepository readingHistoryRepository, IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext, JwtService jwtService, ICategoryNovelRepository categoryNovelRepository, IVolumnRepository volumnRepository, IImageRepository imageRepository) : base(appDbContext, jwtService)
        {
            _readingHistoryRepository = readingHistoryRepository;
            _categoryNovelRepository = categoryNovelRepository;
            _volumnRepository = volumnRepository;
            _imageRepository = imageRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<bool> Delete(Novel novel)
        {
            if (novel != null)
            {
                _dbSet.Remove(novel);
                var result = await SaveChange();
                var resultDeleteCategoryNovel = await _categoryNovelRepository.DeleteByNovelId(novel.Id);
                var resultDeleteVolumn = await _volumnRepository.DeleteByNovelId(novel.Id);
                var resdingHistory = await _context.ReadingHistorys.Where(r => r.NovelId == novel.Id).ToListAsync();
                await _readingHistoryRepository.DeleteRange(resdingHistory);
                var resultDeletImage = true;
                if (!string.IsNullOrEmpty(novel.ImageId))
                {
                    var image = await _imageRepository.GetById(novel.ImageId);
                    resultDeletImage = await _imageRepository.Delete(image);
                }
                
                result = result & resultDeleteCategoryNovel & resultDeleteVolumn & resultDeletImage;
                return result;
            }
            return false;
        }


        public async Task<DetailNovel> GetDetailNovelById(string id)
        {
            var novel = await _dbSet.FindAsync(id);
            if (novel == null)
                throw new Exception("Novel not found");

            var categoryIds = await _context.CategorieNovels
                .Where(cn => cn.NovelId == id)
                .Select(cn => cn.CategoryId)
                .ToListAsync();

            var categories = await _context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync();

            var volumns = await _context.Volumns.Where(v => v.NovelId!.Equals(id)).OrderBy(v => v.Order).ToListAsync();
            List<VolumnVsChapters> volumnVsChapters = new List<VolumnVsChapters>();
            foreach (var vol in volumns)
            {
                var chapters = await _context.Chapters
                    .Where(c => c.VolumnId!.Equals(vol.Id)).OrderBy(c => c.Order).ToListAsync();

                volumnVsChapters.Add(new VolumnVsChapters()
                {
                    Volumn = vol,
                    Chapters = chapters,
                });
            }

            var detailNovelResponese = novel.Adapt<DetailNovel>();
            detailNovelResponese.Categories = categories;
            detailNovelResponese.VolumnVsChapters = volumnVsChapters;

            return detailNovelResponese;
        }

        public async Task<PaginationData<List<NovelResponese>>> GetNovelResponse(string? keyword, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 100) pageSize = 100;

            int offset = (page - 1) * pageSize;

            string? sanitizedKeyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();

            var sql = @"
                SELECT 
                    n.Id AS Id,
                    n.Name AS NovelName,
                    n.Author AS Author,
                    n.Summary AS Summary,
                    i.ImagePath AS ImageUrl,
                    latest_ch.ChapterId,
                    latest_ch.ChapterName,
                    latest_ch.VolumnName,
                    latest_ch.TotalChapters AS TotalChapter,
                    latest_ch.UpdateDate AS LatestChapterUpdateDate,
                    (
                        SELECT GROUP_CONCAT(cat.Name, '; ')
                        FROM Categories cat
                        INNER JOIN CategorieNovels cn ON cn.CategoryId = cat.Id
                        WHERE cn.NovelId = n.Id
                    ) AS Categories,
                    n.CreateBy,
                    n.UpdateBy,
                    n.IsLocked,
                    n.SharedUserIds,
                    n.IsAdult
                FROM Novels n
                LEFT JOIN Images i ON i.Id = n.ImageId
                LEFT JOIN (
                    SELECT 
                        c.Id AS ChapterId,
                        c.Name AS ChapterName,
                        c.UpdateDate,
                        v.Name AS VolumnName,
                        v.NovelId,
                        COUNT(*) OVER (PARTITION BY v.NovelId) AS TotalChapters,
                        ROW_NUMBER() OVER (PARTITION BY v.NovelId ORDER BY c.UpdateDate DESC) AS rn
                    FROM Chapters c
                    INNER JOIN Volumns v ON v.Id = c.VolumnId
                ) AS latest_ch ON latest_ch.NovelId = n.Id AND latest_ch.rn = 1
                WHERE ({0} IS NULL OR {0} = '' OR 
                       n.Name LIKE CONCAT('%', {0}, '%') OR 
                       n.Author LIKE CONCAT('%', {0}, '%'))
                ORDER BY latest_ch.UpdateDate DESC
                LIMIT {1} OFFSET {2}";

            var novels = await _context.Database
                .SqlQueryRaw<NovelResponese>(sql, sanitizedKeyword!, pageSize, offset)
                .ToListAsync();
            var request = _httpContextAccessor.HttpContext?.Request;
            string baseUrl = $"{request?.Scheme}://{request?.Host}";
            foreach (var item in novels)
            {
                item.ImageUrl = $"{baseUrl}{item.ImageUrl}";
            }

            var query = _context.Novels.AsQueryable();
            if (!string.IsNullOrEmpty(sanitizedKeyword))
            {
                query = query.Where(n =>
                    n.Name!.Contains(sanitizedKeyword) ||
                    n.Author!.Contains(sanitizedKeyword));
            }

            var totalRecords = await query.CountAsync();

            return new PaginationData<List<NovelResponese>>(
                rows: novels,
                pageNumber: page,
                pageSize: pageSize,
                totalRecords: totalRecords
            );
        }

        public async Task<PaginationData<List<NovelResponese>>> GetNovelsFilter(NovelFilter filter)
        {
            int page = filter.Page ?? 1;
            int pageSize = filter.PageSize ?? 10;

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 100) pageSize = 100;
            int offset = (page - 1) * pageSize;

            string? sanitizedName = string.IsNullOrWhiteSpace(filter.NameNovel) ? null : filter.NameNovel.Trim();
            string? sanitizedAuthor = string.IsNullOrWhiteSpace(filter.Author) ? null : filter.Author.Trim();
            string? sanitizedCategories = string.IsNullOrWhiteSpace(filter.Categories) ? null : filter.Categories.Trim();

            List<string>? categoryList = null;
            int categoryCount = 0;
            if (!string.IsNullOrEmpty(sanitizedCategories))
            {
                categoryList = sanitizedCategories
                    .Split(';')
                    .Select(c => c.Trim())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList();
                categoryCount = categoryList.Count;
            }

            string categoryCondition = "";
            if (categoryList != null && categoryList.Any())
            {
                var categoryParams = string.Join(", ", Enumerable.Range(0, categoryList.Count).Select(i => $"{{{i + 3}}}"));
                categoryCondition = $@"
                    AND (
                        SELECT COUNT(DISTINCT cat.Id)
                        FROM Categories cat
                        INNER JOIN CategorieNovels cn ON cn.CategoryId = cat.Id
                        WHERE cn.NovelId = n.Id 
                        AND cat.Name IN ({categoryParams})
                    ) = {categoryCount}";
                }

                var sql = $@"
                    SELECT 
                        n.Id AS Id,
                        n.Name AS NovelName,
                        n.Author AS Author,
                        n.Summary AS Summary,
                        i.ImagePath AS ImageUrl,
                        latest_ch.ChapterId,
                        latest_ch.ChapterName,
                        latest_ch.VolumnName,
                        latest_ch.TotalChapters AS TotalChapter,
                        latest_ch.UpdateDate AS LatestChapterUpdateDate,
                        (
                            SELECT GROUP_CONCAT(cat.Name, '; ')
                            FROM Categories cat
                            INNER JOIN CategorieNovels cn ON cn.CategoryId = cat.Id
                            WHERE cn.NovelId = n.Id
                        ) AS Categories,
                        n.CreateBy,
                        n.UpdateBy,
                        n.IsLocked,
                        n.SharedUserIds,
                        n.IsAdult
                    FROM Novels n
                    LEFT JOIN Images i ON i.Id = n.ImageId
                    LEFT JOIN (
                        SELECT 
                            c.Id AS ChapterId,
                            c.Name AS ChapterName,
                            c.UpdateDate,
                            v.Name AS VolumnName,
                            v.NovelId,
                            COUNT(*) OVER (PARTITION BY v.NovelId) AS TotalChapters,
                            ROW_NUMBER() OVER (PARTITION BY v.NovelId ORDER BY c.UpdateDate DESC) AS rn
                        FROM Chapters c
                        INNER JOIN Volumns v ON v.Id = c.VolumnId
                    ) AS latest_ch ON latest_ch.NovelId = n.Id AND latest_ch.rn = 1
                    WHERE 
                        ({{0}} IS NULL OR {{0}} = '' OR n.Name LIKE CONCAT('%', {{0}}, '%'))
                        AND ({{1}} IS NULL OR {{1}} = '' OR n.Author LIKE CONCAT('%', {{1}}, '%'))
                        AND ({{2}} IS NULL OR n.Status = {{2}})
                        {categoryCondition}
                    ORDER BY latest_ch.UpdateDate DESC
                    LIMIT {{{categoryCount + 3}}} OFFSET {{{categoryCount + 4}}}";

            var parameters = new List<object?>
            {
                sanitizedName!,
                sanitizedAuthor!,
                filter.NovelStatus.HasValue ? (int)filter.NovelStatus.Value : (object)DBNull.Value
            };

            if (categoryList != null && categoryList.Any())
            {
                parameters.AddRange(categoryList);
            }

            parameters.Add(pageSize);
            parameters.Add(offset);

            var novels = await _context.Database
                .SqlQueryRaw<NovelResponese>(sql, parameters.ToArray()!)
                .ToListAsync();

            var request = _httpContextAccessor.HttpContext?.Request;
            string baseUrl = $"{request?.Scheme}://{request?.Host}";

            foreach (var item in novels)
            {
                item.ImageUrl = $"{baseUrl}{item.ImageUrl}";
            }

            var query = _context.Novels.AsQueryable();

            if (!string.IsNullOrEmpty(sanitizedName))
            {
                query = query.Where(n => n.Name!.Contains(sanitizedName));
            }

            if (!string.IsNullOrEmpty(sanitizedAuthor))
            {
                query = query.Where(n => n.Author!.Contains(sanitizedAuthor));
            }

            if (filter.NovelStatus.HasValue)
            {
                query = query.Where(n => n.Status == filter.NovelStatus.Value);
            }

            if (categoryList != null && categoryList.Any())
            {
                query = query.Where(n =>
                    _context.CategorieNovels
                        .Where(cn => cn.NovelId == n.Id)
                        .Join(_context.Categories,
                            cn => cn.CategoryId,
                            cat => cat.Id,
                            (cn, cat) => cat.Name)
                        .Distinct()
                        .Count(name => categoryList.Contains(name!)) == categoryCount);
            }

            var totalRecords = await query.CountAsync();

            return new PaginationData<List<NovelResponese>>(
                rows: novels,
                pageNumber: page,
                pageSize: pageSize,
                totalRecords: totalRecords
            );
        }

        public async Task<List<OverviewNovel>> GetRandomNovels(int count)
        {
            var sql = @"
                   SELECT n.Id, n.Name, n.Summary, i.ImagePath as ImageUrl
                FROM Novels n
                JOIN Images i
                on n.ImageId = i.id
                ORDER BY RANDOM() 
                LIMIT {0}";

            var novels = await _context.Database
                .SqlQueryRaw<OverviewNovel>(sql, count)
                .ToListAsync();

            var request = _httpContextAccessor.HttpContext?.Request;
            string baseUrl = $"{request?.Scheme}://{request?.Host}";
            foreach (var item in novels)
            {
                item.ImageUrl = $"{baseUrl}{item.ImageUrl}";
            }

            return novels;
        }

        public async Task UpdateDateNovelById(string novelId)
        {
            var novel = await _dbSet.FindAsync(novelId);
            if (novel is null)
                throw new Exception("Chapter not found");
            novel.UpdateDate = DateTime.Now;

            _dbSet.Update(novel);
            await SaveChange();
        }
    }
}
