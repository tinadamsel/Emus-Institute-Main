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
    public class StaffDocumentation : Basemodel
    {
        public string ApplicationLetter { get; set; }
        public string Resume { get; set; }
        public string Identification { get; set; }
        public StaffStatus StaffStatus { get; set; }
        public StaffPosition StaffPosition { get; set; }
        public DateTime? DateOfApproval { get; set; }
        public bool? IsApproved { get; set; }
        public string? ApprovedbyId { get; set; }
        public string? UserId { get; set; }
        [Display(Name = "User")]
        [ForeignKey("UserId")]
        public virtual ApplicationUser Users { get; set; }
        public string? RefLink { get; set; }
        public decimal? AmountPerStudent { get; set; }

    }
}
