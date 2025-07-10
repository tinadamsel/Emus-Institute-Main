using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.ECollegeEnums;

namespace Core.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }
        public decimal? Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? UserId { get; set; }
        [Display(Name = "User")]
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        public PaymentStatus? Status { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Reference { get; set; }
        public bool? IsTextbookFeePaid { get; set; }
        [Display(Name = "Approved By")]
        public string? ApprovedById { get; set; }
        [Display(Name = "Details")]
        public string? Details { get; set; }
    }
}
