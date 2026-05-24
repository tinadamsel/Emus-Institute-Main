using Core.Config;
using Core.DB;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logic.Services
{
    public class EvaluationReminderService : IEvaluationReminderService
    {
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
                .Where(u => !u.Deactivated
                    && !u.IsAdmin
                    && !u.Paid
                    && !u.IsStudent
                    && u.Email != null
                    && u.StudentId != null)
                .ToListAsync()
                .ConfigureAwait(false);

            var sent = 0;
            foreach (var student in students)
            {
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

                if (_emailService.SendEmail(student.Email!, subject, message))
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
                    && u.Email != null)
                .ToListAsync()
                .ConfigureAwait(false);

            var sent = 0;
            foreach (var staff in staffMembers)
            {
                var evaluationUrl = $"{siteBaseUrl}/AcademicStaff/EvaluateCredentials?userId={staff.Id}";
                var subject = "A Gentle Reminder: Complete Your Staff Credential Evaluation";
                var message =
                    $"Dear <b>{staff.FirstName}</b>,<br/><br/>" +
                    "Thank you again for applying to join Emus Institute. We wanted to reach out with a friendly reminder " +
                    "that your staff credential evaluation has not yet been completed.<br/><br/>" +
                    "This evaluation is necessary for your application to progress. " +
                    "Please upload your documents and complete the evaluation fee when you have a moment.<br/><br/>" +
                    "You can continue here:<br/>" +
                    $"<a href='{evaluationUrl}' target='_blank'>" +
                    "<button style='color:white; background-color:#06BBCC; padding:12px; border:1px solid #06BBCC;'>Complete Staff Evaluation</button></a>" +
                    "<br/><br/>If you have questions or need support, please contact us and we will gladly help.<br/><br/>" +
                    "Warm regards,<br/>Emus Institute Team";

                if (_emailService.SendEmail(staff.Email!, subject, message))
                {
                    sent++;
                }
            }

            return sent;
        }
    }
}
