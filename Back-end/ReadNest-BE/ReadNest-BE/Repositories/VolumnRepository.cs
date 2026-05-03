using Mapster;
using Microsoft.EntityFrameworkCore;
using ReadNest_BE.Infrastructure;
using ReadNest_BE.Interfaces.Repositories;
using ReadNest_BE.Models;
using ReadNest_BE.Repositories.Shares;
using ReadNest_BE.Services;
using ReadNest_Models;

namespace ReadNest_BE.Repositories
{
    public class VolumnRepository : ExtendRepository<Volumn>, IVolumnRepository
    {
        IChapterRepository _chapterRepository;

        public VolumnRepository( AppDbContext appDbContext, JwtService jwtService, IChapterRepository chapterRepository) : base(appDbContext, jwtService)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<bool> DeleteRange(List<Volumn> entities)
        {
            try
            {
                if (entities.Count == 0)
                    return true;
                foreach (var entity in entities)
                {
                    await _chapterRepository.DeleteByVolumnId(entity.Id);
                    _dbSet.Remove(entity);
                }
                var result = await SaveChange();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> DeleteByNovelId(string novelId)
        {
            var items = await _dbSet.Where(c => c.NovelId!.Equals(novelId)).ToListAsync();

            if (items.Count == 0)
                return true;
            foreach (var item in items)
            {
                await _chapterRepository.DeleteByVolumnId(item.Id);
                _dbSet.Remove(item);
            }
            var result = await SaveChange();
            return result;
        }

        public async Task<List<Volumn>> GetVolumesByNovelId(string novelId)
        {
            var result = await _dbSet.Where(n => n.NovelId!.Equals(novelId)).ToListAsync();
            return result;
        }

        public async Task<List<VolumnContent>> GetVolumnContentsByChapterId(string chapterId)
        {
            var novelId = await _context.Database
                .SqlQueryRaw<IdResult>(@"
                        SELECT n.Id
                        FROM Chapters c
                        INNER JOIN Volumns v ON c.VolumnId = v.Id
                        INNER JOIN Novels n ON v.NovelId = n.Id
                        WHERE c.Id = {0}
                    ", chapterId)
                .FirstOrDefaultAsync();

            if (novelId == null)
                throw new Exception("Chapter, Volumn or Novel not found");

            var rows = await _context.Database
                .SqlQueryRaw<VolumeChapterRow>(@"
                    SELECT
	                    v.Id AS VolumnId,
	                    v.Name AS VolumnName,
	                    v.NovelId AS NovelId,
	                    c.Id AS ChapterId,
	                    c.Name AS ChapterName
                    FROM Volumns v
                    LEFT JOIN Chapters c ON v.Id = c.VolumnId
                    WHERE v.NovelId = {0}
                    ORDER BY v.[Order], c.[Order]
                ", novelId.Id!)
                .ToListAsync();

            var result = rows
                .GroupBy(r => new { r.VolumnId, r.VolumnName, r.NovelId })
                .Select(g => new VolumnContent
                {
                    Id = g.Key.VolumnId,
                    Name = g.Key.VolumnName,
                    NovelId = g.Key.NovelId,
                    Chapters = g
                        .Where(x => !string.IsNullOrEmpty(x.ChapterId))
                        .Select(x => new ChapterName
                        {
                            Id = x.ChapterId,
                            Name = x.ChapterName
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }




    }
}
