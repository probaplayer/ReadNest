namespace ReadNest_Models
{
    public class Image : BaseModel, ExtendModel
    {
        public string? ImageName { get; set; }
        public long? ImageSize { get; set; }
        public string? ImagePath { get; set; }
        public string? ImageUrl { get; set; }
        public string? ContentType { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
