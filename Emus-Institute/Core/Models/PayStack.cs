using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PayStack
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public DateTime? Transaction_date { get; set; }
        public string? Reference { get; set; }
        public string? Domain { get; set; }
        public string? Gateway_response { get; set; }
        public string? Message { get; set; }
        public string? Channel { get; set; }
        public string? Ip_Address { get; set; }
        public string? Fees { get; set; }
        public string? Last4 { get; set; }
        public string? Exp_month { get; set; }
        public string? Exp_year { get; set; }
        public string? Card_type { get; set; }
        public string? Bank { get; set; }
        public string? Country_code { get; set; }
        public string? Brand { get; set; }
        public bool? Reusable { get; set; }
        public string? Signature { get; set; }
        public string? Authorization_url { get; set; }
        public string? Access_code { get; set; }
    }
}

