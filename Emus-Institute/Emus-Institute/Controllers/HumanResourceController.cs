using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace e_college.Controllers
{
    //[Authorize(Roles = "HumanResoure, SuperAdmin")]
    public class HumanResourceController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly ISuperAdminHelper _superAdminHelper;

        public HumanResourceController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserHelper userHelper, ISuperAdminHelper superAdminHelper)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
            _superAdminHelper = superAdminHelper;
        }
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    var user = _userHelper.GetCurrentUserId(User?.Identity?.Name);
        //    var academicStaff = _userHelper.GetTotalAcademicStaff();
        //    var nonAcademicStaff = _userHelper.GetTotalNonAcademicStaff();
        //    //var subjects = _superAdminHelper.GetTotalSubjects();
        //    var model = new ApplicationUserViewModel()
        //    {
        //        AcademicStaff = academicStaff,
        //        NonAcademicStaff = nonAcademicStaff,
        //        //TotalSubjects = subjects,
        //    };
        //    return View(model);
        //}

        
       





    }
}
