
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
    public class StudentController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;

        public StudentController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserHelper userHelper)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var userId = _userHelper.GetCurrentUserId(User?.Identity?.Name);
            var getStudentDetails = _userHelper.GetStudentDetails(userId);
            //var departments = _superAdminHelper.GetTotalDepartments();
            var model = new ApplicationUserViewModel()
            {
                CurrentSession = getStudentDetails.CurrentSession,
                FirstName = getStudentDetails.FirstName,
                LastName = getStudentDetails.LastName,
                AcademicLevel = getStudentDetails.AcademicLevel,
                DepartmentName = getStudentDetails?.Department?.Name

                //TotalDepartments = departments,
            };
            return View(model);
        }
    }
}
