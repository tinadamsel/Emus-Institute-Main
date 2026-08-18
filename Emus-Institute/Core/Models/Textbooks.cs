using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Core.DB.ECollegeEnums;

namespace Core.Models
{
    public class Textbooks : Basemodel
    {
        public string? Description { get; set; }
        public string TextbookCode { get; set; }
        public decimal Price { get; set; }
        public int? DepartmentId { get; set; }
        [Display(Name = "Department")]
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }
        public TextbookEnum TextbookForEachSession { get; set; }
        public string? UserId { get; set; }
        [Display(Name = "User")]
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Users { get; set; }
        public string AddedBy { get; set; }
        public bool? IsApproved { get; set; }
        public TextbookApprovalStatus Status { get; set; } = TextbookApprovalStatus.Pending;
        public string? FilePath { get; set; }
        public string? CoverImagePath { get; set; }
    }
}
