namespace Core.ViewModels
{
    public class AssignmentSubmissionViewModel
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string AssignmentName { get; set; } = string.Empty;
        public decimal TotalMarks { get; set; }
        public string StudentUserId { get; set; } = string.Empty;
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string? StudentIdNumber { get; set; }
        public string? FilePath { get; set; }
        public string? OriginalFileName { get; set; }
        public string? Comment { get; set; }
        public DateTime SubmittedAt { get; set; }
        public decimal? Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime? GradedAt { get; set; }
        public bool IsGraded { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
    }

    public class AssignmentSubmissionsPageViewModel
    {
        public AssignmentViewModel Assignment { get; set; } = new();
        public List<AssignmentSubmissionViewModel> Submissions { get; set; } = new();
    }
}
