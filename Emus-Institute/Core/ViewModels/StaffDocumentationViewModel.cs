using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.ECollegeEnums;
using System.Xml.Linq;

namespace Core.ViewModels
{
    public class StaffDocumentationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public string ApplicationLetter { get; set; }
        public string Resume { get; set; }
        public string Identification { get; set; }
        public StaffStatus StaffStatus { get; set; }
        public StaffPosition StaffPosition { get; set; }
        public DateTime? DateOfApproval { get; set; }
        public bool? IsApproved { get; set; }
        public string? UserId { get; set; }
        public string? CourseName { get; set; }
        public string? IDPix { get; set; }
        public virtual ApplicationUser Users { get; set; }
        public int? CourseId { get; set; }
        public virtual Department? Courses { get; set; }
        public List<StaffDocumentationViewModel>? StaffDetails { get; set; } 
    }
}
