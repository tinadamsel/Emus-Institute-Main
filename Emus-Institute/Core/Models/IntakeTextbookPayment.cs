using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class IntakeTextbookPayment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        public string DataPhoneNumber { get; set; }
        public Guid? PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; }
        public bool IsPaid { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? PaidAt { get; set; }
        public decimal Amount { get; set; }
    }
}
