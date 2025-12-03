using Core.DB;
using Core.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace e_college.Controllers
{
    //[Authorize(Roles = "Staff, SuperAdmin")]
    public class AcademicStaffController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;

        public AcademicStaffController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserHelper userHelper)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult ReferralLink()
        {
            try
            {
                var url = "/AcademicStaff/Index"; //this url is to lead the user to the payment if he has not paid yet before getting the refId
                var currentUser = _userHelper.FindByUserName(User.Identity.Name);
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
