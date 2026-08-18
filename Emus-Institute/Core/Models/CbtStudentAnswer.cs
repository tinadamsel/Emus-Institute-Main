using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class CbtStudentAnswer
    {
        public int Id { get; set; }
        public int CbtAttemptId { get; set; }
        [ForeignKey("CbtAttemptId")]
        public virtual CbtAttempt? CbtAttempt { get; set; }
        public int CbtQuestionId { get; set; }
        [ForeignKey("CbtQuestionId")]
        public virtual CbtQuestion? CbtQuestion { get; set; }
        public string? SelectedAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public decimal MarksAwarded { get; set; }
    }
}
