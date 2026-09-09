using Core.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Logic.IHelpers
{
    public interface ICbtHelper
    {
        List<CbtTestViewModel> GetStaffCbtTests(string userId, string? search = null, string? filter = null);
        CbtTestViewModel? GetStaffCbtTest(int testId, string userId);
        Task<(bool Success, string Message, int? TestId)> CreateCbtTestAsync(CbtTestViewModel model, string staffUserId);
        Task<(bool Success, string Message)> UpdateCbtTestAsync(CbtTestViewModel model, string staffUserId);
        (bool Success, string Message) DeleteCbtTest(int testId, string staffUserId);
        bool CanEditTest(int testId);
        List<CbtQuestionViewModel> GetQuestionsForTest(int testId, string userId, bool forTaking = false, int? seed = null);
        Task<(bool Success, string Message)> SaveQuestionAsync(CbtQuestionViewModel model, IFormFile? imageFile, string staffUserId, string webRootPath, int? questionId = null);
        (bool Success, string Message) DeleteQuestion(int questionId, string staffUserId);
        List<CbtAttemptViewModel> GetTestScores(int testId, string staffUserId);
        CbtAnalyticsViewModel? GetTestAnalytics(int testId, string staffUserId);
        CbtStudentTestsViewModel GetStudentCbtTests(string userId);
        (bool Success, string Message, int? AttemptId) StartAttempt(int testId, string userId, string? browserCode);
        CbtAttemptViewModel? GetAttemptForTaking(int attemptId, string userId);
        Task<(bool Success, string Message, CbtAttemptViewModel? Result)> SubmitAttemptAsync(int attemptId, string userId, List<CbtStudentAnswerViewModel> answers, bool autoSubmitted);
        CbtAttemptViewModel? GetAttemptResult(int attemptId, string userId, bool isStaff = false);

        bool HasPublishedPublicAssessment();
        CbtTestViewModel? GetPublishedPublicAssessment();
        List<CbtTestViewModel> GetPublicAssessments(string? search = null, string? filter = null);
        CbtTestViewModel? GetPublicAssessment(int testId);
        Task<(bool Success, string Message, int? TestId)> CreatePublicAssessmentAsync(CbtTestViewModel model, string adminUserId);
        Task<(bool Success, string Message)> UpdatePublicAssessmentAsync(CbtTestViewModel model, string adminUserId);
        (bool Success, string Message) DeletePublicAssessment(int testId);
        List<CbtQuestionViewModel> GetPublicQuestions(int testId);
        Task<(bool Success, string Message)> SavePublicQuestionAsync(CbtQuestionViewModel model, IFormFile? imageFile, string webRootPath, int? questionId = null);
        (bool Success, string Message) DeletePublicQuestion(int questionId);
        List<CbtAttemptViewModel> GetPublicTestScores(int testId);
        CbtAnalyticsViewModel? GetPublicTestAnalytics(int testId);
        (bool Success, string Message, int? AttemptId) StartPublicAttempt(int registrationId);
        CbtAttemptViewModel? GetPublicAttemptForTaking(int attemptId, Guid accessToken);
        Task<(bool Success, string Message, CbtAttemptViewModel? Result)> SubmitPublicAttemptAsync(int attemptId, Guid accessToken, List<CbtStudentAnswerViewModel> answers, bool autoSubmitted);
        CbtAttemptViewModel? GetPublicAttemptResult(int attemptId, Guid accessToken);
    }
}
