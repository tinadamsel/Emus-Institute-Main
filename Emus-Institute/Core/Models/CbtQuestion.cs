using System.ComponentModel.DataAnnotations.Schema;
using static Core.DB.ECollegeEnums;

namespace Core.Models
{
    public class CbtQuestion
    {
        public int Id { get; set; }
        public int CbtTestId { get; set; }
        [ForeignKey("CbtTestId")]
        public virtual CbtTest? CbtTest { get; set; }
        public CbtQuestionType QuestionType { get; set; }
        public string QuestionText { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string CorrectAnswer { get; set; }
        public decimal Marks { get; set; }
        public string? Explanation { get; set; }
        public string? ImagePath { get; set; }
        public int SortOrder { get; set; }
        public bool Active { get; set; } = true;
        public DateTime DateCreated { get; set; }
    }
}
