using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Core.DB.ECollegeEnums;

namespace Core.Models
{
	public class PaymentForm
	{
        public Guid Id { get; set; }

        [Display(Name = "Details")]
        public string Details { get; set; }

        [Display(Name = "Amount Paid ")]
        public decimal Amount { get; set; }

        [Display(Name = "Date of Payment ")]
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        [Display(Name = "User")]
        [ForeignKey("UserId")]
        public virtual ApplicationUser Users { get; set; }

        [Display(Name = "Reg Fee Payment Status")]
        public PaymentStatus Status { get; set; }

        [Display(Name = "Approved By")]
        public string? ApprovedById { get; set; }
        public string PaymentProofSlip { get; set; }

        [Display(Name = "Date of Approval")]
        public DateTime? StatuseChangeDate { get; set; }

        [Display(Name = "Bank Account Paid To")]
        public int? AccountId { get; set; }
        [Display(Name = "School Account")]
        [ForeignKey("School AccountId")]
        public virtual CommonDropdowns SchoolAccount { get; set; }

        [Display(Name = "Account Name  paid from")]
        public string AccountNamePaidFrom { get; set; }

        [Display(Name = "Bank Name paid from")]
        public string BankNamePaidFrom { get; set; }
        [Display(Name = "Account Number  paid from")]
        public string AccountNumberPaidFrom { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [Display(Name = "Mode Of Payment")]
        public string PaymentMethod { get; set; }
        [Display(Name = "Textbook Fee Paid")]
        public bool? IsTextbookFeePaid { get; set; }
    }
}
