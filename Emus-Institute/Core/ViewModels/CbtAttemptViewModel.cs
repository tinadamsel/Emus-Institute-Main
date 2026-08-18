namespace Core.ViewModels
{
    public class CbtAttemptViewModel
    {
        public int Id { get; set; }
        public int CbtTestId { get; set; }
        public string? TestTitle { get; set; }
        public string? StudentUserId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal Score { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassMark { get; set; }
        public bool Passed { get; set; }
        public bool IsSubmitted { get; set; }
        public bool AutoSubmitted { get; set; }
        public int DurationMinutes { get; set; }
        public int RemainingSeconds { get; set; }
        public string? Instructions { get; set; }
        public bool RequiresBrowserCode { get; set; }
        public List<CbtQuestionViewModel> Questions { get; set; } = new();
        public List<CbtStudentAnswerViewModel> Answers { get; set; } = new();
    }

    public class CbtStudentAnswerViewModel
    {
        public int QuestionId { get; set; }
        public string? SelectedAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public decimal MarksAwarded { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
    }

    public class CbtAnalyticsViewModel
    {
        public int CbtTestId { get; set; }
        public string? TestTitle { get; set; }
        public int TotalAttempts { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public decimal AverageScore { get; set; }
        public decimal PassRate { get; set; }
        public decimal FailRate { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassMark { get; set; }
        public List<CbtAttemptViewModel> Attempts { get; set; } = new();
    }

    public class CbtStudentTestsViewModel
    {
        public List<CbtTestViewModel> ActiveTests { get; set; } = new();
        public List<CbtTestViewModel> CompletedTests { get; set; } = new();
    }
}
