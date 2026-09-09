using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
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
        private readonly ITextbookHelper _textbookHelper;
        private readonly ILiveSessionHelper _liveSessionHelper;
        private readonly ICbtHelper _cbtHelper;
        private readonly IAnnouncementHelper _announcementHelper;
        private readonly IAssignmentHelper _assignmentHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentController(
            AppDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper,
            ITextbookHelper textbookHelper,
            ILiveSessionHelper liveSessionHelper,
            ICbtHelper cbtHelper,
            IAnnouncementHelper announcementHelper,
            IAssignmentHelper assignmentHelper,
            IWebHostEnvironment webHostEnvironment)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
            _textbookHelper = textbookHelper;
            _liveSessionHelper = liveSessionHelper;
            _cbtHelper = cbtHelper;
            _announcementHelper = announcementHelper;
            _assignmentHelper = assignmentHelper;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = _userHelper.GetCurrentUserId(User?.Identity?.Name);
            var getStudentDetails = _userHelper.GetStudentDetails(userId);
            var textbooksCount = 0;
            var announcementCount = 0;
            var assignmentCount = 0;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                textbooksCount = _textbookHelper.GetApprovedTextbooksForStudent(userId).Count;
                announcementCount = _announcementHelper.GetActiveAnnouncementCountForStudent(userId);
                assignmentCount = _assignmentHelper.GetStudentAssignmentCount(userId);
            }
            ViewBag.HasWholeStudentAnnouncement = _announcementHelper.GetActiveWholeStudentAnnouncementCount() > 0;
            var model = new ApplicationUserViewModel()
            {
                CurrentSession = getStudentDetails.CurrentSession,
                FirstName = getStudentDetails.FirstName,
                LastName = getStudentDetails.LastName,
                AcademicLevel = getStudentDetails.AcademicLevel,
                DepartmentName = getStudentDetails?.Department?.Name,
                TotalDepartmentTextbooks = textbooksCount,
                TotalActiveAnnouncements = announcementCount,
                TotalAssignments = assignmentCount
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Textbooks()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var textbooks = _textbookHelper.GetApprovedTextbooksForStudent(currentUser.Id);
            return View(textbooks);
        }

        [HttpGet]
        public IActionResult DownloadTextbook(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var textbook = _textbookHelper.GetTextbookForDownload(id, currentUser.Id, isStudent: true);
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

        [HttpGet]
        public IActionResult VirtualClass()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var sessions = _liveSessionHelper.GetStudentLiveSessions(currentUser.Id);
            return View(sessions);
        }

        [HttpGet]
        public IActionResult JoinLiveSession(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var session = _liveSessionHelper.GetLiveSessionForJoin(id, currentUser.Id, isStaff: false);
            if (session == null)
            {
                return NotFound();
            }

            if (!session.CanJoin)
            {
                TempData["Error"] = session.IsUpcoming
                    ? "Attend Class is available only during the scheduled session time."
                    : "This session is not available to join right now.";
                return RedirectToAction(nameof(VirtualClass));
            }

            return View(session);
        }

        [HttpGet]
        public IActionResult MyCbtTests()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");
            return View(_cbtHelper.GetStudentCbtTests(currentUser.Id));
        }

        [HttpPost]
        public JsonResult StartCbtTest(int testId, string? browserCode)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return Json(new { isError = true, msg = "Please login again." });

            var result = _cbtHelper.StartAttempt(testId, currentUser.Id, browserCode);
            if (result.Success && result.AttemptId.HasValue)
                return Json(new { isError = false, msg = result.Message, attemptId = result.AttemptId.Value });
            return Json(new { isError = true, msg = result.Message });
        }

        [HttpGet]
        public IActionResult TakeCbtTest(int attemptId)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var attempt = _cbtHelper.GetAttemptForTaking(attemptId, currentUser.Id);
            if (attempt == null)
            {
                TempData["Error"] = "This test session is no longer available.";
                return RedirectToAction(nameof(MyCbtTests));
            }

            return View(attempt);
        }

        [HttpPost]
        public async Task<JsonResult> SubmitCbtTest(int attemptId, [FromBody] CbtSubmitRequest request)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null) return Json(new { isError = true, msg = "Please login again." });

                var answers = request?.Answers?.Select(a => new CbtStudentAnswerViewModel
                {
                    QuestionId = a.QuestionId,
                    SelectedAnswer = a.SelectedAnswer
                }).ToList() ?? new List<CbtStudentAnswerViewModel>();

                var result = await _cbtHelper.SubmitAttemptAsync(
                    attemptId, currentUser.Id, answers, request?.AutoSubmitted ?? false).ConfigureAwait(false);

                if (result.Success && result.Result != null)
                    return Json(new { isError = false, msg = result.Message, attemptId = result.Result.Id });
                return Json(new { isError = true, msg = result.Message });
            }
            catch (Exception ex) { return Json(new { isError = true, msg = ex.Message }); }
        }

        [HttpGet]
        public IActionResult CbtTestResult(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var result = _cbtHelper.GetAttemptResult(id, currentUser.Id);
            if (result == null) return NotFound();
            return View(result);
        }

        [HttpGet]
        public IActionResult Announcements()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var announcements = _announcementHelper.GetStudentAnnouncements(currentUser.Id);
            return View(announcements);
        }

        [HttpGet]
        public IActionResult Assignments()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(_assignmentHelper.GetStudentAssignments(currentUser.Id));
        }

        [HttpGet]
        public IActionResult SubmitAssignment(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var assignment = _assignmentHelper.GetStudentAssignment(id, currentUser.Id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        [HttpPost]
        public async Task<JsonResult> SubmitAssignment(int assignmentId, string? comment, IFormFile? file)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null)
                {
                    return Json(new { isError = true, msg = "Please login again." });
                }

                var result = await _assignmentHelper.SubmitAssignmentAsync(
                    assignmentId, comment, file, currentUser.Id, _webHostEnvironment.WebRootPath).ConfigureAwait(false);
                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult DownloadAssignmentFile(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var assignment = _assignmentHelper.GetAssignmentFileForDownload(id, currentUser.Id, isStaff: false);
            if (assignment == null)
            {
                return NotFound();
            }

            return ServeUploadedFile(assignment.FilePath, assignment.Name);
        }

        [HttpGet]
        public IActionResult DownloadMyAssignmentSubmission(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var submission = _assignmentHelper.GetSubmissionFileForDownload(id, currentUser.Id, isStaff: false);
            if (submission == null)
            {
                return NotFound();
            }

            return ServeUploadedFile(submission.FilePath, submission.OriginalFileName);
        }

        private IActionResult ServeUploadedFile(string? relativePath, string? downloadName)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return NotFound();
            }

            var absolutePath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(absolutePath))
            {
                return NotFound();
            }

            var fileName = string.IsNullOrWhiteSpace(downloadName)
                ? Path.GetFileName(absolutePath)
                : Path.GetFileName(downloadName);
            return PhysicalFile(absolutePath, "application/octet-stream", fileName);
        }
    }

    public class CbtSubmitRequest
    {
        public List<CbtSubmitAnswerRequest>? Answers { get; set; }
        public bool AutoSubmitted { get; set; }
    }

    public class CbtSubmitAnswerRequest
    {
        public int QuestionId { get; set; }
        public string? SelectedAnswer { get; set; }
    }
}
