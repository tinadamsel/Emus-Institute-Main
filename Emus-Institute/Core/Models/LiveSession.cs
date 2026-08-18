using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Core.DB.ECollegeEnums;

namespace Core.Models
{
    public class LiveSession
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string? WelcomeMessage { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        public string CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser? CreatedBy { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string RoomCode { get; set; }
        public LiveSessionStatus Status { get; set; } = LiveSessionStatus.Scheduled;
        public bool Active { get; set; } = true;
        public DateTime DateCreated { get; set; }
    }
}
