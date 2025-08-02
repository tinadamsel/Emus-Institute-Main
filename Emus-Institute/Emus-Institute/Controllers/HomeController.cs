
using Emus_Institute.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace e_college.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ISuperAdminHelper _superAdminHelper;

		public HomeController(ILogger<HomeController> logger, ISuperAdminHelper superAdminHelper)
		{
			_logger = logger;
			_superAdminHelper = superAdminHelper;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}