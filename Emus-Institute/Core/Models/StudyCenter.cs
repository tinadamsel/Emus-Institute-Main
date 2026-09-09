using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Core.DB.ECollegeEnums;

namespace Core.Models
{
    public class StudyCenter : Basemodel
    {
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public StudyCenterApprovalStatus Status { get; set; } = StudyCenterApprovalStatus.Pending;
        public string? ApprovedByUserId { get; set; }

        [Display(Name = "Approved By")]
        [ForeignKey("ApprovedByUserId")]
        public virtual ApplicationUser? ApprovedBy { get; set; }

        public DateTime? DateApproved { get; set; }
    }
}
