using Core.ViewModels;

namespace Logic.IHelpers
{
    public interface ILiveSessionHelper
    {
        List<LiveSessionViewModel> GetStaffDepartmentLiveSessions(string userId);
        List<LiveSessionViewModel> GetStudentLiveSessions(string userId);
        LiveSessionViewModel? GetLiveSessionForJoin(int sessionId, string userId, bool isStaff);
        Task<(bool Success, string Message)> CreateLiveSessionAsync(LiveSessionViewModel model, string staffUserId);
        bool HasDepartmentScheduleConflict(int departmentId, DateTime startDateTime, DateTime endDateTime, int? excludeSessionId = null);
    }
}
