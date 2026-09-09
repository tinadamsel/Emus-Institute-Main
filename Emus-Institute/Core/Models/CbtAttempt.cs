using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class CbtAttempt
    {
        public int Id { get; set; }
        public int CbtTestId { get; set; }
        [ForeignKey("CbtTestId")]
        public virtual CbtTest? CbtTest { get; set; }
        public string? StudentUserId { get; set; }
        [ForeignKey("StudentUserId")]
        public virtual ApplicationUser? Student { get; set; }
        public int? AssessmentRegistrationId { get; set; }
        [ForeignKey("AssessmentRegistrationId")]
        public virtual AssessmentRegistration? AssessmentRegistration { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal Score { get; set; }
        public decimal TotalMarks { get; set; }
        public bool Passed { get; set; }
        public bool IsSubmitted { get; set; }
        public bool AutoSubmitted { get; set; }
        public virtual ICollection<CbtStudentAnswer> Answers { get; set; } = new List<CbtStudentAnswer>();
    }
}
