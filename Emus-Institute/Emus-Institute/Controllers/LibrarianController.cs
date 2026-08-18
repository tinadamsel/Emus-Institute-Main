using Core.DB;
using Core.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace e_college.Controllers
{
    public class LibrarianController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly ITextbookHelper _textbookHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAnnouncementHelper _announcementHelper;

        public LibrarianController(
            AppDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper,
            ITextbookHelper textbookHelper,
            IWebHostEnvironment webHostEnvironment,
            IAnnouncementHelper announcementHelper)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
            _textbookHelper = textbookHelper;
            _webHostEnvironment = webHostEnvironment;
            _announcementHelper = announcementHelper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.PendingCount = _textbookHelper.GetPendingTextbooks().Count;
            return View();
        }

        [HttpGet]
        public IActionResult Textbooks()
        {
            var textbooks = _textbookHelper.GetPendingTextbooks();
            return View(textbooks);
        }

        [HttpPost]
        public JsonResult ApproveTextbook(int id)
        {
            if (id <= 0)
            {
                return Json(new { isError = true, msg = "Invalid textbook." });
            }

            var approved = _textbookHelper.ApproveTextbook(id);
            if (approved)
            {
                return Json(new { isError = false, msg = "Textbook approved successfully." });
            }
            return Json(new { isError = true, msg = "Unable to approve textbook." });
        }

        [HttpPost]
        public JsonResult DeclineTextbook(int id)
        {
            if (id <= 0)
            {
                return Json(new { isError = true, msg = "Invalid textbook." });
            }

            var declined = _textbookHelper.DeclineTextbook(id);
            if (declined)
            {
                return Json(new { isError = false, msg = "Textbook declined successfully." });
            }
            return Json(new { isError = true, msg = "Unable to decline textbook." });
        }

        [HttpGet]
        public IActionResult DownloadTextbook(int id)
        {
            var textbook = _context.Textbooks.FirstOrDefault(x => x.Id == id && x.Active && !string.IsNullOrEmpty(x.FilePath));
            if (textbook == null || string.IsNullOrWhiteSpace(textbook.FilePath))
            {
                return NotFound();
            }

            var absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, textbook.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(absolutePath))
            {
                return NotFound();
            }

            var downloadName = string.IsNullOrWhiteSpace(textbook.Name) ? Path.GetFileName(absolutePath) : $"{textbook.Name}.pdf";
            return PhysicalFile(absolutePath, "application/pdf", downloadName);
        }

        //[HttpGet]
        //public IActionResult Announcements()
        //{
        //    return RedirectToAction("Announcements", "AcademicStaff");
        //}


        [HttpGet]
        public IActionResult Announcements()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Departments = _context.Departments
                .Where(d => d.Active && !d.Deleted && currentUser.DepartmentId == d.Id)
                .OrderBy(d => d.Name)
                .ToList();
            //return View(_announcementHelper.GetStaffAnnouncements(currentUser.Id));
            return View();
        }
    }
}
