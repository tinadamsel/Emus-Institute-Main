using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Core.DB.ECollegeEnums;

namespace e_college.Controllers
{
    public class AcademicStaffController : Controller
    {
        private const decimal ReferralCommissionPerStudent = 20m;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly IStaffPaymentHelper _staffPaymentHelper;
        private readonly IStaffPaystackHelper _staffPaystackHelper;
        private readonly ITextbookHelper _textbookHelper;
        private readonly ILiveSessionHelper _liveSessionHelper;
        private readonly ICbtHelper _cbtHelper;
        private readonly IAnnouncementHelper _announcementHelper;
        private readonly IAssignmentHelper _assignmentHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AcademicStaffController(
            AppDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper,
            IStaffPaymentHelper staffPaymentHelper,
            IStaffPaystackHelper staffPaystackHelper,
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
            _staffPaymentHelper = staffPaymentHelper;
            _staffPaystackHelper = staffPaystackHelper;
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
            var model = new ApplicationUserViewModel();
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser != null)
            {
                var referralStats = GetReferralStats(currentUser.Id);
                model.TotalReferredStudents = referralStats.ReferredCount;
                model.ReferralEarnings = referralStats.TotalEarnings;
                model.TotalDepartmentTextbooks = _textbookHelper.GetStaffDepartmentTextbooks(currentUser.Id).Count;
                model.TotalAssignments = _assignmentHelper.GetStaffAssignmentCount(currentUser.Id);
            }
            return View(model);
        }

