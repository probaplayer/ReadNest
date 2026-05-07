namespace ReadNest_Models
{
    public class NovelResponese
    {
        public string? Id { get; set; }
        public string? NovelName { get; set; }
        public string? Author { get; set; }
        public string? Summary { get; set; }
        public string? ImageUrl { get; set; }
        public string? ChapterId { get; set; }
        public string? ChapterName { get; set; }
        public string? VolumnName { get; set; }
        public int? TotalChapter { get; set; }
        public string? LatestChapterUpdateDate { get; set; }
        public string? Categories { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public bool? IsLocked { get; set; }
        public string? SharedUserIds { get; set; }
        public bool? IsAdult { get; set; }
    }
}
