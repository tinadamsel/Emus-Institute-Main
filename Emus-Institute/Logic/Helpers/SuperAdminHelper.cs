using Core.Config;
using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class SuperAdminHelper : ISuperAdminHelper
    {
        private readonly AppDbContext _context;
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly IEmailService _emailService;

        public SuperAdminHelper(AppDbContext context, IGeneralConfiguration generalConfiguration, IEmailService emailService)
        {
            _context = context;
            _generalConfiguration = generalConfiguration;
            _emailService = emailService;
        }

        public bool CheckExistingDeptName(string name)
        {
            if (name != null)
            {
                var checkDeptName = _context.Departments.Where(x => x.Name == name && x.Active && !x.Deleted).FirstOrDefault();
                if (checkDeptName != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CreateDepartment(DepartmentViewModel departmentViewModel)
        {
            if (departmentViewModel != null)
            {
                var department = new Department()
                {
                    Name = departmentViewModel.Name,
                    Description = departmentViewModel.Description,
                    DateCreated = DateTime.Now,
                    Active = true,
                    Deleted = false,
                };
                _context.Departments.Add(department);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<DepartmentViewModel> GetDepartments()
        {
            var departmentViewModel = new List<DepartmentViewModel>();
            departmentViewModel = _context.Departments.Where(x => x.Id > 0 && x.Active && !x.Deleted)
                .Select(x => new DepartmentViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    DateCreated = x.DateCreated,
                }).ToList();
            return departmentViewModel;
        }

        public int GetTotalApprovedStudents()
        {
            return _context.ApplicationUser.Where(a => a.Id != null && a.StudentId != null && !a.IsAdmin && !a.Deactivated && a.IsStudent == true).Count();
        }
        public int GetTotalRegisteredStudents()
        {
            return _context.ApplicationUser.Where(a => a.Id != null && a.StudentId != null && !a.IsAdmin && !a.Deactivated && a.IsStudent == false).Count();
        }
        public int GetTotalPaidStudents()
        {
            return _context.ApplicationUser.Where(a => a.Id != null && a.StudentId != null && !a.IsAdmin && !a.Deactivated && a.IsStudent == true && a.Paid).Count();
        }
        
        public int GetTotalDepartments()
        {
            return _context.Departments.Where(x => x.Id > 0 && x.Active).Count();
        }
        public int GetTotalStaff()
        {
            return _context.StaffDocuments.Where(a => a.Id > 0 && a.Active).Count();
        }
       
        public DepartmentViewModel GetDeptToEdit(int id)
        {
            var departmentToEdit = _context.Departments.Where(a => a.Id == id && a.Active && !a.Deleted)
                .Select(a => new DepartmentViewModel()
                {
                    Name = a.Name,
                    Id = a.Id,
                    Description = a.Description,
                    DateCreated = a.DateCreated,
                }).FirstOrDefault();
                if (departmentToEdit != null)
                {
                    return departmentToEdit;
                }
                return null;
        }
        
        public bool SaveEditedDept(DepartmentViewModel departmentViewModel)
        {
            if (departmentViewModel != null) 
            { 
                var editDept = _context.Departments.Where(x => x.Id == departmentViewModel.Id && x.Active && !x.Deleted).FirstOrDefault();
                if (editDept != null)
                {
                    editDept.Name = departmentViewModel.Name;
                    editDept.Description = departmentViewModel.Description;
                }
                _context.Update(editDept);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        
        public bool DeleteDept(int id)
        {
            var deptToDelete = _context.Departments.Where(a => a.Id == id && a.Active && !a.Deleted).FirstOrDefault();
            if (deptToDelete != null)
            {
                deptToDelete.Active = false;
                deptToDelete.Deleted = true;
                _context.Update(deptToDelete);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<ApplicationUserViewModel> GetAllRegisteredStudents()
        {
            var appUserViewModel = new List<ApplicationUserViewModel>();
            appUserViewModel = _context.ApplicationUser.Where(x => x.Id != null && x.StudentId != null && !x.IsStudent && !x.IsAdmin && !x.Deactivated).Include(x => x.Department)
                .Select(x => new ApplicationUserViewModel()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName, 
                    OtherName = x.OtherName,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    FullName = x.FirstName + " " + x.LastName,
                    DateRegistered = x.DateRegistered,
                    DOB = x.DOB,
                    Address = x.Address,
                    Country = x.Country,
                    Email = x.Email,
                    State = x.State,
                    StudentId = x.StudentId,
                    CurrentSession = x.CurrentSession,
                    AcademicLevel = x.AcademicLevel,
                    Phonenumber = x.PhoneNumber,
                }).ToList();
            return appUserViewModel;
        }
        public List<ApplicationUserViewModel> GetAllApprovedStudents()
        {
            var appUserViewModel = new List<ApplicationUserViewModel>();
            appUserViewModel = _context.ApplicationUser.Where(x => x.Id != null && x.StudentId != null && x.IsStudent && !x.IsAdmin && !x.Deactivated).Include(x => x.Department)
                .Select(x => new ApplicationUserViewModel()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    OtherName = x.OtherName,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    FullName = x.FirstName + " " + x.LastName,
                    DateRegistered = x.DateRegistered,
                    DOB = x.DOB,
                    Address = x.Address,
                    Country = x.Country,
                    Email = x.Email,
                    State = x.State,
                    StudentId = x.StudentId,
                    CurrentSession = x.CurrentSession,
                    AcademicLevel = x.AcademicLevel,
                    Phonenumber = x.PhoneNumber,
                }).ToList();
            return appUserViewModel;
        }

        public List<ApplicationUserViewModel> GetAllPaidStudents()
        {
            var appUserViewModel = new List<ApplicationUserViewModel>();
            appUserViewModel = _context.ApplicationUser.Where(x => x.Id != null && x.StudentId != null && x.IsStudent && !x.IsAdmin && !x.Deactivated && x.Paid).Include(x => x.Department)
                .Select(x => new ApplicationUserViewModel()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    OtherName = x.OtherName,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    FullName = x.FirstName + " " + x.LastName,
                    DateRegistered = x.DateRegistered,
                    DOB = x.DOB,
                    Address = x.Address,
                    Country = x.Country,
                    Email = x.Email,
                    State = x.State,
                    StudentId = x.StudentId,
                    CurrentSession = x.CurrentSession,
                    AcademicLevel = x.AcademicLevel,
                    Phonenumber = x.PhoneNumber,
                    Paid = x.Paid,
                }).ToList();
            return appUserViewModel;
        }

        public bool CheckIfStudentIsApproved(string userId)
        {
            if (userId != null)
            {
                var checkIfStudentIsAproved = _context.ApplicationUser.Where(x => x.Id == userId && x.IsStudent && !x.Deactivated).FirstOrDefault();
                if (checkIfStudentIsAproved != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ApproveStudent(string userId)
        {
            string toEmailBug = _generalConfiguration.DeveloperEmail;
            string subjectEmailBug = " Exception Message on Emu-Institute";
            try
            {
                if (userId != null)
                {
                    var approveStudent = _context.ApplicationUser.Where(a => a.Id == userId && !a.Deactivated && !a.IsStudent && !a.IsAdmin).FirstOrDefault();
                    if (approveStudent != null)
                    {
                        approveStudent.IsStudent = true;
                        approveStudent.DateModified = DateTime.Now;
                        approveStudent.DateOfApproval = DateTime.Now;
                        _context.Update(approveStudent);
                        _context.SaveChanges();

                        if (approveStudent.Email != null)
                        {
                            string toEmail = approveStudent.Email;
                            string subject = "Hooray!!!, Student Application Approved ";
                            string message = "Hello " + "<b>" + approveStudent.FirstName + " " + approveStudent.LastName + ", </b>" + 
                                "<br> your application into Emus Institute has been approved. You can now continue to login  <br> <br>" +
                              " Once again, Congratulations !!! " +
                              "<br> Emus Institute Team";

                            _emailService.SendEmail(toEmail, subject, message);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                string message = "Exception " + ex.Message + " and inner exception:" + ex.InnerException.Message + "  Occured at " + DateTime.Now;
                _emailService.SendEmail(toEmailBug, subjectEmailBug, message);
                throw;
            }
        }

        public bool DeclineStudent(string userId)
        {
            string toEmailBug = _generalConfiguration.DeveloperEmail;
            string subjectEmailBug = "Exception Message on Emus-Institute";
            try
            {
                if (userId != null)
                {
                    var declineStudent = _context.ApplicationUser.Where(a => a.Id == userId && !a.Deactivated && !a.IsStudent && !a.IsAdmin).FirstOrDefault();
                    if (declineStudent != null)
                    {
                        declineStudent.Deactivated = true;
                        declineStudent.DateModified = DateTime.Now;

                        _context.Update(declineStudent);
                        _context.SaveChanges();

                        if (declineStudent.Email != null)
                        {
                            string toEmail = declineStudent.Email;
                            string subject = "Sorry, Student Application Declined";
                            string message = "Hello " + "<b>" + declineStudent.FirstName + " " + declineStudent.LastName + ", </b>" +
                                "<br> We regret to announce to you that your application into Emus Institute has been declined. " + 
                                " We thank you for your interest, but we can not move further with you. " +
                                " <br> We wish you well in your future endeavours <br> <br> " +
                                " Warm Regards <br> " +
                              "Emus Institute Team";

                            _emailService.SendEmail(toEmail, subject, message);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                string message = "Exception " + ex.Message + " and inner exception:" + ex.InnerException.Message + "  Occured at " + DateTime.Now;
                _emailService.SendEmail(toEmailBug, subjectEmailBug, message);
                throw;
            }
        }

        public List<StaffDocumentationViewModel> GetPendingApplications()
        {
            var getApplications = new List<StaffDocumentationViewModel>();
            getApplications = _context.StaffDocuments.Where(x => x.Id > 0 && x.Active && x.StaffStatus == StaffStatus.Pending).Include(x => x.Users)
           .Select(x => new StaffDocumentationViewModel()
           {
               Id = x.Id,
               Name = x.Users.FirstName + " " + x.Users.LastName,
               Email = x.Users.Email,
               DateCreated = x.DateCreated,
               ApplicationLetter = x.ApplicationLetter,
               StaffPosition = x.StaffPosition,
               UserId = x.UserId,
               Identification = x.Identification,
               Resume = x.Resume,
               Active = x.Active,
           }).ToList();
            return getApplications;
        }
        public bool ApproveApplication(int id)
        {
            string toEmailBug = _generalConfiguration.DeveloperEmail;
            string subjectEmailBug = " Exception Message on Ecollege";
            try
            {
                if (id > 0)
                {
                    var appApprove = _context.StaffDocuments.Where(x => x.Id == id && x.StaffStatus == StaffStatus.Pending).Include(x => x.Users).FirstOrDefault();
                    if (appApprove != null)
                    {
                        appApprove.IsApproved = true;
                        appApprove.DateOfApproval = DateTime.Now;
                        appApprove.StaffStatus = StaffStatus.Approved;
                        _context.Update(appApprove);
                        _context.SaveChanges();

                        if (appApprove?.Users?.Email != null)
                        {
                            string toEmail = appApprove?.Users?.Email;
                            string subject = "Hooray!!!, Application Approved ";
                            string message = "Hello " + "<b>" + appApprove?.Users?.FirstName + " " + appApprove?.Users?.LastName + ", </b>" + 
                                "<br> your application for the post of " + appApprove?.StaffPosition + " on our platform has been approved.  " +
                                "You can now login with the following credentials: <br>"  +
                                " <b> Email: " + appApprove?.Users?.Email + ", Password: " + appApprove?.Users?.Password + " </b> " +
                                "<br> <br> We are happy to have you and we look forward to having a nice working relationship with you. " +
                                "<br> <br>" +
                              " Once again, Congratulations !!! " +
                              "<br> Emus Institute Team";

                            _emailService.SendEmail(toEmail, subject, message);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                string message = "Exception " + ex.Message + " and inner exception:" + ex.InnerException.Message + "  Occured at " + DateTime.Now;
                _emailService.SendEmail(toEmailBug, subjectEmailBug, message);
                throw;
            }
        }

        public bool RejectApplication(int id)
        {
            string toEmailBug = _generalConfiguration.DeveloperEmail;
            string subjectEmailBug = "Exception Message on Ecollege";
            try
            {
                if (id > 0)
                {
                    var rejectApprove = _context.StaffDocuments.Where(x => x.Id == id && x.StaffStatus == StaffStatus.Pending).Include(x => x.Users).FirstOrDefault();
                    if (rejectApprove != null)
                    {
                        rejectApprove.StaffStatus = StaffStatus.Rejected;
                        _context.Update(rejectApprove);
                        _context.SaveChanges();

                        if (rejectApprove?.Users?.Email != null)
                        {
                            string toEmail = rejectApprove?.Users?.Email;
                            string subject = "Sorry, Application Declined ";
                            string message = "Hello " + "<b>" + rejectApprove?.Users?.FirstName + "" + rejectApprove?.Users?.LastName + ", </b>" + "<br> your application for the post of " + rejectApprove?.StaffPosition + " on our platform has been declined. We thank you for your interest, but we can not move further with you. Keep on visiting our platform for other available positions. " + " <br> <br> We wish you well in your future endeavours <br> " +
                              "HR, Ecollege Team";

                            _emailService.SendEmail(toEmail, subject, message);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                string message = "Exception " + ex.Message + " and inner exception:" + ex.InnerException.Message + "  Occured at " + DateTime.Now;
                _emailService.SendEmail(toEmailBug, subjectEmailBug, message);
                throw;
            }
        }

        //public int GetTotalAcademicStaff()
        //{
        //    return _context.ApplicationUser.Where(a => a.Id != null && !a.Deactivated && a.IsStudent == false && a.StaffType == StaffType.AcademicStaff).Count();
        //}
        //public int GetTotalNonAcademicStaff()
        //{
        //    return _context.ApplicationUser.Where(a => a.Id != null && !a.Deactivated && a.IsStudent == false && a.StaffType == StaffType.NonAcademicStaff).Count();
        //}









    }
}
