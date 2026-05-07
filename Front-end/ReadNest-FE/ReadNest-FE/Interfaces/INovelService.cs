using ReadNest_Models;

namespace ReadNest_FE.Interfaces
{
    public interface INovelService : IRequestHandler<Novel>
    {
        Task<Response<NovelHomePage>> GetNovelsInHomePage(int page = 1, int pageSize = 5);
        new Task<Response<PaginationData<List<NovelResponese>>>> GetValue(string? keyword, int page = 1, int pageSize = 5);
        Task<Response<PaginationData<List<NovelResponese>>>> GetNovelsFilter(NovelFilter novelFilter);
        new Task<Response<DetailNovel>> GetValueById(string id);
        Task<Response<DetailNovel>> DetailPost(DetailNovel detailNovel);
        Task<Response<DetailNovel>> DetailPut(DetailNovel detailNovel);
        Task<Response<bool>> ShareNovel(string novelId, string userId);

    }
}
