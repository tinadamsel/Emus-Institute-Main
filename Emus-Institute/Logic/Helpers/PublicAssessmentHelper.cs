using Core.Config;
using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class PublicAssessmentHelper : IPublicAssessmentHelper
    {
        private readonly AppDbContext _context;
        private readonly ICbtHelper _cbtHelper;
        private readonly IPaymentHelper _paymentHelper;
        private readonly IPaystackHelper _paystackHelper;
        private readonly IGeneralConfiguration _generalConfiguration;

        public PublicAssessmentHelper(
            AppDbContext context,
            ICbtHelper cbtHelper,
            IPaymentHelper paymentHelper,
            IPaystackHelper paystackHelper,
            IGeneralConfiguration generalConfiguration)
        {
            _context = context;
            _cbtHelper = cbtHelper;
            _paymentHelper = paymentHelper;
            _paystackHelper = paystackHelper;
            _generalConfiguration = generalConfiguration;
        }

        public bool HasPublishedPublicAssessment() => _cbtHelper.HasPublishedPublicAssessment();

        public async Task<(bool Success, string Message, string? AuthorizationUrl)> SubmitRegistrationAndPayAsync(
            string email,
            int programType,
            string country,
            int scholarshipType)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                return (false, "Please enter a valid email address.", null);
            }

            if (!Enum.IsDefined(typeof(AssessmentProgramType), programType))
            {
                return (false, "Please select Undergraduate or Graduate program.", null);
            }

            if (string.IsNullOrWhiteSpace(country))
            {
                return (false, "Please enter your country.", null);
            }

            if (!Enum.IsDefined(typeof(AssessmentScholarshipType), scholarshipType))
            {
                return (false, "Please select Partial or Full scholarship.", null);
            }

            var test = _cbtHelper.GetPublishedPublicAssessment();
            if (test == null || test.QuestionCount < 1)
            {
                return (false, "There is no published assessment available at this time.", null);
            }

            var registration = new AssessmentRegistration
            {
                Email = email.Trim(),
                ProgramType = (AssessmentProgramType)programType,
                Country = country.Trim(),
                ScholarshipType = (AssessmentScholarshipType)scholarshipType,
                CbtTestId = test.Id,
                AccessToken = Guid.NewGuid(),
                IsPaid = false,
                HasOpenedQuiz = false,
                DateCreated = DateTime.Now
            };

            _context.AssessmentRegistrations.Add(registration);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            // Charge in NGN like other Emus Paystack flows; UI still presents the fee as £6.
            var amount = _generalConfiguration.AssessmentAmountNgn > 0
                ? _generalConfiguration.AssessmentAmountNgn
                : 12000m;

            var paystackResponse = await _paymentHelper.CreateAssessmentPayment(registration, amount).ConfigureAwait(false);
            if (paystackResponse?.data?.authorization_url == null)
            {
                var paystackMsg = paystackResponse?.message;
                var msg = string.IsNullOrWhiteSpace(paystackMsg)
                    ? "Unable to start payment. Please try again or contact admin."
                    : $"Unable to start payment: {paystackMsg}";
                return (false, msg, null);
            }

            return (true, "Form submitted. You will now be redirected to pay £6 for the assessment.", paystackResponse.data.authorization_url);
        }

        public async Task<(bool Success, string Message, Guid? AccessToken)> CompleteAssessmentPaymentAsync(PayStack paystack)
        {
            var response = await _paystackHelper.VerifyAssessmentPayment(paystack).ConfigureAwait(false);
            if (response?.data == null)
            {
                return (false, "Unable to verify payment.", null);
            }

            var registration = await _context.AssessmentRegistrations
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Payment != null && x.Payment.Reference == paystack.Reference)
                .ConfigureAwait(false);

            if (registration == null && !string.IsNullOrWhiteSpace(response.data.reference))
            {
                var paystackRow = await _context.PayStackpayments
                    .FirstOrDefaultAsync(x => x.Reference == response.data.reference)
                    .ConfigureAwait(false);
                if (paystackRow != null)
                {
                    registration = await _context.AssessmentRegistrations
                        .FirstOrDefaultAsync(x => x.PaymentId == paystackRow.PaymentId)
                        .ConfigureAwait(false);
                }
            }

            if (registration == null || !registration.IsPaid)
            {
                return (false, "Payment was not completed successfully.", null);
            }

            return (true, "Payment verified.", registration.AccessToken);
        }

        public (bool Success, string Message, int? AttemptId) OpenPaidAssessment(Guid accessToken)
        {
            var registration = _context.AssessmentRegistrations
                .FirstOrDefault(x => x.AccessToken == accessToken);

            if (registration == null || !registration.IsPaid)
            {
                return (false, "No paid assessment was found for this session. Please complete payment first.", null);
            }

            return _cbtHelper.StartPublicAttempt(registration.Id);
        }
    }
}
