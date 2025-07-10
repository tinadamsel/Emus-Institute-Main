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

        //[HttpGet]
        //public IActionResult PendingApplication()
        //{
        //    var pendingApp = _userHelper.GetPendingApplications();
        //    return View(pendingApp);
        //}
        //public JsonResult GetCoverLetter(int Id)
        //{
        //    if (Id > 0)
        //    {
        //        var coverLetter = _userHelper.GetCoverLetter(Id);
        //        if (coverLetter != null)
        //        {
        //            return Json(new {isError = false, data = coverLetter});
        //        }
        //        return Json(new { isError = true, msg = "Unable To Get Cover Letter" });
        //    }
        //    return Json(new { isError = true, msg = "Network Error" });
        //}

        //public JsonResult ApproveApplication(int id)
        //{
        //    try
        //    {
        //        if (id > 0)
        //        {
        //            //var user = _userHelper.GetCurrentUserId(User.Identity.Name);
        //            var checkifApprovedBefore = _userHelper.CheckIfApproved(id);
        //            if (checkifApprovedBefore)
        //            {
        //                return Json(new { isError = true, msg = "This application has been approved before" });
        //            }
        //            var approve = _userHelper.ApproveApplication(id);
        //            if (approve)
        //            {
        //                return Json(new { isError = false, msg = "Application has been approved successfully" });
        //            }
        //            return Json(new { isError = true, msg = "Could not approve" });
        //        }
        //        return Json(new { isError = true, msg = "Network Failure" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { isError = true, msg = ex.Message });
        //    }
        //}
        //[HttpPost]
        //public JsonResult DeclineApplication(int id)
        //{
        //    try
        //    {
        //        if (id > 0)
        //        {
        //            var checkifApprovedBefore = _userHelper.CheckIfApproved(id);
        //            if (checkifApprovedBefore)
        //            {
        //                return Json(new { isError = true, msg = "This application has been approved before" });
        //            }
        //            var checkifRejectedBefore = _userHelper.CheckIfDeclined(id);
        //            if (checkifRejectedBefore)
        //            {
        //                return Json(new { isError = true, msg = "This token payment has been declined before" });
        //            }
        //            var rejectApplication = _userHelper.RejectApplication(id);
        //            if (rejectApplication)
        //            {
        //                return Json(new { isError = false, msg = " Application declined" });
        //            }
        //            return Json(new { isError = true, msg = "Error occured while rejecting, try again." });

        //        }
        //        return Json(new { isError = true, msg = " No Application Request Found" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { isError = true, msg = ex.Message });
        //    }
        //}




    }
}
