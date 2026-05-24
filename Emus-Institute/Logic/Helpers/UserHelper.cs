using Core.Config;
using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Macs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly IEmailService _emailService;

        public UserHelper(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            //_generalConfiguration = generalConfiguration;
            _emailService = emailService;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.Users.Where(s => s.Email == email)?.FirstOrDefaultAsync();
        }
        public ApplicationUser FindByUserName(string username)
        {
            return _userManager.Users.Where(s => s.UserName == username).FirstOrDefault();
        }
        public async Task<ApplicationUser> FindByUserNameAsync(string username)
        {
            return await _userManager.Users.Where(s => s.UserName == username).FirstOrDefaultAsync();
        }

        //public int GetAllUser()
        //{
        //    return _userManager.Users.Where(x => !x.Deactivated).Count();
        //}
        public string GetUserRole(string userId)
        {
            if (userId != null)
            {
                var userRole = _context.UserRoles.Where(x => x.UserId == userId).FirstOrDefault();
                if (userRole != null)
                {
                    var roles = _context.Roles.Where(x => x.Id == userRole.RoleId).FirstOrDefault();
                    if (roles != null)
                    {
                        return roles.Name;
                    }
                }
            }
            return null;
        }
        public string GetUserId(string username)
        {
            return _userManager.Users.Where(s => s.UserName == username)?.FirstOrDefaultAsync().Result.Id?.ToString();
        }
        public ApplicationUser FindById(string Id)
        {
            return _userManager.Users.Where(s => s.Id == Id).FirstOrDefault();
        }
        public string GetCurrentUserId(string username)
        {
            try
            {
                if (username != null)
                {
                    return _userManager.Users.Where(s => s.UserName == username)?.FirstOrDefault()?.Id?.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public string GenerateStudentID()
        {
            var dateConvert = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
            int sequence = 10;
            var initial = "EmusINS";
            var studentId = initial + dateConvert + sequence;
            return studentId;
        }

        public bool CheckForUserName(string userName)
        {
            if (userName != null)
            {
                var checkUserName = _context.ApplicationUser.Where(x => x.UserName == userName && !x.Deactivated).FirstOrDefault();
                if (checkUserName != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckIfUserIsSuspended(string email)
        {
            if (email != null)
            {
                var getUser = _context.ApplicationUser.Where(x => x.Email == email && x.Id != null && !x.Deactivated).FirstOrDefault();
                if (getUser != null)
                {
                    var checkForSuspension = _context.Suspensions.Where(x => x.UserId == getUser.Id && x.IsSuspended).FirstOrDefault();
                    if (checkForSuspension != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckIfStaffIsApproved(string email)
        {
            if (email != null)
            {
                var getUser = _context.ApplicationUser.Where(x => x.Email == email && x.Id != null && x.IsStudent == false && x.IsAdmin == true).FirstOrDefault();
                if (getUser == null)
                {
                    return true;
                }
                else
                {
                    var documentation = _context.StaffDocuments.Where(x => x.UserId == getUser.Id && (bool)x.IsApproved).FirstOrDefault();
                    if (documentation != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> RegisterStudent(ApplicationUserViewModel userDetails, string linkToClick, string refLink)
        {
            try
            {
                if (userDetails != null)
                {
                    var studentId = GenerateStudentID();

                    var user = new ApplicationUser();
                    user.UserName = userDetails.Email;
                    user.Email = userDetails.Email;
                    user.FirstName = userDetails.FirstName;
                    user.LastName = userDetails.LastName;
                    user.PhoneNumber = userDetails.Phonenumber;
                    user.DateRegistered = DateTime.Now;
                    user.Deactivated = false;
                    user.Address = userDetails.Address;
                    user.Country = userDetails.Country;
                    user.State = userDetails.State;
                    user.DOB = userDetails.DOB;
                    user.OtherName = userDetails.OtherName;
                    user.IsStudent = false;
                    user.IsAdmin = false;
                    user.Paid = false;
                    user.AcademicLevel = AcademicLevel.Tertiary;
                    user.CurrentSession = CurrentSession.YearOne;
                    user.DepartmentId = userDetails.DepartmentId;
                    user.StudentId = studentId;
                    user.RefLink = refLink != null ? refLink : null;
                    var createUser = await _userManager.CreateAsync(user, userDetails.Password).ConfigureAwait(false);
                    if (createUser.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "TertiaryStudent").ConfigureAwait(false);
                        var url = linkToClick + user.Id;
                        if (user.Email != null)
                        {
                            string toEmail = user.Email;
                            string subject = "Student Application Submission";
                            string message = "Hello," + "<b>" + user?.FirstName + " " + user?.LastName + ",</b> " +
                                "<br> Your application into Emus Institute was successful and your Student ID is " + "<b>" + user?.StudentId + ".</b>" +
                                "<br/> <br/> However, you need to complete the evaluation form to be fully admitted into the school." +
                                ". <br/> <br/> Please, click on the button below to log into the evaluation page and make the necessary payment of &euro;200 " +
                                "(which covers application, transcript review and certificate evaluation)" +
                                "<br>" + "<a style:'border:2px; text-decoration: none;' href='" + url + "' target='_blank'>" + "<button style='color:white; background-color:#06BBCC; padding:12px; border:1px solid #06BBCC;'> Evaluate Credentials </button>" + "</a>" +
                                "<br/> <br/> Thank you  " +
                                "<br/> <br/> Emus Institute Team";
                            _emailService.SendEmail(toEmail, subject, message);

                            var adminEmail = "nwachukwuarinze00@gmail.com";
                            string adminSubject = "Student Application Submission";
                            string adminMessage = "Hello SuperAdmin, <br> A student application has been submitted by " +
                                "<b>" + user?.FirstName + " " + user?.LastName + "</b> on " + user.DateRegistered.ToString() + ". " +
                                "<br> Student ID: <b>" + user?.StudentId + "</b>. " +
                                "<br> Please, endeavor to review the registration and make the necessary move. " +
                                "<br> Thank you!!!";
                            _emailService.SendEmail(adminEmail, adminSubject, adminMessage);

                            return true;
                        }

                        //if (user.Email != null)
                        //{
                        //    string toEmail = user.Email;
                        //    string subject = "Student Application Submission";
                        //    string message = "Hello," + "<b>" + user?.FirstName + " " + user?.LastName + ",</b> " +
                        //        "<br> Your application into Emus Institute was successful and you Student ID is " + "<b>" + user?.StudentId + ".</b>" +
                        //        "<br/> You will receive a notification from the school with further details when your application has been approved." +
                        //        "<br/> <br/> Once again, Congratulations! " +
                        //        "<br/> <br/> Thank you " +
                        //        "<br/> <br/> Emus Institute Team";
                        //    _emailService.SendEmail(toEmail, subject, message);
                        //    return true;
                        //}
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckIfUserIsStudent(string email)
        {
            if (email != null)
            {
                var checkIfStudent = _context.ApplicationUser.Where(x => x.Email == email && x.IsStudent && !x.Deactivated).FirstOrDefault();
                if (checkIfStudent != null)
                {
                    return true;
                }
            }
            return false;
        }

        //public async Task<EvaluationDetails> SaveStudentEvaluationDetails(EvaluationDetails model)
        //{
        //    try
        //    {
        //        if (model != null)
        //        {
        //            //check if userId exists in evaluation table, if it does, move to payment

        //            var checkIfUserEvaluationDetailsIsSaved = _context.EvaluationDetails.Where(x => x.UserId == model.UserId).FirstOrDefault();
        //            if (checkIfUserEvaluationDetailsIsSaved != null)
        //            {
        //                checkIfUserEvaluationDetailsIsSaved.Passport = model.Passport;
        //                checkIfUserEvaluationDetailsIsSaved.SchoolTranscript = model.SchoolTranscript;
        //                checkIfUserEvaluationDetailsIsSaved.HighSchoolCompletionCertificate = model.HighSchoolCompletionCertificate;
        //                checkIfUserEvaluationDetailsIsSaved.WAECScratchCard = model.WAECScratchCard;
        //                checkIfUserEvaluationDetailsIsSaved.OtherCertificate = model.OtherCertificate;
        //                checkIfUserEvaluationDetailsIsSaved.DateAdded = DateTime.Now;

        //                _context.Update(checkIfUserEvaluationDetailsIsSaved);
        //                _context.SaveChanges();
        //                return checkIfUserEvaluationDetailsIsSaved;
        //            }
        //            else
        //            {
        //                var evaluationDetails = new EvaluationDetails();
        //                evaluationDetails.UserId = model.UserId;
        //                evaluationDetails.Passport = model.Passport;
        //                evaluationDetails.SchoolTranscript = model.SchoolTranscript;
        //                evaluationDetails.HighSchoolCompletionCertificate = model.HighSchoolCompletionCertificate;
        //                evaluationDetails.WAECScratchCard = model.WAECScratchCard;
        //                evaluationDetails.OtherCertificate = model.OtherCertificate;
        //                evaluationDetails.DateAdded = DateTime.Now;

        //                _context.EvaluationDetails.Add(evaluationDetails);
        //                _context.SaveChanges();
        //                return evaluationDetails;
        //            }
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<EvaluationDetails> SaveStudentEvaluationDetails(string UserId, string passport, string transcript,
            string highSchCert, string waecScratchCard, string anyRelevantCert)
        {
            try
            {
                if (UserId != null)
                {
                    //check if userId exists in evaluation table, if it does, move to payment
                    var checkIfUserEvaluationDetailsIsSaved = _context.EvaluationDetails.Where(x => x.UserId == UserId).FirstOrDefault();
                    if (checkIfUserEvaluationDetailsIsSaved != null)
                    {
                        checkIfUserEvaluationDetailsIsSaved.Passport = passport;
                        checkIfUserEvaluationDetailsIsSaved.SchoolTranscript = transcript;
                        checkIfUserEvaluationDetailsIsSaved.HighSchoolCompletionCertificate = highSchCert;
                        checkIfUserEvaluationDetailsIsSaved.WAECScratchCard = waecScratchCard;
                        checkIfUserEvaluationDetailsIsSaved.OtherCertificate = anyRelevantCert;
                        checkIfUserEvaluationDetailsIsSaved.DateAdded = DateTime.Now;

                        _context.Update(checkIfUserEvaluationDetailsIsSaved);
                        _context.SaveChanges();
                        return checkIfUserEvaluationDetailsIsSaved;
                    }
                    else
                    {
                        var evaluationDetails = new EvaluationDetails();
                        evaluationDetails.UserId = UserId;
                        evaluationDetails.Passport = passport;
                        evaluationDetails.SchoolTranscript = transcript;
                        evaluationDetails.HighSchoolCompletionCertificate = highSchCert;
                        evaluationDetails.WAECScratchCard = waecScratchCard;
                        evaluationDetails.OtherCertificate = anyRelevantCert;
                        evaluationDetails.DateAdded = DateTime.Now;

                        _context.EvaluationDetails.Add(evaluationDetails);
                        _context.SaveChanges();
                        return evaluationDetails;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SendPaymentCompletionEmail(string email)
        {
            if (email != null)
            {
                var getUser = FindByEmailAsync(email).Result;
                string toEmail = email;
                string subject = "Payment Successful";
                string message = "Dear " + getUser.FirstName + "<b>" + " " + ",</b> " +
                    ". <br/> <br/> Your school and course payment fee was successful. " +
                    "Please, login with your details and continue to class. " +
                    "<br/> <br/> Thank you  " +
                    "<br/> <br/> Emus Institute Team";
                _emailService.SendEmail(toEmail, subject, message);
                return true;
            }
            return false;
        }

        public async Task<StaffEvaluationDetails> SaveStaffEvaluationDetails(string userId, string passport, string transcript,
            string highSchCert, string waecScratchCard, string anyRelevantCert)
        {
            try
            {
                if (userId == null)
                {
                    return null;
                }

                var existing = _context.StaffEvaluationDetails.FirstOrDefault(x => x.UserId == userId);
                if (existing != null)
                {
                    existing.Passport = passport;
                    existing.SchoolTranscript = transcript;
                    existing.HighSchoolCompletionCertificate = highSchCert;
                    existing.WAECScratchCard = waecScratchCard;
                    existing.OtherCertificate = anyRelevantCert;
                    existing.DateAdded = DateTime.Now;
                    _context.Update(existing);
                    _context.SaveChanges();
                    return existing;
                }

                var evaluationDetails = new StaffEvaluationDetails
                {
                    UserId = userId,
                    Passport = passport,
                    SchoolTranscript = transcript,
                    HighSchoolCompletionCertificate = highSchCert,
                    WAECScratchCard = waecScratchCard,
                    OtherCertificate = anyRelevantCert,
                    DateAdded = DateTime.Now
                };
                _context.StaffEvaluationDetails.Add(evaluationDetails);
                _context.SaveChanges();
                return evaluationDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SendStaffPaymentCompletionEmail(string email)
        {
            if (email == null)
            {
                return false;
            }

            var getUser = FindByEmailAsync(email).Result;
            if (getUser == null)
            {
                return false;
            }

            string subject = "Staff Evaluation Payment Successful";
            string message = "Dear " + getUser.FirstName + "<b>" + " " + ",</b> " +
                ". <br/> <br/> Your staff credential evaluation payment of &pound;100 was successful. " +
                "Please, login with your details and continue to your dashboard. " +
                "<br/> <br/> Thank you  " +
                "<br/> <br/> Emus Institute Team";
            _emailService.SendEmail(email, subject, message);
            return true;
        }

        public bool CheckIfStaffHasPaid(string email)
        {
            if (email == null)
            {
                return false;
            }

            var staffUser = _context.ApplicationUser
                .FirstOrDefault(x => x.Email == email && x.IsAdmin && !x.Deactivated);

            return staffUser != null && staffUser.Paid;
        }

        public ApplicationUser GetStudentDetails(string userId)
        {
            var studentDetail = _context.ApplicationUser.Where(x => x.Id == userId && x.IsStudent && !x.Deactivated)
                .Include(a => a.Department).FirstOrDefault();
            if (studentDetail != null)
            {
                return studentDetail;
            }
            return null;
        }

        public async Task<bool> RegStaff(ApplicationUserViewModel userDetails, string staffPosition, string appLetter, string validId, string resume, string linkToClick)
        {
            try
            {
                if (userDetails != null)
                {
                    var staffPassword = GenerateStaffPassword();
                    var user = new ApplicationUser();
                    user.UserName = userDetails.Email;
                    user.Email = userDetails.Email;
                    user.FirstName = userDetails.FirstName;
                    user.LastName = userDetails.LastName;
                    user.PhoneNumber = userDetails.Phonenumber;
                    user.Password = staffPassword;
                    user.DateRegistered = DateTime.Now;
                    user.Deactivated = false;
                    user.Address = userDetails.Address;
                    user.Country = userDetails.Country;
                    user.State = userDetails.State;
                    user.DOB = userDetails.DOB;
                    user.OtherName = userDetails.OtherName;
                    user.IsStudent = false;
                    user.Paid = false;
                    user.IsAdmin = true;
                    user.DepartmentId = userDetails.DepartmentId != 0 ? userDetails.DepartmentId : null;
                    if (userDetails.DepartmentId > 0)
                    {
                        user.StaffType = StaffType.AcademicStaff;
                        var createStaff = await _userManager.CreateAsync(user, staffPassword).ConfigureAwait(false);
                        if (createStaff.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "AcademicStaff").ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        user.StaffType = StaffType.NonAcademicStaff;
                        var createUser = await _userManager.CreateAsync(user, staffPassword).ConfigureAwait(false);
                        if (createUser.Succeeded)
                        {
                            if (staffPosition.ToLower().Contains("humanresource"))
                            {
                                await _userManager.AddToRoleAsync(user, "HumanResourceOfficer").ConfigureAwait(false);
                            }
                            if (staffPosition.ToLower().Contains("admissionofficer"))
                            {
                                await _userManager.AddToRoleAsync(user, "AdmissionOfficer").ConfigureAwait(false);
                            }
                            if (staffPosition.ToLower().Contains("librarianofficer"))
                            {
                                await _userManager.AddToRoleAsync(user, "LibrarianOfficer").ConfigureAwait(false);
                            }
                            if (staffPosition.ToLower().Contains("accountofficer"))
                            {
                                await _userManager.AddToRoleAsync(user, "AccountOfficer").ConfigureAwait(false);
                            }
                            //if (staffPosition.ToLower().Contains("examsofficer"))
                            //{
                            //    await _userManager.AddToRoleAsync(user, "ExamsOfficer").ConfigureAwait(false);
                            //}
                            //if (staffPosition.ToLower().Contains("businessdevofficer"))
                            //{
                            //    await _userManager.AddToRoleAsync(user, "BusinessDevOfficer").ConfigureAwait(false);
                            //}
                            //if (staffPosition.ToLower().Contains("publicrelofficer"))
                            //{
                            //    await _userManager.AddToRoleAsync(user, "PublicRelOfficer").ConfigureAwait(false);
                            //}
                            //if (staffPosition.ToLower().Contains("studentaffairs"))
                            //{
                            //    await _userManager.AddToRoleAsync(user, "ViceChancellorStudentAffairs").ConfigureAwait(false);
                            //}
                        }
                    }
                    AddStaffDocuments(user.Id, staffPosition, user.DepartmentId, validId, appLetter, resume);
                    if (user.Id != null)
                    {
                        if (user.Email != null && !string.IsNullOrEmpty(linkToClick))
                        {
                            var evaluationUrl = linkToClick + user.Id;
                            string staffSubject = "Staff Application Submission";
                            string staffMessage = "Hello, <b>" + user?.FirstName + " " + user?.LastName + ",</b> " +
                                "<br> Your application to join Emus Institute was received successfully." +
                                "<br/> <br/> Please complete your staff credential evaluation to proceed with your application." +
                                "<br/> <br/> Click the button below to upload your documents and make the evaluation payment of &pound;100 " +
                                "(which covers application, transcript review and certificate evaluation)" +
                                "<br>" + "<a style:'border:2px; text-decoration: none;' href='" + evaluationUrl + "' target='_blank'>" +
                                "<button style='color:white; background-color:#06BBCC; padding:12px; border:1px solid #06BBCC;'> Evaluate Credentials </button></a>" +
                                "<br/> <br/> After payment, you can login with the following credentials:" +
                                "<br> <b>Email:</b> " + user.Email +
                                "<br> <b>Password:</b> " + staffPassword +
                                "<br/> <br/> Please keep these details safe." +
                                "<br/> <br/> Thank you " +
                                "<br/> <br/> Emus Institute Team";
                            _emailService.SendEmail(user.Email, staffSubject, staffMessage);
                        }

                        var adminEmail = "nwachukwuarinze00@gmail.com";
                        string adminSubject = "Staff Application Submission";
                        string adminMessage = "Hello SuperAdmin, <br> A staff application has been submitted, by " + "<b>" + user?.FirstName + " " + user?.LastName + " </b> on " + user.DateRegistered.ToString() + ". " +
                                         "<br> Please, endeavor to review the pending application and make the necessary move. " +
                                         "<br> Thank you!!!";
                        _emailService.SendEmail(adminEmail, adminSubject, adminMessage);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool AddStaffDocuments(string userId, string staffPosition, int? deptId, string validId, string appLetter, string resume)
        {
            if (userId != null && validId != null && appLetter != null && resume != null)
            { 
                var position = 0;
                if (deptId == null)
                {
                    position = GetStaffPosition(staffPosition);
                }
                var addStaff = new StaffDocumentation()
                {
                    DateCreated = DateTime.Now,
                    Active = true,
                    StaffStatus = StaffStatus.Pending,
                    UserId = userId,
                    StaffPosition = deptId != null ? StaffPosition.AcademicStaff : (StaffPosition)position,
                    ApplicationLetter = appLetter,
                    Identification = validId,
                    Resume = resume,
                };
                _context.Add(addStaff);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public int GetStaffPosition(string staffPosition)
        {
            if (staffPosition.ToLower().Contains("humanresource"))
            {
                return (int)StaffPosition.HumanResource;
            }
            if (staffPosition.ToLower().Contains("vicechancelloracademics"))
            {
                return (int)StaffPosition.ViceChancellorAcademics;
            }
            if (staffPosition.ToLower().Contains("admissionofficer"))
            {
                return (int)StaffPosition.AdmissionOfficer;
            }
            if (staffPosition.ToLower().Contains("librarianofficer"))
            {
                return (int)StaffPosition.LibrarianOfficer;
            }
            if (staffPosition.ToLower().Contains("accountofficer"))
            {
                return (int)StaffPosition.AccountOfficer;
            }
            if (staffPosition.ToLower().Contains("examsofficer"))
            {
                return (int)StaffPosition.ExamsOfficer;
            }
            if (staffPosition.ToLower().Contains("businessdevofficer"))
            {
                return (int)StaffPosition.BusinessDevOfficer;
            }
            if (staffPosition.ToLower().Contains("publicrelofficer"))
            {
                return (int)StaffPosition.PublicRelOfficer;
            }
            if (staffPosition.ToLower().Contains("vicechancellorstudentaffairs"))
            {
                return (int)StaffPosition.ViceChancellorStudentAffairs;
            }
            return 0;
        }

        public StaffDocumentation GetCoverLetter(int id)
        {
            var getCoverLetter = _context.StaffDocuments.Where(x => x.Id == id && x.Active).FirstOrDefault();
            if (getCoverLetter != null)
            {
                return getCoverLetter;
            }
            return null;
        }

        public string GenerateStaffPassword()
        {
            var dateConvert = DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
            int sequence = 10;
            var initial = "EmusINSStaff";
            var staffPassword = initial + dateConvert + sequence;
            return staffPassword;
        }
        public bool CheckIfApproved(int id)
        {
            if (id > 0)
            {
                return _context.StaffDocuments.Where(x => x.Id == id && x.StaffStatus == StaffStatus.Approved).Any();
            }
            return false;
        }
        public bool CheckIfDeclined(int id)
        {
            if (id > 0)
            {
                return _context.StaffDocuments.Where(x => x.Id == id && x.StaffStatus == StaffStatus.Rejected).Any();
            }
            return false;
        }

    }
}
