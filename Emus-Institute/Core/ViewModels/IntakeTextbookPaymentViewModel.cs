namespace Core.ViewModels
{
    public class IntakeTextbookPaymentViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? StudentId { get; set; }
        public string? DepartmentName { get; set; }
        public string DataPhoneNumber { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? PaymentReference { get; set; }
    }
}
