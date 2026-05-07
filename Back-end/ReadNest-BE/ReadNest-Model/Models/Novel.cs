using ReadNest_Enums;
using System.ComponentModel.DataAnnotations;

namespace ReadNest_Models
{
    public class Novel : BaseModel, ExtendModel
    {
        public string? Name { get; set; }
        [MaxLength(30)]
        public string? Author { get; set; }
        public string? Summary { get; set; }
        public NovelStatus? Status { get; set; }
        public string? ImageId { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public bool? IsLocked { get; set; }
        public string? SharedUserIds { get; set; }
        public bool? IsAdult { get; set; }
    }
}
