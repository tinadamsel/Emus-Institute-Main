using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Data;
using static Core.DB.ECollegeEnums;

namespace e_college.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly ISuperAdminHelper _superAdminHelper;
        private readonly IStudyCenterHelper _studyCenterHelper;
        private readonly ICbtHelper _cbtHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SuperAdminController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserHelper userHelper, ISuperAdminHelper superAdminHelper, IStudyCenterHelper studyCenterHelper, ICbtHelper cbtHelper, IWebHostEnvironment webHostEnvironment)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
            _superAdminHelper = superAdminHelper;
            _studyCenterHelper = studyCenterHelper;
            _cbtHelper = cbtHelper;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var user = _userHelper.GetCurrentUserId(User?.Identity?.Name);
            var approvedStudents = _superAdminHelper.GetTotalApprovedStudents();
            var paidStudents = _superAdminHelper.GetTotalPaidStudents();
            var registeredStudents = _superAdminHelper.GetTotalRegisteredStudents();
            var staff = _superAdminHelper.GetTotalStaff();
            var departments = _superAdminHelper.GetTotalDepartments();
            var suspendedUsers = _superAdminHelper.GetTotalSuspendedUsers();
            var model = new ApplicationUserViewModel()
            {
                TotalStaff = staff,
                TotalApprovedStudents = approvedStudents,
                TotalDepartments = departments,
                TotalRegisteredStudents = registeredStudents,
                TotalPaidStudents = paidStudents,
                TotalSuspendedUsers = suspendedUsers,
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult Departments()
        {
            var deptDetails = _superAdminHelper.GetDepartments();
            return View(deptDetails);

        }
        public JsonResult CreateDepartment(string deptDetails)
        {
            if (deptDetails != null)
            {
                var deptViewModel = JsonConvert.DeserializeObject<DepartmentViewModel>(deptDetails);
                if (deptViewModel != null)
                {
                    var checkDeptName = _superAdminHelper.CheckExistingDeptName(deptViewModel?.Name);
                    if (!checkDeptName)
                    {
                        var department = _superAdminHelper.CreateDepartment(deptViewModel);
                        if (department)
                        {
                            return Json(new { isError = false, msg = "Department Created Successfully" });
                        }
                        return Json(new { isError = true, msg = "Unable to Create" });
                    }
                    return Json(new { isError = true, msg = "Department Name Already Exists" });
                }
            }
            return Json(new { isError = true, msg = "Network Failure" });
        }

        public JsonResult EditDepartment(int Id)
        {
            if (Id > 0)
            {
                var deptToEdit = _superAdminHelper.GetDeptToEdit(Id);
                if (deptToEdit != null)
                {
                    return Json(deptToEdit);
                }
                return Json(new { isError = true, msg = "Unable To Get Subject" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        public JsonResult EditedDepartment(string editDept)
        {
            if (editDept != null)
            {
                var departmentViewModel = JsonConvert.DeserializeObject<DepartmentViewModel>(editDept);
                if (departmentViewModel != null)
                {
                    var editDepartment = _superAdminHelper.SaveEditedDept(departmentViewModel);
                    if (editDepartment)
                    {
                        return Json(new { isError = false, msg = "Department Edited Successfully" });
                    }
                    return Json(new { isError = true, msg = "Unable to Edit" });
                }
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        public JsonResult DeleteDepartment(int id)
        {
            if (id > 0)
            {
                var deleteDept = _superAdminHelper.DeleteDept(id);
                if (deleteDept)
                {
                    return Json(new { isError = false, msg = "Department Deleted successfully" });
                }
                return Json(new { isError = true, msg = "Unable To Delete Department" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        //public JsonResult TestToEdit(int id)
        //{
        //    ViewBag.Layout = _userHelper.GetRoleLayout();
        //    if (id > 0)
        //    {
        //        var testToEdit = _userHelper.GetTestToEdit(id);
        //        if (testToEdit != null)
        //        {
        //            ViewBag.Specimens = _dropdownHelper.DropdownOfSpecimen();
        //            return Json(new { isError = false, data = testToEdit });
        //        }
        //    }
        //    return Json(new { isError = true, msg = "Unable To Get test" });
        //}

        public IActionResult RegisteredStudents()
        {
            var regStudents = _superAdminHelper.GetAllRegisteredStudents();
            return View(regStudents);

        }
        public IActionResult ApprovedStudents()
        {
            var approvedStudents = _superAdminHelper.GetAllApprovedStudents();
            return View(approvedStudents);

        }

        public IActionResult PaidStudents()
        {
            var approvedStudents = _superAdminHelper.GetAllPaidStudents();
            return View(approvedStudents);

        }

        public IActionResult ScholarshipStudents()
        {
            var scholarshipStudents = _superAdminHelper.GetScholarshipStudents();
            return View(scholarshipStudents);
        }

        public IActionResult ApprovedScholarshipStudents()
        {
            var approvedScholarshipStudents = _superAdminHelper.GetApprovedScholarshipStudents();
            return View(approvedScholarshipStudents);
        }

        public JsonResult ScholarshipRegistrationLink()
        {
            var link = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/Account/ScholarshipRegistration";
            return Json(link);
        }

        public JsonResult ScholarshipStudentApproval(string userId)
        {
            try
            {
                if (userId != null)
                {
                    if (_superAdminHelper.CheckIfScholarshipStudentIsApproved(userId))
                    {
                        return Json(new { isError = true, msg = "This scholarship student has been approved before" });
                    }
                    var approveStudent = _superAdminHelper.ApproveScholarshipStudent(userId);
                    if (approveStudent)
                    {
                        return Json(new { isError = false, msg = "Scholarship student has been approved successfully" });
                    }
                    return Json(new { isError = true, msg = "Could not approve scholarship student" });
                }
                return Json(new { isError = true, msg = "Network Failure" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        public JsonResult DeclineScholarshipStudent(string userId)
        {
            try
            {
                if (userId != null)
                {
                    var declineStudent = _superAdminHelper.DeclineScholarshipStudent(userId);
                    if (declineStudent)
                    {
                        return Json(new { isError = false, msg = "Scholarship student has been declined" });
                    }
                    return Json(new { isError = true, msg = "Could not decline scholarship student" });
                }
                return Json(new { isError = true, msg = "Network Failure" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        public JsonResult StudentApproval(string userId)
        {
            try
            {
                if (userId != null)
                {
                    //var user = _userHelper.GetCurrentUserId(User.Identity.Name);
                    var checkifStudentIsApproved = _superAdminHelper.CheckIfStudentIsApproved(userId);
                    if (checkifStudentIsApproved)
                    {
                        return Json(new { isError = true, msg = "This student has been approved before" });
                    }
                    var approveStudent = _superAdminHelper.ApproveStudent(userId);
                    if (approveStudent)
                    {
                        return Json(new { isError = false, msg = "Student has been approved successfully" });
                    }
                    return Json(new { isError = true, msg = "Could not approve" });
                }
                return Json(new { isError = true, msg = "Network Failure" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        public JsonResult DeclineStudent(string userId)
        {
            try
            {
                if (userId != null)
                {
                    var declineStudent = _superAdminHelper.DeclineStudent(userId);
                    if (declineStudent)
                    {
                        return Json(new { isError = false, msg = "Student has been declined" });
                    }
                    return Json(new { isError = true, msg = "Could not decline" });
                }
                return Json(new { isError = true, msg = "Network Failure" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult PendingApplication()
        {
            var pendingApp = _superAdminHelper.GetPendingApplications();
            return View(pendingApp);
        }

        public JsonResult ApproveApplication(int id)
        {
            try
            {
                if (id > 0)
                {
                    //var user = _userHelper.GetCurrentUserId(User.Identity.Name);
                    var checkifApprovedBefore = _userHelper.CheckIfApproved(id);
                    if (checkifApprovedBefore)
                    {
                        return Json(new { isError = true, msg = "This application has been approved before" });
                    }
                    var approve = _superAdminHelper.ApproveApplication(id);
                    if (approve)
                    {
                        return Json(new { isError = false, msg = "Application has been approved successfully" });
                    }
                    return Json(new { isError = true, msg = "Could not approve" });
                }
                return Json(new { isError = true, msg = "Network Failure" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult DeclineApplication(int id)
        {
            try
            {
                if (id > 0)
                {
                    var checkifApprovedBefore = _userHelper.CheckIfApproved(id);
                    if (checkifApprovedBefore)
                    {
                        return Json(new { isError = true, msg = "This application has been approved before" });
                    }
                    var checkifRejectedBefore = _userHelper.CheckIfDeclined(id);
                    if (checkifRejectedBefore)
                    {
                        return Json(new { isError = true, msg = "This token payment has been declined before" });
                    }
                    var rejectApplication = _superAdminHelper.RejectApplication(id);
                    if (rejectApplication)
                    {
                        return Json(new { isError = false, msg = " Application declined" });
                    }
                    return Json(new { isError = true, msg = "Error occured while rejecting, try again." });

                }
                return Json(new { isError = true, msg = " No Application Request Found" });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ApprovedStaff()
        {
            var pendingApp = _superAdminHelper.GetApprovedStaff();
            ViewBag.Departments = _superAdminHelper.GetDepartments();
            return View(pendingApp);
        }

        [HttpPost]
        public async Task<JsonResult> ReassignStaffRole(int staffDocumentId, int newStaffPosition, int? departmentId)
        {
            try
            {
                var result = await _superAdminHelper.ReassignStaffRoleAsync(staffDocumentId, newStaffPosition, departmentId);
                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SuspendUser(string userId)
        {
            if (userId != null)
            {
                //check if user is susspended 
                var checkifSuspended = _userHelper.CheckIfSuspended(userId);
                if (checkifSuspended)
                {
                    return Json(new { isError = true, msg = "This user is already suspended" });
                }
                var suspendStaff = _superAdminHelper.SuspendUser(userId);
                if (suspendStaff)
                {
                    return Json(new { isError = false, msg = "User Suspended" });
                }
                return Json(new { isError = true, msg = "Unable To Suspend" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        [HttpPost]
        public JsonResult DeactivateUser(string userId)
        {
            if (userId != null)
            {
                var deactivateUser = _superAdminHelper.DeactivateUser(userId);
                if (deactivateUser)
                {
                    return Json(new { isError = false, msg = "User Deactivated" });
                }
                return Json(new { isError = true, msg = "Unable To Deactivate" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        [HttpGet]
        public IActionResult SuspendedUsers()
        {
            var suspendedUsers = _superAdminHelper.GetSuspendedUsers();
            return View(suspendedUsers);
        }

        [HttpPost]
        public JsonResult RemoveUserFromSuspension(int id)
        {
            if (id > 0)
            {
                //check if user is removed
                var removeSuspension = _superAdminHelper.RemoveSuspension(id);
                if (removeSuspension)
                {
                    return Json(new { isError = false, msg = "User Removed From Suspension" });
                }
                return Json(new { isError = true, msg = "Unable To Remove" });
            }
            return Json(new { isError = true, msg = "Network Error" });
        }

        [HttpGet]
        public IActionResult Announcements()
        {
            return RedirectToAction("Announcements", "AcademicStaff");
        }

        [HttpGet]
        public IActionResult PendingStudyCenters()
        {
            var pendingCenters = _studyCenterHelper.GetPendingStudyCenters();
            return View(pendingCenters);
        }

        [HttpPost]
        public JsonResult ApproveStudyCenter(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { isError = true, msg = "Invalid study center." });
                }

                var userId = _userHelper.GetCurrentUserId(User?.Identity?.Name);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Json(new { isError = true, msg = "Unable to identify the current user." });
                }

                var approved = _studyCenterHelper.ApproveStudyCenter(id, userId);
                if (approved)
                {
                    return Json(new { isError = false, msg = "Study center approved successfully." });
                }

                return Json(new { isError = true, msg = "Could not approve study center." });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeclineStudyCenter(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { isError = true, msg = "Invalid study center." });
                }

                var declined = _studyCenterHelper.DeclineStudyCenter(id);
                if (declined)
                {
                    return Json(new { isError = false, msg = "Study center declined." });
                }

                return Json(new { isError = true, msg = "Could not decline study center." });
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult PublicAssessments(string? search, string? filter)
        {
            ViewBag.Search = search;
            ViewBag.Filter = filter ?? "all";
            return View(_cbtHelper.GetPublicAssessments(search, filter));
        }

        [HttpGet]
        public IActionResult CreatePublicAssessment()
        {
            return View(new CbtTestViewModel { MaximumAttempts = 1, MarkPerQuestion = 1, DurationMinutes = 30 });
        }

        [HttpPost]
        public async Task<JsonResult> CreatePublicAssessment(
            string title, string? description, int durationMinutes, string startDateTime, string endDateTime,
            decimal passMark, decimal markPerQuestion, bool shuffleQuestions, bool shuffleOptions,
            bool isPublished, string? instructions)
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
                    MaximumAttempts = 1,
                    ShuffleQuestions = shuffleQuestions,
                    ShuffleOptions = shuffleOptions,
                    IsPublished = isPublished,
                    Instructions = instructions
                };

                var result = await _cbtHelper.CreatePublicAssessmentAsync(model, currentUser.Id).ConfigureAwait(false);
                if (result.Success)
                    return Json(new { isError = false, msg = result.Message, testId = result.TestId });
                return Json(new { isError = true, msg = result.Message });
            }
            catch (Exception ex) { return Json(new { isError = true, msg = ex.Message }); }
        }

        [HttpGet]
        public IActionResult EditPublicAssessment(int id)
        {
            var test = _cbtHelper.GetPublicAssessment(id);
            if (test == null) return NotFound();
            if (!test.CanEdit) return RedirectToAction(nameof(PublicAssessmentAnalytics), new { id });
            return View(test);
        }

        [HttpPost]
        public async Task<JsonResult> UpdatePublicAssessment(
            int id, string title, string? description, int durationMinutes, string startDateTime, string endDateTime,
            decimal passMark, decimal markPerQuestion, bool shuffleQuestions, bool shuffleOptions,
            bool isPublished, string? instructions)
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
                    MaximumAttempts = 1,
                    ShuffleQuestions = shuffleQuestions,
                    ShuffleOptions = shuffleOptions,
                    IsPublished = isPublished,
                    Instructions = instructions
                };

                var result = await _cbtHelper.UpdatePublicAssessmentAsync(model, currentUser.Id).ConfigureAwait(false);
                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex) { return Json(new { isError = true, msg = ex.Message }); }
        }

        [HttpPost]
        public JsonResult DeletePublicAssessment(int id)
        {
            var result = _cbtHelper.DeletePublicAssessment(id);
            return Json(new { isError = !result.Success, msg = result.Message });
        }

        [HttpGet]
        public IActionResult ManagePublicAssessmentQuestions(int id)
        {
            var test = _cbtHelper.GetPublicAssessment(id);
            if (test == null) return NotFound();
            ViewBag.Test = test;
            ViewBag.TestId = test.Id;
            return View(_cbtHelper.GetPublicQuestions(id));
        }

        [HttpPost]
        public async Task<JsonResult> SavePublicCbtQuestion(
            int cbtTestId, int questionId, int questionType, string questionText,
            string? optionA, string? optionB, string? optionC, string? optionD,
            string correctAnswer, decimal marks, string? explanation, IFormFile? imageFile)
        {
            try
            {
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

                var result = await _cbtHelper.SavePublicQuestionAsync(
                    model, imageFile, _webHostEnvironment.WebRootPath,
                    questionId > 0 ? questionId : null).ConfigureAwait(false);
                return Json(new { isError = !result.Success, msg = result.Message });
            }
            catch (Exception ex) { return Json(new { isError = true, msg = ex.Message }); }
        }

        [HttpPost]
        public JsonResult DeletePublicCbtQuestion(int id)
        {
            var result = _cbtHelper.DeletePublicQuestion(id);
            return Json(new { isError = !result.Success, msg = result.Message });
        }

        [HttpGet]
        public IActionResult PublicAssessmentScores(int id)
        {
            var test = _cbtHelper.GetPublicAssessment(id);
            if (test == null) return NotFound();
            ViewBag.Test = test;
            ViewBag.TestId = test.Id;
            return View(_cbtHelper.GetPublicTestScores(id));
        }

        [HttpGet]
        public IActionResult PublicAssessmentAnalytics(int id)
        {
            var analytics = _cbtHelper.GetPublicTestAnalytics(id);
            if (analytics == null) return NotFound();
            ViewBag.TestId = id;
            return View(analytics);
        }

    }
}
