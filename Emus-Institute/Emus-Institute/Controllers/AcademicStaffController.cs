using Core.DB;
using Core.Models;
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
        public IActionResult Textbooks()
        {
            return View();
        }
    }
}
