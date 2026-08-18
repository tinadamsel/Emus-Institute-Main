using Core.Config;
using Core.DB;
using Core.Models;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logic.Services
{
    public class EvaluationReminderService : IEvaluationReminderService
    {
        private const int MaxEvaluationReminderSends = 4;

        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly ILogger<EvaluationReminderService> _logger;

        public EvaluationReminderService(
            AppDbContext context,
            IEmailService emailService,
            IGeneralConfiguration generalConfiguration,
            ILogger<EvaluationReminderService> logger)
        {
            _context = context;
            _emailService = emailService;
            _generalConfiguration = generalConfiguration;
            _logger = logger;
        }

        public async Task<bool> HasReachedEvaluationReminderLimitAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            var sentCount = await _context.ApplicationUser
                .Where(u => u.Email == email)
                .Select(u => (int?)u.EvaluationReminderSentCount)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return sentCount >= MaxEvaluationReminderSends;
        }

        public async Task SendWeeklyEvaluationRemindersAsync()
        {
            var siteBaseUrl = (_generalConfiguration.SiteBaseUrl ?? "https://localhost:44329").TrimEnd('/');

            var studentsSent = await SendStudentRemindersAsync(siteBaseUrl).ConfigureAwait(false);
            var staffSent = await SendStaffRemindersAsync(siteBaseUrl).ConfigureAwait(false);

            _logger.LogInformation(
                "Weekly evaluation reminders sent. Students: {StudentCount}, Staff: {StaffCount}",
                studentsSent,
                staffSent);
        }

        private async Task<int> SendStudentRemindersAsync(string siteBaseUrl)
        {
            var students = await _context.ApplicationUser
                .Include(u => u.Department)
                .Where(u => !u.Deactivated
                    && !u.IsAdmin
                    && !u.Paid
                    && !u.IsStudent
                    && u.Email != null
                    && u.StudentId != null
                    && u.EvaluationReminderSentCount < MaxEvaluationReminderSends
                    && (u.Department == null || !u.Department.IsUnderScholarship))
                .ToListAsync()
                .ConfigureAwait(false);

            var sent = 0;
            foreach (var student in students)
            {
                if (await HasReachedEvaluationReminderLimitAsync(student.Email!).ConfigureAwait(false))
                {
                    continue;
                }

                var evaluationUrl = $"{siteBaseUrl}/Account/EvaluateCredentials?userId={student.Id}";
                var subject = "A Gentle Reminder: Complete Your Credential Evaluation";
                var message =
                    $"Dear <b>{student.FirstName}</b>,<br/><br/>" +
                    "We hope this message finds you well. We noticed that you have registered with Emus Institute, " +
                    "but your credential evaluation is not yet complete.<br/><br/>" +
                    "Completing your evaluation is a very important step for your application to be reviewed and accepted. " +
                    "It only takes a short time to upload your documents and finish the process.<br/><br/>" +
                    "Whenever you are ready, please use the link below to continue:<br/>" +
                    $"<a href='{evaluationUrl}' target='_blank'>" +
                    "<button style='color:white; background-color:#06BBCC; padding:12px; border:1px solid #06BBCC;'>Complete My Evaluation</button></a>" +
                    "<br/><br/>If you have already started, thank you — you may simply return to the page above to finish. " +
                    "If you need help, our team will be happy to assist you.<br/><br/>" +
                    "Warm regards,<br/>Emus Institute Team";

                if (await TrySendAndRecordReminderAsync(student, subject, message).ConfigureAwait(false))
                {
                    sent++;
                }
            }

            return sent;
        }

        private async Task<int> SendStaffRemindersAsync(string siteBaseUrl)
        {
            var staffMembers = await _context.ApplicationUser
                .Where(u => !u.Deactivated
                    && u.IsAdmin
                    && !u.Paid
                    && u.Email != null
                    && u.EvaluationReminderSentCount < MaxEvaluationReminderSends)
                .ToListAsync()
                .ConfigureAwait(false);

            var sent = 0;
            foreach (var staff in staffMembers)
            {
                if (await HasReachedEvaluationReminderLimitAsync(staff.Email!).ConfigureAwait(false))
                {
                    continue;
                }

                var evaluationUrl = $"{siteBaseUrl}/AcademicStaff/EvaluateCredentials?userId={staff.Id}";
                var subject = "A Gentle Reminder: Complete Your Staff Credential Evaluation";
                var message =
                    $"Dear <b>{staff.FirstName}</b>,<br/><br/>" +
                    " We hope this message finds you well. We noticed that you have applied to Emus Institute," +
                    " but your credential evaluation is not yet complete. <br/><br/>" +
                    "Completing your evaluation is a very important step for your application to be reviewed and accepted. It only takes a shortime to upload your documents and finish the process." +
                    "Whenever you are ready please use the link below to continue: <br/><br/>" +
                    
                    $"<a href='{evaluationUrl}' target='_blank'>" +
                    "<button style='color:white; background-color:#06BBCC; padding:12px; border:1px solid #06BBCC;'>Complete My Evaluation</button></a>" +
                    "<br/><br/>If you have already started, thank you - you may simply return to the page above to finish. If you need help, our team will be happy to assist you.<br/><br/>" +

                    "<br/> Please Note: If you have a Statement of Comparability, you no longer need to pay for the evaluation fee as this is required for all Staff seeking " +
                    "to work in UK with any degree obtained outside UK. <br/>" +
                    "A Statement of Comparability is a certificate that shows how international qualifications compare to the UK education systems. The UK's education systems include England, Scotland, Wales and Northern Ireland " +
                    "The Statement gives international qualifications context when applying for jobs, studies or professional registration in the UK. <br/>" +
                    "Show the Statement of Comparability with your original qualification to HR department by emailing the evidence to hr@emusinstitute.com. <br/>" +
                    "Once comfirmed, your application will be accepted. <br/>" +
                    "If you do not have it, request for an assistance from the HR. Your application will be approved but we shall pay for your Comparability Certificate on your behalf and deduct it from your first salary with us. <br/><br/>" +
                    "Warm regards,<br/>Emus Institute Team";

                if (await TrySendAndRecordReminderAsync(staff, subject, message).ConfigureAwait(false))
                {
                    sent++;
                }
            }

            return sent;
        }

        private async Task<bool> TrySendAndRecordReminderAsync(ApplicationUser user, string subject, string message)
        {
            if (user.Email == null || user.EvaluationReminderSentCount >= MaxEvaluationReminderSends)
            {
                return false;
            }

            if (!_emailService.SendEmail(user.Email, subject, message))
            {
                return false;
            }

            user.EvaluationReminderSentCount++;
            await _context.SaveChangesAsync().ConfigureAwait(false);

            if (user.EvaluationReminderSentCount >= MaxEvaluationReminderSends)
            {
                _logger.LogInformation(
                    "Evaluation reminder limit reached for {Email}. No further reminders will be sent.",
                    user.Email);
            }

            return true;
        }
    }
}
