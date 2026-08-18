using static Core.DB.ECollegeEnums;

namespace Core.ViewModels
{
    public class AnnouncementViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public AnnouncementAudience Audience { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActiveNow { get; set; }
        public bool IsUpcoming { get; set; }
        public bool IsExpired { get; set; }
        public string? StatusLabel { get; set; }
        public string? AudienceLabel { get; set; }
    }
}
