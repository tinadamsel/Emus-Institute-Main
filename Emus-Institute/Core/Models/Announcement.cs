using System.ComponentModel.DataAnnotations.Schema;
using static Core.DB.ECollegeEnums;

namespace Core.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public AnnouncementAudience Audience { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        public string CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser? CreatedBy { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool Active { get; set; } = true;
        public DateTime DateCreated { get; set; }
    }
}
