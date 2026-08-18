using static Core.DB.ECollegeEnums;

namespace Core.ViewModels
{
    public class CbtQuestionViewModel
    {
        public int Id { get; set; }
        public int CbtTestId { get; set; }
        public string? TestTitle { get; set; }
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
        public bool CanEdit { get; set; }
        public List<CbtOptionViewModel> DisplayOptions { get; set; } = new();
    }

    public class CbtOptionViewModel
    {
        public string Key { get; set; }
        public string Text { get; set; }
    }
}
