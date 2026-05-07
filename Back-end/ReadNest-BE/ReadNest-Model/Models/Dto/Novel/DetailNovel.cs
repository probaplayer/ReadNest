using ReadNest_Enums;

namespace ReadNest_Models
{
    public class NovelHomePage{
        public List<OverviewNovel>? RandomNovel { get; set; }
        public PaginationData<List<NovelResponese>>? PaginationData { get; set; }
    }
    public class  OverviewNovel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Summary { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class DetailNovel : ExtendModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? Summary { get; set; }
        public NovelStatus? Status { get; set; }
        public List<Category>? Categories { get; set; }
        public List<VolumnVsChapters>? VolumnVsChapters { get; set; }
        public string? ImageId { get; set; }
        public string? ImageUrl { get; set; }
        public string? CreateBy { get ; set ; }
        public string? UpdateBy { get ; set ; }
        public bool? IsLocked { get; set; }
        public string? SharedUserIds { get; set; }
        public bool? IsAdult { get; set; }
    }
}
