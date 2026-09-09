using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Assignment : Basemodel
    {
        public string Description { get; set; }
        public bool? IsSubmitted { get; set; }
        public DateTime ValidUntilDate { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Departments { get; set; }
        public string? CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser? CreatedBy { get; set; }
        public string? FilePath { get; set; }
        public decimal TotalMarks { get; set; }
        public virtual ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
    }
}
