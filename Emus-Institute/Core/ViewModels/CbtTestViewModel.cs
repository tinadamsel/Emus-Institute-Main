namespace Core.ViewModels
{
    public class CbtTestViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsPublicAssessment { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? StaffName { get; set; }
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
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public int QuestionCount { get; set; }
        public bool CanEdit { get; set; }
        public bool HasStarted { get; set; }
        public bool IsActiveNow { get; set; }
        public bool IsUpcoming { get; set; }
        public bool IsEnded { get; set; }
        public bool CanTake { get; set; }
        public bool HasCompletedAttempt { get; set; }
        public string? StatusLabel { get; set; }
        public string? ScheduleDisplay { get; set; }
        public int? LatestAttemptId { get; set; }
        public decimal? LatestScore { get; set; }
        public bool? LatestPassed { get; set; }
    }
}
