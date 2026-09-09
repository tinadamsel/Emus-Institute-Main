using Core.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Logic.IHelpers
{
    public interface IStudyCenterHelper
    {
        Task<(bool Success, string Message)> SubmitStudyCenterAsync(
            string name,
            string address,
            string country,
            string contactPerson,
            IFormFile imageFile,
            string webRootPath);

        List<StudyCenterViewModel> GetApprovedStudyCenters();
        List<StudyCenterViewModel> GetPendingStudyCenters();
        List<StudyCenterViewModel> GetApprovedStudyCentersForAdmin();
        bool ApproveStudyCenter(int id, string superAdminUserId);
        bool DeclineStudyCenter(int id);
    }
}
