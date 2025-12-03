using Azure;
using Core.Config;
using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace e_college.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly IDropDownHelper _dropdownHelper;
        private readonly IPaymentHelper _paymentHelper;
        private readonly IPaystackHelper _paystackHelper;

        public AccountController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserHelper userHelper, IGeneralConfiguration generalConfiguration, IDropDownHelper dropdownHelper, IPaymentHelper paymentHelper, IPaystackHelper paystackHelper)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
            _generalConfiguration = generalConfiguration;
            _dropdownHelper = dropdownHelper;
            _paymentHelper = paymentHelper;
            _paystackHelper = paystackHelper;
        }
        [HttpGet]
        public IActionResult StudentRegistration()
        {
            ViewBag.Departments = _dropdownHelper.DropdownOfDepartments();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> StudentRegistration(string userDetails, string refLink)
        {
            if (userDetails != null)
            {
                var appUserViewModel = JsonConvert.DeserializeObject<ApplicationUserViewModel>(userDetails);
                if (appUserViewModel != null)
                {
                    var checkEmail = await _userHelper.FindByEmailAsync(appUserViewModel.Email).ConfigureAwait(false);
                    if (checkEmail != null)
                    {
                        return Json(new { isError = true, msg = "Email Already Exists" });
                    }
                    if (appUserViewModel.DepartmentId == 0)
                    {
                        return Json(new { isError = true, msg = "Please select a department" });
                    }
                    if (appUserViewModel.Password != appUserViewModel.ConfirmPassword)
                    {
                        return Json(new { isError = true, msg = "Password and Confirm password do not match" });
                    }
                    if (appUserViewModel.Password.Length < 8)
                    {
                        return Json(new { isError = true, msg = "Password must be from 8 characters" });
                    }
                    string linkToClick = HttpContext.Request.Scheme.ToString() + "://" +
                    HttpContext.Request.Host.ToString() + "/Account/EvaluateCredentials?userId=";

                    var createStudent = await _userHelper.RegisterStudent(appUserViewModel, linkToClick, refLink).ConfigureAwait(false);
                    if (createStudent)
                    {
                        return Json(new { isError = false, msg = "Registration Successful, Login to your email and follow the instructions" });
                    }
                    return Json(new { isError = true, msg = "Unable to register" });
                }
            }
            return Json(new { isError = true, msg = "Network Error" });
        }
        [HttpGet]
        public IActionResult EvaluateCredentials(string userId)
        {
            if (userId != null)
            {
                var model = new ApplicationUserViewModel
                {
                    Id = userId,
                };
                return View(model);
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<JsonResult> EvaluateUserDetails(string UserId, string passport, 
            string transcript, string highSchCert, string waecScratchCard, string anyRelevantCert)
        {
            if (UserId != null && passport != null && transcript != null && highSchCert != null && 
                waecScratchCard != null && anyRelevantCert != null)
            {
                var saveEvaluationDetails = await _userHelper.SaveStudentEvaluationDetails(UserId, passport, transcript, highSchCert, waecScratchCard, anyRelevantCert).ConfigureAwait(false);
                if (saveEvaluationDetails != null)
                {
                    var response = await _paymentHelper.CreateStudentPayment(saveEvaluationDetails.UserId, saveEvaluationDetails?.Users).ConfigureAwait(false);
                    if (response != null)
                    {
                        return Json(new { isError = false, data = response.data.authorization_url, msg = "Evaluation Details saved. Click ok to continue your payment" });
                    }
                }
                return Json(new { isError = true, msg = "Failed to make payments. " +
                "If issue persists, contact the admin ..." });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        [AllowAnonymous]
        public async Task<IActionResult> PaystackResponseFeedback(PayStack paystack)
        { 
            var paystackResponse = await _paystackHelper.VerifyPayment(paystack).ConfigureAwait(false);
            
            //var user = await _userHelper.FindByUserNameAsync(User?.Identity?.Name).ConfigureAwait(false);
            var getUserEmail = paystackResponse.data.customer.email;
            if (paystackResponse != null && getUserEmail != null)
            {
                TempData["Message"] = "Payment Successful. Visit your email to see your payment confirmation and continue to login";

                //_emailHelper.AfterPaymentEmailers(user);
                _userHelper.SendPaymentCompletionEmail(getUserEmail);
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public IActionResult Careers()
        {
            ViewBag.Departments = _dropdownHelper.DropdownOfDepartments();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> StaffRegistration(string userDetails, string staffPosition, string appLetter, string validId)
        {
            if (userDetails != null && appLetter != null && validId != null)
            {
                var appUserViewModel = JsonConvert.DeserializeObject<ApplicationUserViewModel>(userDetails);
                if (appUserViewModel != null)
                {
                    var checkEmail = await _userHelper.FindByEmailAsync(appUserViewModel.Email).ConfigureAwait(false);
                    if (checkEmail != null)
                    {
                        return Json(new { isError = true, msg = "Email Already Exists" });
                    }
                    
                    var createStaff = await _userHelper.RegStaff(appUserViewModel, staffPosition, appLetter, validId).ConfigureAwait(false);
                    if (createStaff)
                    {
                        return Json(new { isError = false, msg = "Application successful. Thank you for your interest. Our team will contact you soon", });
                    }
                }
                return Json(new { isError = true, msg = "Unable to register" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }
      


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Login(string email, string password)
        {
            if (email != null && password != null)
            {
                var filterSpace = email.Replace(" ", "");
                var url = "";
                var existingUser = _userHelper.FindByEmailAsync(filterSpace).Result;
                if (existingUser != null) 
                {
                    var userRole = await _userManager.GetRolesAsync(existingUser).ConfigureAwait(false);
                    if (userRole.FirstOrDefault().ToLower().Contains("superadmi"))
                    {
                        var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                        if (PasswordSignIn.Succeeded)
                        {
                            url = "/SuperAdmin/Index";
                            return Json(new { isError = false, dashboard = url });
                        }
                    }
                    if (userRole.FirstOrDefault().ToLower().Contains("academicstaff"))
                    {
                        var staffIsApproved = _userHelper.CheckIfStaffIsApproved(email);
                        if (!staffIsApproved)
                        {
                            return Json(new { isError = true, msg = "Your application has not been approved yet" });
                        }
                        var checkIfSuspended = _userHelper.CheckIfUserIsSuspended(email);
                        if (checkIfSuspended)
                        {
                            return Json(new { isError = true, msg = "You are still under suspension. Login when your suspension period expires" });
                        }
                        var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                        if (PasswordSignIn.Succeeded)
                        {
                            url = "/AcademicStaff/Index";
                            return Json(new { isError = false, dashboard = url });
                        }
                    }
                    if (userRole.FirstOrDefault().ToLower().Contains("humanresource"))
                    {
                        var staffIsApproved = _userHelper.CheckIfStaffIsApproved(email);
                        if (!staffIsApproved)
                        {
                            return Json(new { isError = true, msg = "Your application has not been approved yet" });
                        }
                        var checkIfSuspended = _userHelper.CheckIfUserIsSuspended(email);
                        if (checkIfSuspended)
                        {
                            return Json(new { isError = true, msg = "You are still under suspension. Login when your suspension period expires" });
                        }
                        var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                        if (PasswordSignIn.Succeeded)
                        {
                            url = "/AcademicStaff/Index";
                            return Json(new { isError = false, dashboard = url });
                        }
                    }
                    if (userRole.FirstOrDefault().ToLower().Contains("admissionofficer"))
                    {
                        var staffIsApproved = _userHelper.CheckIfStaffIsApproved(email);
                        if (!staffIsApproved)
                        {
                            return Json(new { isError = true, msg = "Your application has not been approved yet" });
                        }
                        var checkIfSuspended = _userHelper.CheckIfUserIsSuspended(email);
                        if (checkIfSuspended)
                        {
                            return Json(new { isError = true, msg = "You are still under suspension. Login when your suspension period expires" });
                        }
                        var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                        if (PasswordSignIn.Succeeded)
                        {
                            url = "/AcademicStaff/Index";
                            return Json(new { isError = false, dashboard = url });
                        }
                    }
                    if (userRole.FirstOrDefault().ToLower().Contains("librarianofficer"))
                    {
                        var staffIsApproved = _userHelper.CheckIfStaffIsApproved(email);
                        if (!staffIsApproved)
                        {
                            return Json(new { isError = true, msg = "Your application has not been approved yet" });
                        }
                        var checkIfSuspended = _userHelper.CheckIfUserIsSuspended(email);
                        if (checkIfSuspended)
                        {
                            return Json(new { isError = true, msg = "You are still under suspension. Login when your suspension period expires" });
                        }
                        var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                        if (PasswordSignIn.Succeeded)
                        {
                            url = "/AcademicStaff/Index";
                            return Json(new { isError = false, dashboard = url });
                        }
                    }
                    if (userRole.FirstOrDefault().ToLower().Contains("accountofficer"))
                    {
                        var staffIsApproved = _userHelper.CheckIfStaffIsApproved(email);
                        if (!staffIsApproved)
                        {
                            return Json(new { isError = true, msg = "Your application has not been approved yet" });
                        }
                        var checkIfSuspended = _userHelper.CheckIfUserIsSuspended(email);
                        if (checkIfSuspended)
                        {
                            return Json(new { isError = true, msg = "You are still under suspension. Login when your suspension period expires" });
                        }
                        var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                        if (PasswordSignIn.Succeeded)
                        {
                            url = "/AcademicStaff/Index";
                            return Json(new { isError = false, dashboard = url });
                        }
                    }
                    if (userRole.FirstOrDefault().ToLower().Contains("tertiarystudent"))
                    {
                        var checkIfUserIsStudent = _userHelper.CheckIfUserIsStudent(email);
                        if (!checkIfUserIsStudent)
                        {
                            return Json(new { isError = true, msg = "You application has not been approved yet" });
                        }
                        var checkIfSuspended = _userHelper.CheckIfUserIsSuspended(email);
                        if (checkIfSuspended)
                        {
                            return Json(new { isError = true, msg = "You are still under suspension. Login when your suspension period expires" });
                        }
                        var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                        if (PasswordSignIn.Succeeded)
                        {
                            url = "/Student/Index";
                            return Json(new { isError = false, dashboard = url });
                        }
                    }
                }
                return Json(new { isError = true, msg = "Account does not exist, contact admin" });

                //this was the previous login logic
                //var PasswordSignIn = await _signInManager.PasswordSignInAsync(existingUser, password, true, true).ConfigureAwait(false);
                //if (PasswordSignIn.Succeeded)
                //{
                //    var userRole = await _userManager.GetRolesAsync(existingUser).ConfigureAwait(false);
                //    if (userRole.FirstOrDefault().ToLower().Contains("TertiaryStudent"))
                //    {
                //        url = "/Student/Index";
                //    }
                //    if (userRole.FirstOrDefault().ToLower().Contains("SuperAdmin"))
                //    {
                //        url = "/SuperAdmin/Index";
                //    }
                //    if (userRole.FirstOrDefault().ToLower().Contains("humanresource"))
                //    {
                //        url = "/HumanResource/Index";
                //    }
                //    if (userRole.FirstOrDefault().ToLower().Contains("staff"))
                //    {
                //        url = "/AcademicStaff/Index";
                //    }
                //    if (userRole.FirstOrDefault().ToLower().Contains("admissionofficer"))
                //    {
                //        url = "/AdmissionOAfficer/Index";
                //    }
                //    return Json(new { isError = false, dashboard = url });
                //}
            }
            return Json(new { isError = true, msg = "Username and Password Required" });
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}
