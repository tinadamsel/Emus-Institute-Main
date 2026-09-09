using Core.Config;
using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public SuperAdminHelper(
            AppDbContext context,
            IGeneralConfiguration generalConfiguration,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _generalConfiguration = generalConfiguration;
            _emailService = emailService;
            _userManager = userManager;
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
                    IsUnderScholarship = departmentViewModel.IsUnderScholarship,
                    ScholarshipAmount = departmentViewModel.IsUnderScholarship ? departmentViewModel.ScholarshipAmount : null,
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
                    IsUnderScholarship = x.IsUnderScholarship,
                    ScholarshipAmount = x.ScholarshipAmount,
                    DateCreated = x.DateCreated,
                }).ToList();
            return departmentViewModel;
        }

        public int GetTotalApprovedStudents()
        {
            return _context.ApplicationUser
                .Include(x => x.Department)
                .Where(a => a.Id != null && a.StudentId != null && !a.IsAdmin && !a.Deactivated && a.IsStudent == true
                    && (a.Department == null || !a.Department.IsUnderScholarship))
                .Count();
        }
        public int GetTotalRegisteredStudents()
        {
            return _context.ApplicationUser
                .Include(x => x.Department)
                .Where(a => a.Id != null && a.StudentId != null && !a.IsAdmin && !a.Deactivated && a.IsStudent == false
                    && (a.Department == null || !a.Department.IsUnderScholarship))
                .Count();
        }
        public int GetTotalPaidStudents()
        {
            return _context.ApplicationUser.Where(a => a.Id != null && a.StudentId != null && !a.IsAdmin && !a.Deactivated && a.IsStudent == true && a.Paid).Count();
        }
        
        public int GetTotalDepartments()
        {
            return _context.Departments.Where(x => x.Id > 0 && x.Active).Count();
        }
        public int GetTotalSuspendedUsers()
        {
            return _context.Suspensions.Where(x => x.Id > 0 && x.IsSuspended).Count();
        }
        public int GetTotalStaff()
        {
            return _context.StaffDocuments.Where(a => a.Id > 0 && a.StaffStatus == StaffStatus.Approved && a.Active).Count();
        }
       
        public DepartmentViewModel GetDeptToEdit(int id)
        {
            var departmentToEdit = _context.Departments.Where(a => a.Id == id && a.Active && !a.Deleted)
                .Select(a => new DepartmentViewModel()
                {
                    Name = a.Name,
                    Id = a.Id,
                    Description = a.Description,
                    IsUnderScholarship = a.IsUnderScholarship,
                    ScholarshipAmount = a.ScholarshipAmount,
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
                    editDept.IsUnderScholarship = departmentViewModel.IsUnderScholarship;
                    editDept.ScholarshipAmount = departmentViewModel.IsUnderScholarship ? departmentViewModel.ScholarshipAmount : null;
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
            appUserViewModel = _context.ApplicationUser
                .Where(x => x.Id != null && x.StudentId != null && !x.IsStudent && !x.IsAdmin && !x.Deactivated
                    && (x.Department == null || !x.Department.IsUnderScholarship))
                .Include(x => x.Department)
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
            var result = new List<ApplicationUserViewModel>();
            var appUserViewModel = _context.ApplicationUser
                .Where(x => x.Id != null && x.StudentId != null && x.IsStudent && !x.IsAdmin && !x.Deactivated
                    && (x.Department == null || !x.Department.IsUnderScholarship))
                .Include(x => x.Department);
            if (appUserViewModel.Any())
            {
                result = appUserViewModel.Select(x => new ApplicationUserViewModel()
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
                    Password = x.Password,
                    State = x.State,
                    StudentId = x.StudentId,
                    CurrentSession = x.CurrentSession,
                    AcademicLevel = x.AcademicLevel,
                    Phonenumber = x.PhoneNumber,
                }).OrderByDescending(o => o.DateRegistered).ToList();
                foreach (var item in result)
                {
                    item.IsSuspended = GetUserSuspensionStatus(item.Id);
                }
                return result;
            }

            return result;
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

        public List<ApplicationUserViewModel> GetScholarshipStudents()
        {
            return GetScholarshipStudentsQuery(approved: false);
        }

        public List<ApplicationUserViewModel> GetApprovedScholarshipStudents()
        {
            return GetScholarshipStudentsQuery(approved: true);
        }

        private List<ApplicationUserViewModel> GetScholarshipStudentsQuery(bool approved)
        {
            var query = _context.ApplicationUser
                .Include(x => x.Department)
                .Where(x => x.Id != null && x.StudentId != null && !x.IsAdmin && !x.Deactivated
                    && x.Department != null && x.Department.IsUnderScholarship
                    && x.IsStudent == approved)
                .Select(x => new ApplicationUserViewModel()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    OtherName = x.OtherName,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    ScholarshipAmount = x.Department.ScholarshipAmount,
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
                    IsStudent = x.IsStudent,
                    Paid = x.Paid,
                    DateOfApproval = x.DateOfApproval,
                });

            return approved
                ? query.OrderByDescending(x => x.DateOfApproval).ToList()
                : query.OrderByDescending(x => x.DateRegistered).ToList();
        }

        public bool CheckIfScholarshipStudentIsApproved(string userId)
        {
            if (userId != null)
            {
                var approvedStudent = _context.ApplicationUser
                    .Include(x => x.Department)
                    .Where(x => x.Id == userId && x.IsStudent && !x.Deactivated
                        && x.Department != null && x.Department.IsUnderScholarship)
                    .FirstOrDefault();
                if (approvedStudent != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ApproveScholarshipStudent(string userId)
        {
            string toEmailBug = _generalConfiguration.DeveloperEmail;
            string subjectEmailBug = " Exception Message on Emu-Institute";
            try
            {
                if (userId != null)
                {
                    var approveStudent = _context.ApplicationUser
                        .Include(x => x.Department)
                        .Where(a => a.Id == userId && !a.Deactivated && !a.IsStudent && !a.IsAdmin
                            && a.Department != null && a.Department.IsUnderScholarship)
                        .FirstOrDefault();
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
                            string subject = "Scholarship Application Approved";
                            string message = "Hello " + "<b>" + approveStudent.FirstName + " " + approveStudent.LastName + ", </b>" +
                                "<br> Your scholarship application into Emus Institute has been approved. You can now continue to login." +
                                "<br><br> Once again, Congratulations!" +
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

        public bool DeclineScholarshipStudent(string userId)
        {
            string toEmailBug = _generalConfiguration.DeveloperEmail;
            string subjectEmailBug = "Exception Message on Emus-Institute";
            try
            {
                if (userId != null)
                {
                    var declineStudent = _context.ApplicationUser
                        .Include(x => x.Department)
                        .Where(a => a.Id == userId && !a.Deactivated && !a.IsStudent && !a.IsAdmin
                            && a.Department != null && a.Department.IsUnderScholarship)
                        .FirstOrDefault();
                    if (declineStudent != null)
                    {
                        declineStudent.Deactivated = true;
                        declineStudent.DateModified = DateTime.Now;

                        _context.Update(declineStudent);
                        _context.SaveChanges();

                        if (declineStudent.Email != null)
                        {
                            string toEmail = declineStudent.Email;
                            string subject = "Scholarship Application Declined";
                            string message = "Hello " + "<b>" + declineStudent.FirstName + " " + declineStudent.LastName + ", </b>" +
                                "<br> We regret to announce to you that your scholarship application into Emus Institute has been declined." +
                                " We thank you for your interest, but we can not move further with you." +
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
                    var approveStudent = _context.ApplicationUser
                        .Include(a => a.Department)
                        .Where(a => a.Id == userId && !a.Deactivated && !a.IsStudent && !a.IsAdmin
                            && (a.Department == null || !a.Department.IsUnderScholarship))
                        .FirstOrDefault();
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
                    var declineStudent = _context.ApplicationUser
                        .Include(a => a.Department)
                        .Where(a => a.Id == userId && !a.Deactivated && !a.IsStudent && !a.IsAdmin
                            && (a.Department == null || !a.Department.IsUnderScholarship))
                        .FirstOrDefault();
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
               DepartmentName = x.Users.Department.Name,
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
                                "You can now login with your credentials <br>" +
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
                        rejectApprove.IsApproved = false;
                        rejectApprove.Active = false;
                        if (rejectApprove.Users != null)
                        {
                            rejectApprove.Users.Deactivated = true;
                            rejectApprove.Users.DateModified = DateTime.Now;
                        }
                        else if (!string.IsNullOrEmpty(rejectApprove.UserId))
                        {
                            var staffUser = _context.ApplicationUser.FirstOrDefault(x => x.Id == rejectApprove.UserId);
                            if (staffUser != null)
                            {
                                staffUser.Deactivated = true;
                                staffUser.DateModified = DateTime.Now;
                                _context.Update(staffUser);
                            }
                        }
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

        public List<StaffDocumentationViewModel> GetApprovedStaff()
        {
            var result = new List<StaffDocumentationViewModel>();
            var approvedStaff = _context.StaffDocuments.Where(x => x.Id > 0 && x.Active && x.StaffStatus == StaffStatus.Approved).Include(x => x.Users);
            if (approvedStaff.Any())
            {
                result = approvedStaff.Select(x => new StaffDocumentationViewModel()
                {
                    Id = x.Id,
                    Name = x.Users.FirstName + " " + x.Users.LastName,
                    Email = x.Users.Email,
                    Password = x.Users.Password,
                    DateCreated = x.DateCreated,
                    ApplicationLetter = x.ApplicationLetter,
                    StaffPosition = x.StaffPosition,
                    UserId = x.UserId,
                    Identification = x.Identification,
                    Resume = x.Resume,
                    Active = x.Active,
                    DepartmentName = x.Users.Department != null ? x.Users.Department.Name : null,
                    DepartmentId = x.Users.DepartmentId,
                }).OrderByDescending(o => o.DateCreated).ToList();
                foreach (var item in result)
                {
                    item.IsSuspended = GetUserSuspensionStatus(item.UserId);
                }
                return result;
            }
            return result;
        }

        public bool GetUserSuspensionStatus(string userId)
        {
            if (userId != null)
            {
                var getStatus = _context.Suspensions.Where(x => x.UserId == userId).FirstOrDefault();
                if (getStatus != null)
                {
                    return getStatus.IsSuspended;
                }
            }
            return false;
        }

        public bool SuspendUser(string userId)
        {
            if (userId != null)
            {
                var getUserInitialSuspensionIfAny = _context.Suspensions.Where(x => x.UserId == userId && !x.IsSuspended && x.IsRemoved == true).FirstOrDefault();
                if (getUserInitialSuspensionIfAny != null)
                {
                    getUserInitialSuspensionIfAny.IsSuspended = true;
                    getUserInitialSuspensionIfAny.DateSuspended = DateTime.Now;
                    getUserInitialSuspensionIfAny.IsRemoved = false;

                    _context.Update(getUserInitialSuspensionIfAny);
                    _context.SaveChanges();
                }
                else
                {
                    var suspend = new Suspension()
                    {
                        IsSuspended = true,
                        IsRemoved = false,
                        UserId = userId,
                        DateSuspended = DateTime.Now,
                        SuspensionDuration = " N/A",
                        SuspensionReason = " Misconduct",
                    };
                    _context.Suspensions.Add(suspend);
                    _context.SaveChanges();
                }

                var getUser = _context.ApplicationUser.Where(x => x.Id == userId && !x.Deactivated).FirstOrDefault();
                if (getUser != null)
                {
                    string toEmail = getUser?.Email;
                    string subject = "Suspension Alert";
                    string message = "Hello " + "<b>" + getUser?.FirstName + " " + getUser?.LastName + ", </b>" +
                        "<br> you have been suspended from having access to EMUS Institute Platform. " +
                        "This simply means you cannot log in to the site currently. You will be notified when your suspension is lifted. " +
                        " <br> <br> Warm Regards, " +
                        " <br> <br> Emus Institute Team";

                    _emailService.SendEmail(toEmail, subject, message);
                    return true;
                }
            }
            return false;
        }

        public bool DeactivateUser(string userId)
        {
            if (userId != null)
            {
                var deactivateUser = _context.ApplicationUser.Where(x => x.Id == userId && !x.Deactivated).FirstOrDefault();
                if (deactivateUser != null)
                {
                    deactivateUser.Deactivated = true;
                    _context.SaveChanges();

                    if (deactivateUser?.Email != null)
                    {
                        string toEmail = deactivateUser?.Email;
                        string subject = "Deactivation Alert";
                        string message = "Hello " + "<b>" + deactivateUser?.FirstName + " " + deactivateUser?.LastName + ", </b>" +
                            "<br> you have been deactivated from using the EMUS Institute Platform. This simply means, you cannot log in to the site again " +
                            " <br> <br> We wish you well in your future endeavours <br> " +
                            " <br> <br> Warm Regards " +
                            "<br> <br> EMUS Institute Team ";

                        _emailService.SendEmail(toEmail, subject, message);
                        return true;
                    }
                }
            }
            return false;
        }

        public List<SuspensionViewModel> GetSuspendedUsers()
        {
            var getSuspendedUsers = new List<SuspensionViewModel>();
            getSuspendedUsers = _context.Suspensions.Where(x => x.Id > 0 && x.IsSuspended && x.IsRemoved == false).Include(x => x.Users)
           .Select(x => new SuspensionViewModel()
           {
               Id = x.Id,
               Name = x.Users.FirstName + " " + x.Users.LastName,
               IsSuspended = x.IsSuspended,
               IsRemoved = x.IsRemoved,
               UserId = x.UserId,
               DateRemoved = x.DateRemoved,
               DateSuspended = x.DateSuspended,
               ValidUntilDate = x.ValidUntilDate,
               SuspensionReason = x.SuspensionReason,
               SuspensionDuration = x.SuspensionDuration,

           }).ToList();
            return getSuspendedUsers;
        }

        public bool RemoveSuspension(int id)
        {
            if (id > 0)
            {
                var removeSuspension = _context.Suspensions.Where(x => x.Id == id && x.IsSuspended).FirstOrDefault();
                if (removeSuspension != null)
                {
                    removeSuspension.IsSuspended = false;
                    removeSuspension.IsRemoved = true;
                    removeSuspension.DateRemoved = DateTime.Now;

                    _context.Update(removeSuspension);
                    _context.SaveChanges();

                    var getUser = _context.ApplicationUser.Where(x => x.Id == removeSuspension.UserId && !x.Deactivated).FirstOrDefault();
                    if (getUser != null)
                    {
                        string toEmail = getUser?.Email;
                        string subject = "Suspension Lifting Alert";
                        string message = "Hello " + "<b>" + getUser?.FirstName + " " + getUser?.LastName + ", </b>" +
                            "<br> your suspension has been lifted. You can now log into the EMUS Institute Platform with your login details " +
                            " <br> <br> Warm Regards, " +
                            "<br> <br> Emus Institute Team";

                        _emailService.SendEmail(toEmail, subject, message);
                        return true;
                    }
                    return true;
                }
            }
            return false;
        }

        public async Task<(bool Success, string Message)> ReassignStaffRoleAsync(int staffDocumentId,int newStaffPosition, int? departmentId)
        {
            if (staffDocumentId <= 0)
            {
                return (false, "Invalid staff record.");
            }

            if (!Enum.IsDefined(typeof(StaffPosition), newStaffPosition))
            {
                return (false, "Invalid staff role selected.");
            }

            var newPosition = (StaffPosition)newStaffPosition;
            var staffDocument = await _context.StaffDocuments
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x =>
                    x.Id == staffDocumentId &&
                    x.Active &&
                    x.StaffStatus == StaffStatus.Approved)
                .ConfigureAwait(false);

            if (staffDocument?.Users == null)
            {
                return (false, "Staff record not found.");
            }

            if (staffDocument.Users.IsStudent)
            {
                return (false, "Only staff accounts can be reassigned.");
            }

            if (newPosition == StaffPosition.AcademicStaff)
            {
                if (!departmentId.HasValue || departmentId.Value <= 0)
                {
                    return (false, "Please select a department for Academic Staff.");
                }

                var departmentExists = await _context.Departments
                    .AnyAsync(x => x.Id == departmentId.Value && x.Active && !x.Deleted)
                    .ConfigureAwait(false);

                if (!departmentExists)
                {
                    return (false, "Selected department was not found.");
                }
            }

            var user = await _userManager.FindByIdAsync(staffDocument.UserId!).ConfigureAwait(false);
            if (user == null)
            {
                return (false, "Staff user account not found.");
            }

            var newRoleName = GetRoleNameForStaffPosition(newPosition);
            if (string.IsNullOrWhiteSpace(newRoleName))
            {
                return (false, "Unable to map the selected role.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            if (currentRoles.Any(role => role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Super Admin accounts cannot be reassigned from this page.");
            }

            if (currentRoles.Count == 1 &&
                currentRoles[0].Equals(newRoleName, StringComparison.OrdinalIgnoreCase) &&
                staffDocument.StaffPosition == newPosition &&
                user.DepartmentId == (newPosition == StaffPosition.AcademicStaff ? departmentId : null))
            {
                return (false, "Staff member already has this role.");
            }

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles).ConfigureAwait(false);
            if (!removeResult.Succeeded)
            {
                return (false, "Unable to remove the staff member's current role.");
            }

            var addResult = await _userManager.AddToRoleAsync(user, newRoleName).ConfigureAwait(false);
            if (!addResult.Succeeded)
            {
                if (currentRoles.Count > 0)
                {
                    await _userManager.AddToRolesAsync(user, currentRoles).ConfigureAwait(false);
                }

                return (false, "Unable to assign the new role.");
            }

            user.StaffType = newPosition == StaffPosition.AcademicStaff
                ? StaffType.AcademicStaff
                : StaffType.NonAcademicStaff;
            user.DepartmentId = newPosition == StaffPosition.AcademicStaff ? departmentId : null;
            user.DateModified = DateTime.Now;
            user.IsAdmin = true;
            user.IsStudent = false;

            staffDocument.StaffPosition = newPosition;

            _context.StaffDocuments.Update(staffDocument);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            var updateUserResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
            if (!updateUserResult.Succeeded)
            {
                return (false, "Role was updated but staff profile could not be saved.");
            }

            return (true, "Staff role reassigned successfully.");
        }

        private static string? GetRoleNameForStaffPosition(StaffPosition position)
        {
            return position switch
            {
                StaffPosition.AcademicStaff => "AcademicStaff",
                StaffPosition.HumanResource => "HumanResourceOfficer",
                StaffPosition.ViceChancellorAcademics => "AcademicStaff",
                StaffPosition.AdmissionOfficer => "AdmissionOfficer",
                StaffPosition.LibrarianOfficer => "LibrarianOfficer",
                StaffPosition.AccountOfficer => "AccountOfficer",
                StaffPosition.MarketingOfficer => "MarketingOfficer",
                StaffPosition.ExamsOfficer => "AcademicStaff",
                StaffPosition.BusinessDevOfficer => "MarketingOfficer",
                StaffPosition.PublicRelOfficer => "MarketingOfficer",
                StaffPosition.ViceChancellorStudentAffairs => "HumanResourceOfficer",
                _ => null
            };
        }

        //public int GetTotalAcademicStaff()
        //{
        //    return _context.ApplicationUser.Where(a => a.Id != null && !a.Deactivated && a.IsStudent == false && a.StaffType == StaffType.AcademicStaff).Count();
        //}
        //public int GetTotalNonAcademicStaff()
        //{
        //    return _context.ApplicationUser.Where(a => a.Id != null && !a.Deactivated && a.IsStudent == false && a.StaffType == StaffType.NonAcademicStaff).Count();
        //}


        //Add the method: check if staff is approved









    }
}
