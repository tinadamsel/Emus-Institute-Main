using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Logic.IHelpers
{
    public interface IAssignmentHelper
    {
        List<AssignmentViewModel> GetStaffDepartmentAssignments(string staffUserId);
        List<AssignmentViewModel> GetStudentAssignments(string studentUserId);
        AssignmentViewModel? GetStaffAssignment(int assignmentId, string staffUserId);
        AssignmentViewModel? GetStudentAssignment(int assignmentId, string studentUserId);
        AssignmentSubmissionsPageViewModel? GetAssignmentSubmissions(int assignmentId, string staffUserId);
        Task<(bool Success, string Message)> CreateAssignmentAsync(
            AssignmentViewModel model, IFormFile? file, string staffUserId, string webRootPath);
        Task<(bool Success, string Message)> SubmitAssignmentAsync(
            int assignmentId, string? comment, IFormFile? file, string studentUserId, string webRootPath);
        Task<(bool Success, string Message)> GradeSubmissionAsync(
            int submissionId, decimal score, string? feedback, string staffUserId);
        Assignment? GetAssignmentFileForDownload(int assignmentId, string userId, bool isStaff);
        AssignmentSubmission? GetSubmissionFileForDownload(int submissionId, string userId, bool isStaff);
        int GetStaffAssignmentCount(string staffUserId);
        int GetStudentAssignmentCount(string studentUserId);
    }
}
