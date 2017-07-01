using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeNetAPI.Models
{
    public class FlaggedComment
    {
        [Key,Required]
        public int ReportID { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string DateReported { get; set; }
        public int IsDealtWith { get; set; }
        public string DealtMessage { get; set; }
        [ForeignKey("AnnouncementCommentID")]
        public int AnnouncementCommentID { get; set; }
        public AnnouncementComment AnnouncementComment { get; set; }
    }
}
