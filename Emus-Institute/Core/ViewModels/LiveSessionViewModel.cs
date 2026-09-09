using static Core.DB.ECollegeEnums;

namespace Core.ViewModels
{
    public class LiveSessionViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string? WelcomeMessage { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? StaffName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string? RoomCode { get; set; }
        public LiveSessionStatus Status { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActiveNow { get; set; }
        public bool CanJoin { get; set; }
        public bool IsUpcoming { get; set; }
        public string? StatusLabel { get; set; }
        public string? ScheduleDisplay { get; set; }
        public string? JitsiDisplayName { get; set; }
        public bool IsHost { get; set; }
        public string? JitsiJoinUrl { get; set; }
    }
}
