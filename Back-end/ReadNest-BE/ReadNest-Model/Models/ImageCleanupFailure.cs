namespace ReadNest_Models
{
    public class ImageCleanupFailure : BaseModel
    {
        public string? ImageId { get; set; }
        public string? ImageName { get; set; }
        public string? ImagePath { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime FirstAttempt { get; set; }
    }
}