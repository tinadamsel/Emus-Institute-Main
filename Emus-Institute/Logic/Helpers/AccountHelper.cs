using Core.Models;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;

namespace Logic.Helpers
{
    public class AccountHelper : IAccountHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountHelper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public void CreateSuperAdminAccount()
        {
            var superUser = _userManager.GetUsersInRoleAsync("SuperAdmin").Result;
            if (!superUser.Any())
            {
                var unknown = new ApplicationUser()
                {
                    FirstName = "Super",
                    LastName  = "Admin",
                    Email = "info@emusinstitute.com",
                    UserName = "info@emusinstitute.com",
                    DateRegistered = DateTime.Now,
                    DateModified = DateTime.Now,
                    Deactivated = false,
                };
                var pass = "5DD4C8A8EB0E9937D";
                var createdAccount = _userManager.CreateAsync(unknown, pass).Result;
                if (createdAccount.Succeeded)
                {
                    var endhere = _userManager.AddToRoleAsync(unknown, "SuperAdmin").Result;
                }
            }
        }
    }
}
