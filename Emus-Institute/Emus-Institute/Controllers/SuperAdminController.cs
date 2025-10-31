using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Data;

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
        public SuperAdminController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserHelper userHelper, ISuperAdminHelper superAdminHelper)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userManager;
            _userHelper = userHelper;
            _superAdminHelper = superAdminHelper;
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
            var model = new ApplicationUserViewModel()
            {
                TotalStaff = staff,
                TotalApprovedStudents = approvedStudents,
                TotalDepartments = departments,
                TotalRegisteredStudents = registeredStudents,
                TotalPaidStudents = paidStudents,
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

    }
}
