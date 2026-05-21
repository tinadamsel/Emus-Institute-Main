using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace e_college.Controllers
{
    //[Authorize(Roles = "Staff, SuperAdmin")]
    public class AcademicStaffController : Controller
    {
        private const decimal ReferralCommissionPerStudent = 20m;

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly IStaffPaymentHelper _staffPaymentHelper;
        private readonly IStaffPaystackHelper _staffPaystackHelper;

        public AcademicStaffController(
            AppDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper,
            IStaffPaymentHelper staffPaymentHelper,
            IStaffPaystackHelper staffPaystackHelper)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
            _staffPaymentHelper = staffPaymentHelper;
            _staffPaystackHelper = staffPaystackHelper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ApplicationUserViewModel();
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser != null)
            {
                var referralStats = GetReferralStats(currentUser.Id);
                model.TotalReferredStudents = referralStats.ReferredCount;
                model.ReferralEarnings = referralStats.TotalEarnings;
            }

            return View(model);
        }

        private (int ReferredCount, decimal TotalEarnings) GetReferralStats(string staffUserId)
        {
            if (string.IsNullOrWhiteSpace(staffUserId))
            {
                return (0, 0m);
            }

            var referredCount = _context.ApplicationUser
                .Count(u => u.RefLink == staffUserId && !u.Deactivated);

            var totalEarnings = referredCount * ReferralCommissionPerStudent;
            return (referredCount, totalEarnings);
        }

        [HttpGet]
        public IActionResult EvaluateCredentials(string userId)
        {
            if (userId == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var staffUser = _context.ApplicationUser
                .FirstOrDefault(x => x.Id == userId && x.IsAdmin && !x.Deactivated);

            if (staffUser == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var model = new ApplicationUserViewModel
            {
                Id = userId,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> EvaluateStaffDetails(string userId, string passport,
            string transcript, string highSchCert, string waecScratchCard, string anyRelevantCert)
        {
            if (userId == null || passport == null || transcript == null || highSchCert == null ||
                waecScratchCard == null || anyRelevantCert == null)
            {
                return Json(new { isError = true, msg = "Network Error" });
            }

            var staffUser = _context.ApplicationUser
                .FirstOrDefault(x => x.Id == userId && x.IsAdmin && !x.Deactivated);

            if (staffUser == null)
            {
                return Json(new { isError = true, msg = "Staff account not found." });
            }

            var saveEvaluationDetails = await _userHelper.SaveStaffEvaluationDetails(
                userId, passport, transcript, highSchCert, waecScratchCard, anyRelevantCert).ConfigureAwait(false);

            if (saveEvaluationDetails == null)
            {
                return Json(new
                {
                    isError = true,
                    msg = "Failed to save evaluation details. If issue persists, contact the admin."
                });
            }

            var response = await _staffPaymentHelper.CreateStaffPayment(saveEvaluationDetails.UserId, staffUser).ConfigureAwait(false);
            if (response != null)
            {
                return Json(new
                {
                    isError = false,
                    data = response.data.authorization_url,
                    msg = "Staff evaluation details saved. Click ok to continue your payment of £100."
                });
            }

            return Json(new
            {
                isError = true,
                msg = "Failed to make payments. If issue persists, contact the admin."
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> StaffPaystackResponseFeedback(PayStack paystack)
        {
            var paystackResponse = await _staffPaystackHelper.VerifyStaffPayment(paystack).ConfigureAwait(false);

            if (paystackResponse?.data?.customer?.email != null)
            {
                TempData["Message"] = "Staff evaluation payment successful. Visit your email for confirmation, then login.";
                _userHelper.SendStaffPaymentCompletionEmail(paystackResponse.data.customer.email);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public JsonResult ReferralLink()
        {
            try
            {
                var url = "/AcademicStaff/Index";
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser != null)
                {
                    var link = currentUser.Id;
                    string referralLink = HttpContext.Request.Scheme.ToString()
                        + "://" + HttpContext.Request.Host.ToString() + "/Account/StudentRegistration?rl=" + link;
                    return Json(referralLink);
                }
                return Json(new { isError = true, dashboard = url });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult Textbooks()
        {
            return View();
        }
    }
}
