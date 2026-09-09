using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class CbtTest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        public bool IsPublicAssessment { get; set; }
        public string CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser? CreatedBy { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassMark { get; set; }
        public decimal MarkPerQuestion { get; set; }
        public int MaximumAttempts { get; set; } = 1;
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleOptions { get; set; }
        public bool IsPublished { get; set; }
        public string? BrowserCode { get; set; }
        public string? Instructions { get; set; }
        public bool Active { get; set; } = true;
        public DateTime DateCreated { get; set; }
        public virtual ICollection<CbtQuestion> Questions { get; set; } = new List<CbtQuestion>();
        public virtual ICollection<CbtAttempt> Attempts { get; set; } = new List<CbtAttempt>();
    }
}
