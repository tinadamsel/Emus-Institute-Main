using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IUserHelper
    {
        bool CheckIfUserIsStudent(string email);
        bool CheckForUserName(string userName);
        bool CheckIfUserIsSuspended(string email);
        Task<ApplicationUser> FindByEmailAsync(string email);
        ApplicationUser FindById(string Id);
        ApplicationUser FindByUserName(string username);
       
        string GetUserId(string username);
        Task<bool> RegStaff(ApplicationUserViewModel userDetails, string staffPosition, string appLetter, string validId, string resume, string linkToClick);
        string GetUserRole(string userId);

        bool CheckIfStaffIsApproved(string email);
       
        bool CheckIfApproved(int id);
        bool CheckIfDeclined(int id);
         string GetCurrentUserId(string username);
        Task<bool> RegisterStudent(ApplicationUserViewModel userDetails, string linkToClick, string refLink);
        Task<EvaluationDetails> SaveStudentEvaluationDetails(string UserId, string passport, string transcript, string highSchCert, string waecScratchCard, string anyRelevantCert);
        Task<StaffEvaluationDetails> SaveStaffEvaluationDetails(string userId, string passport, string transcript, string highSchCert, string waecScratchCard, string anyRelevantCert);
        bool SendPaymentCompletionEmail(string email);
        bool SendStaffPaymentCompletionEmail(string email);
        bool CheckIfStaffHasPaid(string email);
        Task<ApplicationUser> FindByUserNameAsync(string username);
        ApplicationUser GetStudentDetails(string userId);
        StaffDocumentation GetCoverLetter(int id);
        bool CheckIfSuspended(string userId);
       
    }
}
