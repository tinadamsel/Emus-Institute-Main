namespace Core.ViewModels
{
    public class AssignmentViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ValidUntilDate { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? StaffName { get; set; }
        public string? FilePath { get; set; }
        public decimal TotalMarks { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Active { get; set; }
        public int SubmissionCount { get; set; }
        public int GradedCount { get; set; }
        public bool IsOverdue { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public bool HasSubmitted { get; set; }
        public bool IsGraded { get; set; }
        public bool CanSubmit { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal? Score { get; set; }
        public string? Feedback { get; set; }
        public string? SubmissionFilePath { get; set; }
        public string? SubmissionFileName { get; set; }
        public string? SubmissionComment { get; set; }
        public int? SubmissionId { get; set; }
        public string DueDateDisplay => ValidUntilDate.ToString("dddd, MMM d, yyyy · h:mm tt");
    }
}
