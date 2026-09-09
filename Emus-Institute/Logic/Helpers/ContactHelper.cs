using Core.Config;
using Core.DB;
using Core.Models;
using Logic.IHelpers;
using Logic.Services;

namespace Logic.Helpers
{
    public class ContactHelper : IContactHelper
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IGeneralConfiguration _generalConfiguration;

        public ContactHelper(
            AppDbContext context,
            IEmailService emailService,
            IGeneralConfiguration generalConfiguration)
        {
            _context = context;
            _emailService = emailService;
            _generalConfiguration = generalConfiguration;
        }

        public (bool Success, string Message) SubmitContactMessage(string name, string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Please enter your name.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, "Please enter your email address.");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                return (false, "Please enter a subject.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return (false, "Please enter your message.");
            }

            var enquiry = new EnquiryMessage
            {
                Name = name.Trim(),
                Email = email.Trim(),
                Subject = subject.Trim(),
                Description = message.Trim(),
                Active = true,
                DateCreated = DateTime.Now
            };

            _context.Enquiries.Add(enquiry);
            _context.SaveChanges();

            var adminEmail = _generalConfiguration.AdminEmail;
            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var adminSubject = $"New Contact Message: {enquiry.Subject}";
                var adminMessage =
                    "Hello Admin,<br><br>" +
                    "A new contact message has been submitted on the Emus Institute website.<br><br>" +
                    $"<b>Name:</b> {enquiry.Name}<br>" +
                    $"<b>Email:</b> {enquiry.Email}<br>" +
                    $"<b>Subject:</b> {enquiry.Subject}<br>" +
                    $"<b>Message:</b><br>{enquiry.Description.Replace("\n", "<br>")}<br><br>" +
                    $"<b>Date:</b> {enquiry.DateCreated:dddd, dd MMMM yyyy HH:mm}<br><br>" +
                    "Emus Institute Team";

                _emailService.SendEmail(adminEmail, adminSubject, adminMessage);
            }

            return (true, "Your message has been sent to the admin. You will receive a response soon.");
        }
    }
}
