using Core.ViewModels;
using static Core.DB.ECollegeEnums;

namespace Logic.IHelpers
{
    public interface IAnnouncementHelper
    {
        Task<(bool Success, string Message)> CreateAnnouncementAsync(
            string staffUserId,
            string title,
            string message,
            AnnouncementAudience audience,
            int? departmentId,
            DateTime startDateTime,
            DateTime endDateTime);
        List<AnnouncementViewModel> GetStaffAnnouncements(string staffUserId);
        List<AnnouncementViewModel> GetStudentAnnouncements(string studentUserId);
        int GetActiveAnnouncementCountForStudent(string studentUserId);
        int GetActiveWholeStudentAnnouncementCount();
    }
}
