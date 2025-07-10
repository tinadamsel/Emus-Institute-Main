using Core.DB;
using Core.Models;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Helpers
{
    public class DropdownHelper : IDropDownHelper
    {
        private readonly AppDbContext _context;
        private readonly IUserHelper _userHelper;
        private UserManager<ApplicationUser> _userManager;
        public DropdownHelper(AppDbContext context, UserManager<ApplicationUser> userManager, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _userManager = userManager;
        }

        public List<Department> DropdownOfDepartments()
        {
            try
            {
                var common = new Department()
                {
                    Id = 0,
                    Name = "-- Select Department --"
                };
                var listOfDepartments = _context.Departments.Where(x => x.Id > 0 && x.Active).ToList();
                var drp = listOfDepartments.Select(x => new Department
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                drp.Insert(0, common);
                return drp;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }

    public class DropDown
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
