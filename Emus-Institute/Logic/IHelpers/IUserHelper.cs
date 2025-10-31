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
        //Task<bool> RegStaff(ApplicationUserViewModel userDetails, string staffPosition, string appLetter, string validId);
        string GetUserRole(string userId);

        bool CheckIfStaffIsApproved(string email);
        //List<StaffDocumentationViewModel> GetPendingApplications();
        //StaffDocumentation GetCoverLetter(int id);
        bool CheckIfApproved(int id);
        bool CheckIfDeclined(int id);
        //bool ApproveApplication(int id);
        //bool RejectApplication(int id);
        //int GetTotalAcademicStaff();
        //int GetTotalNonAcademicStaff();

        //this 1 not included:
        //Task<bool> RegisterUniStudent(ApplicationUserViewModel userDetails, string edulevel);
        string GetCurrentUserId(string username);
        Task<bool> RegisterStudent(ApplicationUserViewModel userDetails, string linkToClick);
        Task<EvaluationDetails> SaveStudentEvaluationDetails(string UserId, string passport, string transcript, string highSchCert, string waecScratchCard, string anyRelevantCert);
        bool SendPaymentCompletionEmail(string email);
        Task<ApplicationUser> FindByUserNameAsync(string username);
        ApplicationUser GetStudentDetails(string userId);
    }
}
