using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Core.DB.ECollegeEnums;

namespace Core.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string? OtherName { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phonenumber { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public DateTime DateRegistered { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public AcademicLevel? AcademicLevel { get; set; }
        public CurrentSession? CurrentSession { get; set; }
        public StaffType? StaffType { get; set; }
        public bool Deactivated { get; set; }
        public bool IsStudent { get; set; }
        public bool Paid { get; set; }
        public bool IsAdmin { get; set; }
        public string StudentId { get; set; }
        public DateTime? DateOfApproval { get; set; }
        public DateTime? DateModified { get; set; }
        public int TotalApprovedStudents { get; set; }
        public int TotalRegisteredStudents { get; set; }
        public int TotalPaidStudents { get; set; }
        public int TotalStaff { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalAcademicStaff { get; set; }
        public int TotalNonAcademicStaff { get; set; }
        public int TotalDepartmentTextbooks { get; set; }
        public int TotalActiveAnnouncements { get; set; }
        public int TotalComplains { get; set; }
        public string? RefLink { get; set; }
    }
}
