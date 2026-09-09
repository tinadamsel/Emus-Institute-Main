using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class AssignmentSubmission : Basemodel
    {
        public int AssignmentId { get; set; }
        [ForeignKey("AssignmentId")]
        public virtual Assignment? Assignment { get; set; }
        public string StudentUserId { get; set; }
        [ForeignKey("StudentUserId")]
        public virtual ApplicationUser? Student { get; set; }
        public string? FilePath { get; set; }
        public string? OriginalFileName { get; set; }
        public string? Comment { get; set; }
        public DateTime SubmittedAt { get; set; }
        public decimal? Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime? GradedAt { get; set; }
        public string? GradedByUserId { get; set; }
        [ForeignKey("GradedByUserId")]
        public virtual ApplicationUser? GradedBy { get; set; }
    }
}