        private (int ReferredCount, decimal TotalEarnings) GetReferralStats(string staffUserId)
        {
            if (string.IsNullOrWhiteSpace(staffUserId))
            {
                return (0, 0m);
            }
            var referredCount = _context.ApplicationUser.Count(u => u.RefLink == staffUserId && !u.Deactivated);
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
            var staffUser = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && x.IsAdmin && !x.Deactivated);
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
            var staffUser = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && x.IsAdmin && !x.Deactivated);
            if (staffUser == null)
            {
                return Json(new { isError = true, msg = "Staff account not found." });
            }
            var saveEvaluationDetails = await _userHelper.SaveStaffEvaluationDetails(userId, passport, transcript, highSchCert, waecScratchCard, anyRelevantCert).ConfigureAwait(false);
            if (saveEvaluationDetails == null)
            {
                return Json(new { isError = true, msg = "Failed to save evaluation details. If issue persists, contact the admin." });
            }
            var response = await _staffPaymentHelper.CreateStaffPayment(saveEvaluationDetails.UserId, staffUser).ConfigureAwait(false);
            if (response != null)
            {
                return Json(new { isError = false, data = response.data.authorization_url, msg = "Staff evaluation details saved. Click ok to continue your payment of £100." });
            }
            return Json(new { isError = true, msg = "Failed to make payments. If issue persists, contact the admin." });
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

        public IActionResult StaffCredentialEvaluation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Textbooks()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.DepartmentName = currentUser.Department?.Name
                ?? _context.Departments.FirstOrDefault(d => d.Id == currentUser.DepartmentId)?.Name
                ?? "Your department";
            var textbooks = _textbookHelper.GetStaffDepartmentTextbooks(currentUser.Id);
            return View(textbooks);
        }

        [HttpGet]
        public IActionResult CreateTextbook()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (currentUser.DepartmentId == null || currentUser.DepartmentId <= 0)
            {
                TempData["Error"] = "You must be assigned to a department before uploading textbooks.";
                return RedirectToAction(nameof(Textbooks));
            }

            ViewBag.DepartmentName = currentUser.Department?.Name
                ?? _context.Departments.FirstOrDefault(d => d.Id == currentUser.DepartmentId)?.Name
                ?? "Your department";
            ViewBag.Sessions = Enum.GetValues(typeof(TextbookEnum)).Cast<TextbookEnum>().ToList();
            return View(new TextbookViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> CreateTextbook(string name, string description, string textbookCode, int textbookForEachSession, IFormFile pdfFile)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null)
                {
                    return Json(new { isError = true, msg = "Please login again." });
                }
                if (string.IsNullOrWhiteSpace(name))
                {
                    return Json(new { isError = true, msg = "Textbook name is required." });
                }
                if (pdfFile == null || pdfFile.Length == 0)
                {
                    return Json(new { isError = true, msg = "Please upload a PDF file." });
                }
                if (!Enum.IsDefined(typeof(TextbookEnum), textbookForEachSession))
                {
                    return Json(new { isError = true, msg = "Please select a valid session." });
                }

                var model = new TextbookViewModel
                {
                    Name = name,
                    Description = description,
                    TextbookCode = textbookCode,
                    TextbookForEachSession = (TextbookEnum)textbookForEachSession
                };

                var created = await _textbookHelper.CreateTextbookAsync(
                    model,
                    pdfFile,
                    currentUser.Id,
                    _webHostEnvironment.WebRootPath).ConfigureAwait(false);

                if (created)
                {
                    return Json(new { isError = false, msg = "Textbook uploaded successfully and is pending librarian approval." });
                }
                return Json(new { isError = true, msg = "Unable to create textbook. Ensure a PDF is uploaded and you are assigned to a department." });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult DownloadTextbook(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var textbook = _textbookHelper.GetTextbookForDownload(id, currentUser.Id, isStudent: false);
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
        public IActionResult LiveSessions()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.DepartmentName = currentUser.Department?.Name
                ?? _context.Departments.FirstOrDefault(d => d.Id == currentUser.DepartmentId)?.Name
                ?? "Your department";
            var sessions = _liveSessionHelper.GetStaffDepartmentLiveSessions(currentUser.Id);
            return View(sessions);
        }

        [HttpPost]
        public async Task<JsonResult> CreateLiveSession(string roomName, string? welcomeMessage, string startDateTime, int durationMinutes)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null)
                {
                    return Json(new { isError = true, msg = "Please login again." });
                }

                if (string.IsNullOrWhiteSpace(roomName))
                {
                    return Json(new { isError = true, msg = "Room name is required." });
                }

                if (!DateTime.TryParse(startDateTime, out var parsedStart))
                {
                    return Json(new { isError = true, msg = "Please enter a valid start date and time." });
                }

                if (durationMinutes <= 0)
                {
                    return Json(new { isError = true, msg = "Please select a valid duration." });
                }

                var model = new LiveSessionViewModel
                {
                    RoomName = roomName,
                    WelcomeMessage = welcomeMessage,
                    StartDateTime = parsedStart,
                    DurationMinutes = durationMinutes
                };

                var result = await _liveSessionHelper.CreateLiveSessionAsync(model, currentUser.Id).ConfigureAwait(false);
                if (result.Success)
                {
                    return Json(new { isError = false, msg = result.Message });
                }

                return Json(new { isError = true, msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult JoinLiveSession(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var session = _liveSessionHelper.GetLiveSessionForJoin(id, currentUser.Id, isStaff: true);
            if (session == null)
            {
                return NotFound();
            }

            if (!session.CanJoin)
            {
                TempData["Error"] = session.IsUpcoming
                    ? "This room opens 10 minutes before the scheduled start time."
                    : "This session is not available to join right now.";
                return RedirectToAction(nameof(LiveSessions));
            }

            return View(session);
        }

        [HttpGet]
        public IActionResult CbtTests(string? search, string? filter)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            ViewBag.DepartmentName = currentUser.Department?.Name
                ?? _context.Departments.FirstOrDefault(d => d.Id == currentUser.DepartmentId)?.Name
                ?? "Your department";
            ViewBag.Search = search;
            ViewBag.Filter = filter ?? "all";
            return View(_cbtHelper.GetStaffCbtTests(currentUser.Id, search, filter));
        }

        [HttpGet]
        public IActionResult CreateCbtTest()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");
            if (currentUser.DepartmentId == null)
            {
                TempData["Error"] = "You must be assigned to a department before creating CBT tests.";
                return RedirectToAction(nameof(CbtTests));
            }

            ViewBag.DepartmentName = currentUser.Department?.Name ?? "Your department";
            return View(new CbtTestViewModel { MaximumAttempts = 1, MarkPerQuestion = 1, DurationMinutes = 30 });
        }

        [HttpPost]
        public async Task<JsonResult> CreateCbtTest(
            string title, string? description, int durationMinutes, string startDateTime, string endDateTime,
            decimal passMark, decimal markPerQuestion, int maximumAttempts, bool shuffleQuestions, bool shuffleOptions,
            bool isPublished, string? browserCode, string? instructions)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null) return Json(new { isError = true, msg = "Please login again." });
                if (!DateTime.TryParse(startDateTime, out var start) || !DateTime.TryParse(endDateTime, out var end))
                    return Json(new { isError = true, msg = "Please enter valid start and end dates." });

                var model = new CbtTestViewModel
                {
                    Title = title,
                    Description = description,
                    DurationMinutes = durationMinutes,
                    StartDateTime = start,
                    EndDateTime = end,
                    PassMark = passMark,
                    MarkPerQuestion = markPerQuestion,
                    MaximumAttempts = maximumAttempts,
                    ShuffleQuestions = shuffleQuestions,
                    ShuffleOptions = shuffleOptions,
                    IsPublished = isPublished,
                    BrowserCode = browserCode,
                    Instructions = instructions
                };

                var result = await _cbtHelper.CreateCbtTestAsync(model, currentUser.Id).ConfigureAwait(false);
                if (result.Success)
                    return Json(new { isError = false, msg = result.Message, testId = result.TestId });
                return Json(new { isError = true, msg = result.Message });
            }
            catch (Exception ex) { return Json(new { isError = true, msg = ex.Message }); }
        }

        [HttpGet]
        public IActionResult EditCbtTest(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var test = _cbtHelper.GetStaffCbtTest(id, currentUser.Id);
            if (test == null) return NotFound();
            if (!test.CanEdit)
            {
                TempData["Error"] = "This test has started. Only analytics and scores can be viewed.";
                return RedirectToAction(nameof(CbtTestAnalytics), new { id });
            }

            ViewBag.DepartmentName = test.DepartmentName;
            return View(test);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCbtTest(
            int id, string title, string? description, int durationMinutes, string startDateTime, string endDateTime,
            decimal passMark, decimal markPerQuestion, int maximumAttempts, bool shuffleQuestions, bool shuffleOptions,
            bool isPublished, string? browserCode, string? instructions)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null) return Json(new { isError = true, msg = "Please login again." });
                if (!DateTime.TryParse(startDateTime, out var start) || !DateTime.TryParse(endDateTime, out var end))
                    return Json(new { isError = true, msg = "Please enter valid start and end dates." });

                var model = new CbtTestViewModel
                {
                    Id = id,
                    Title = title,
                    Description = description,
                    DurationMinutes = durationMinutes,
                    StartDateTime = start,
                    EndDateTime = end,
                    PassMark = passMark,
                    MarkPerQuestion = markPerQuestion,
                    MaximumAttempts = maximumAttempts,
                    ShuffleQuestions = shuffleQuestions,
                    ShuffleOptions = shuffleOptions,
                    IsPublished = isPublished,
                    BrowserCode = browserCode,
                    Instructions = instructions
                };

                var result = await _cbtHelper.UpdateCbtTestAsync(model, currentUser.Id).ConfigureAwait(false);
                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex) { return Json(new { isError = true, msg = ex.Message }); }
        }

        [HttpPost]
        public JsonResult DeleteCbtTest(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return Json(new { isError = true, msg = "Please login again." });
            var result = _cbtHelper.DeleteCbtTest(id, currentUser.Id);
            return Json(new { isError = !result.Success, msg = result.Message });
        }

        [HttpGet]
        public IActionResult ManageCbtQuestions(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var test = _cbtHelper.GetStaffCbtTest(id, currentUser.Id);
            if (test == null) return NotFound();

            ViewBag.Test = test;
            return View(_cbtHelper.GetQuestionsForTest(id, currentUser.Id));
        }

        [HttpPost]
        public async Task<JsonResult> SaveCbtQuestion(
            int cbtTestId, int questionId, int questionType, string questionText,
            string? optionA, string? optionB, string? optionC, string? optionD,
            string correctAnswer, decimal marks, string? explanation, IFormFile? imageFile)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null) return Json(new { isError = true, msg = "Please login again." });

                var model = new CbtQuestionViewModel
                {
                    CbtTestId = cbtTestId,
                    QuestionType = (CbtQuestionType)questionType,
                    QuestionText = questionText,
                    OptionA = optionA,
                    OptionB = optionB,
                    OptionC = optionC,
                    OptionD = optionD,
                    CorrectAnswer = correctAnswer,
                    Marks = marks,
                    Explanation = explanation
                };

                var result = await _cbtHelper.SaveQuestionAsync(
                    model, imageFile, currentUser.Id, _webHostEnvironment.WebRootPath,
                    questionId > 0 ? questionId : null).ConfigureAwait(false);
                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex) { return Json(new { isError = true, msg = ex.Message }); }
        }

        [HttpPost]
        public JsonResult DeleteCbtQuestion(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return Json(new { isError = true, msg = "Please login again." });
            var result = _cbtHelper.DeleteQuestion(id, currentUser.Id);
            return Json(new { isError = !result.Success, msg = result.Message });
        }

        [HttpGet]
        public IActionResult CbtTestScores(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var test = _cbtHelper.GetStaffCbtTest(id, currentUser.Id);
            if (test == null) return NotFound();

            ViewBag.Test = test;
            return View(_cbtHelper.GetTestScores(id, currentUser.Id));
        }

        [HttpGet]
        public IActionResult CbtTestAnalytics(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null) return RedirectToAction("Login", "Account");

            var analytics = _cbtHelper.GetTestAnalytics(id, currentUser.Id);
            if (analytics == null) return NotFound();
            return View(analytics);
        }

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
            return View(_announcementHelper.GetStaffAnnouncements(currentUser.Id));
        }

        [HttpPost]
        public async Task<JsonResult> CreateAnnouncement(
            string title,
            string message,
            int audience,
            int? departmentId,
            string startDateTime,
            string endDateTime)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null)
                {
                    return Json(new { isError = true, msg = "Please login again." });
                }

                if (!DateTime.TryParse(startDateTime, out var start) || !DateTime.TryParse(endDateTime, out var end))
                {
                    return Json(new { isError = true, msg = "Please enter valid start and end dates." });
                }

                if (!Enum.IsDefined(typeof(AnnouncementAudience), audience))
                {
                    return Json(new { isError = true, msg = "Please select a valid audience." });
                }

                var result = await _announcementHelper.CreateAnnouncementAsync(
                    currentUser.Id,
                    title,
                    message,
                    (AnnouncementAudience)audience,
                    departmentId,
                    start,
                    end).ConfigureAwait(false);

                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Assignments()
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.DepartmentName = currentUser.Department?.Name
                ?? _context.Departments.FirstOrDefault(d => d.Id == currentUser.DepartmentId)?.Name
                ?? "Your department";
            return View(_assignmentHelper.GetStaffDepartmentAssignments(currentUser.Id));
        }

        [HttpPost]
        public async Task<JsonResult> CreateAssignment(string name, string description, string validUntilDate, decimal totalMarks, IFormFile? file)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null)
                {
                    return Json(new { isError = true, msg = "Please login again." });
                }

                if (!DateTime.TryParse(validUntilDate, out var dueDate))
                {
                    return Json(new { isError = true, msg = "Please enter a valid due date." });
                }

                var model = new AssignmentViewModel
                {
                    Name = name,
                    Description = description,
                    ValidUntilDate = dueDate,
                    TotalMarks = totalMarks
                };

                var result = await _assignmentHelper.CreateAssignmentAsync(
                    model, file, currentUser.Id, _webHostEnvironment.WebRootPath).ConfigureAwait(false);
                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult AssignmentSubmissions(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var page = _assignmentHelper.GetAssignmentSubmissions(id, currentUser.Id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        [HttpPost]
        public async Task<JsonResult> GradeAssignment(int submissionId, decimal score, string? feedback)
        {
            try
            {
                var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
                if (currentUser == null)
                {
                    return Json(new { isError = true, msg = "Please login again." });
                }

                var result = await _assignmentHelper.GradeSubmissionAsync(
                    submissionId, score, feedback, currentUser.Id).ConfigureAwait(false);
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

            var assignment = _assignmentHelper.GetAssignmentFileForDownload(id, currentUser.Id, isStaff: true);
            if (assignment == null)
            {
                return NotFound();
            }

            return ServeUploadedFile(assignment.FilePath, assignment.Name);
        }

        [HttpGet]
        public IActionResult DownloadAssignmentSubmission(int id)
        {
            var currentUser = _userHelper.FindByUserName(User.Identity?.Name);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var submission = _assignmentHelper.GetSubmissionFileForDownload(id, currentUser.Id, isStaff: true);
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
}
