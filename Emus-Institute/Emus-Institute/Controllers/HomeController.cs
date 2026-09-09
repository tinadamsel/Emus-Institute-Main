
using Core.Models;
using Core.ViewModels;
using Emus_Institute.Models;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace e_college.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ISuperAdminHelper _superAdminHelper;
		private readonly IStudyCenterHelper _studyCenterHelper;
		private readonly IContactHelper _contactHelper;
		private readonly IPublicAssessmentHelper _publicAssessmentHelper;
		private readonly ICbtHelper _cbtHelper;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public HomeController(
			ILogger<HomeController> logger,
			ISuperAdminHelper superAdminHelper,
			IStudyCenterHelper studyCenterHelper,
			IContactHelper contactHelper,
			IPublicAssessmentHelper publicAssessmentHelper,
			ICbtHelper cbtHelper,
			IWebHostEnvironment webHostEnvironment)
		{
			_logger = logger;
			_superAdminHelper = superAdminHelper;
			_studyCenterHelper = studyCenterHelper;
			_contactHelper = contactHelper;
			_publicAssessmentHelper = publicAssessmentHelper;
			_cbtHelper = cbtHelper;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
            var deptDetails = _superAdminHelper.GetDepartments().Take(3);
            return View(deptDetails);
           
		}
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Courses()
        {
            var deptDetails = _superAdminHelper.GetDepartments();
            return View(deptDetails);
        }
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SubmitContactMessage(string name, string email, string subject, string message)
        {
            try
            {
                var result = _contactHelper.SubmitContactMessage(name, email, subject, message);
                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        public IActionResult DataPolicy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StudyCenters()
        {
            var centers = _studyCenterHelper.GetApprovedStudyCenters();
            return View(centers);
        }

        [HttpGet]
        public IActionResult SubmitStudyCenter()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SubmitStudyCenter(
            string name,
            string address,
            string country,
            string contactPerson,
            IFormFile imageFile)
        {
            try
            {
                var result = await _studyCenterHelper.SubmitStudyCenterAsync(
                    name,
                    address,
                    country,
                    contactPerson,
                    imageFile,
                    _webHostEnvironment.WebRootPath);

                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Assessment()
        {
            var test = _cbtHelper.GetPublishedPublicAssessment();
            ViewBag.HasAssessment = test != null;
            ViewBag.TestTitle = test?.Title;
            ViewBag.DurationMinutes = test?.DurationMinutes ?? 0;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SubmitAssessmentRegistration(
            string email,
            int programType,
            string country,
            int scholarshipType)
        {
            try
            {
                var result = await _publicAssessmentHelper.SubmitRegistrationAndPayAsync(
                    email, programType, country, scholarshipType);
                return Json(new { isError = !result.Success, msg = result.Message, data = result.AuthorizationUrl });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssessmentPaystackResponse(PayStack paystack)
        {
            if (paystack == null)
            {
                paystack = new PayStack();
            }

            if (string.IsNullOrWhiteSpace(paystack.Reference))
            {
                paystack.Reference = Request.Query["reference"].ToString();
                if (string.IsNullOrWhiteSpace(paystack.Reference))
                {
                    paystack.Reference = Request.Query["trxref"].ToString();
                }
            }

            var result = await _publicAssessmentHelper.CompleteAssessmentPaymentAsync(paystack);
            if (!result.Success || result.AccessToken == null)
            {
                TempData["AssessmentError"] = result.Message;
                return RedirectToAction(nameof(Assessment));
            }

            return RedirectToAction(nameof(TakeAssessment), new { token = result.AccessToken });
        }

        [HttpGet]
        public IActionResult TakeAssessment(Guid token)
        {
            if (token == Guid.Empty)
            {
                TempData["AssessmentError"] = "Invalid assessment session.";
                return RedirectToAction(nameof(Assessment));
            }

            var start = _publicAssessmentHelper.OpenPaidAssessment(token);
            if (!start.Success || !start.AttemptId.HasValue)
            {
                TempData["AssessmentError"] = start.Message;
                if (start.AttemptId.HasValue)
                {
                    return RedirectToAction(nameof(AssessmentResult), new { id = start.AttemptId.Value, token });
                }
                return RedirectToAction(nameof(Assessment));
            }

            var attempt = _cbtHelper.GetPublicAttemptForTaking(start.AttemptId.Value, token);
            if (attempt == null)
            {
                var completed = _cbtHelper.GetPublicAttemptResult(start.AttemptId.Value, token);
                if (completed != null)
                {
                    return RedirectToAction(nameof(AssessmentResult), new { id = start.AttemptId.Value, token });
                }

                TempData["AssessmentError"] = "This assessment can no longer be taken. Please pay again to start afresh.";
                return RedirectToAction(nameof(Assessment));
            }

            ViewBag.AccessToken = token;
            return View(attempt);
        }

        [HttpPost]
        public async Task<JsonResult> SubmitPublicAssessment(int attemptId, Guid token, [FromBody] CbtSubmitRequest request)
        {
            try
            {
                var answers = request?.Answers?.Select(a => new Core.ViewModels.CbtStudentAnswerViewModel
                {
                    QuestionId = a.QuestionId,
                    SelectedAnswer = a.SelectedAnswer
                }).ToList() ?? new List<Core.ViewModels.CbtStudentAnswerViewModel>();

                var result = await _cbtHelper.SubmitPublicAttemptAsync(
                    attemptId, token, answers, request?.AutoSubmitted ?? false);
                return Json(new { isError = !result.Success, msg = result.Message, attemptId });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult AssessmentResult(int id, Guid token)
        {
            var result = _cbtHelper.GetPublicAttemptResult(id, token);
            if (result == null)
            {
                TempData["AssessmentError"] = "Result not found.";
                return RedirectToAction(nameof(Assessment));
            }

            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}