using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using static Core.DB.ECollegeEnums;

namespace Core.ViewModels
{
    public class TextbookViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Description { get; set; }
        public string TextbookCode { get; set; }
        public decimal Price { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department? Departments { get; set; }
        public TextbookEnum TextbookForEachSession { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? Users { get; set; }
        public string AddedBy { get; set; }
        public bool? IsApproved { get; set; }
       
    }
}
