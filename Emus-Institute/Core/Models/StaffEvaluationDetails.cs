using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class StaffEvaluationDetails
    {
        public int Id { get; set; }
        public string? Passport { get; set; }
        public string? SchoolTranscript { get; set; }
        public string? HighSchoolCompletionCertificate { get; set; }
        public string? OtherCertificate { get; set; }
        public string? WAECScratchCard { get; set; }
        public string UserId { get; set; }
        [Display(Name = "User")]
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Users { get; set; }
        public DateTime? DateAdded { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? DateApproved { get; set; }
    }
}
