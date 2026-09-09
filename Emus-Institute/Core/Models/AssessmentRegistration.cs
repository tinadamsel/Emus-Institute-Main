using System.ComponentModel.DataAnnotations.Schema;
using static Core.DB.ECollegeEnums;

namespace Core.Models
{
    public class AssessmentRegistration
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public AssessmentProgramType ProgramType { get; set; }
        public string Country { get; set; } = string.Empty;
        public AssessmentScholarshipType ScholarshipType { get; set; }
        public int CbtTestId { get; set; }
        [ForeignKey("CbtTestId")]
        public virtual CbtTest? CbtTest { get; set; }
        public Guid AccessToken { get; set; }
        public Guid? PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }
        public bool IsPaid { get; set; }
        public bool HasOpenedQuiz { get; set; }
        public int? AttemptId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
