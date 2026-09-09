using Core.Models;

namespace Logic.IHelpers
{
    public interface IPublicAssessmentHelper
    {
        bool HasPublishedPublicAssessment();
        Task<(bool Success, string Message, string? AuthorizationUrl)> SubmitRegistrationAndPayAsync(
            string email,
            int programType,
            string country,
            int scholarshipType);
        Task<(bool Success, string Message, Guid? AccessToken)> CompleteAssessmentPaymentAsync(PayStack paystack);
        (bool Success, string Message, int? AttemptId) OpenPaidAssessment(Guid accessToken);
    }
}
