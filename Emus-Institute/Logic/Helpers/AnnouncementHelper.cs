using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class AnnouncementHelper : IAnnouncementHelper
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AnnouncementHelper(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<(bool Success, string Message)> CreateAnnouncementAsync(
            string staffUserId,
            string title,
            string message,
            AnnouncementAudience audience,
            int? departmentId,
            DateTime startDateTime,
            DateTime endDateTime)
        {
            var staff = await _context.ApplicationUser
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == staffUserId && !x.Deactivated && x.IsAdmin)
                .ConfigureAwait(false);
            if (staff == null)
            {
                return (false, "Staff account not found.");
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
            {
                return (false, "Title and message are required.");
            }

            if (endDateTime <= startDateTime)
            {
                return (false, "End date/time must be after start date/time.");
            }

            int? targetDepartmentId = null;
            if (audience == AnnouncementAudience.Department)
            {
                if (!departmentId.HasValue || departmentId <= 0)
                {
                    targetDepartmentId = staff.DepartmentId;
                }
                else
                {
                    targetDepartmentId = departmentId;
                }

                if (!targetDepartmentId.HasValue || targetDepartmentId <= 0)
                {
                    return (false, "Please select a valid department.");
                }
            }

            var announcement = new Announcement
            {
                Title = title.Trim(),
                Message = message.Trim(),
                Audience = audience,
                DepartmentId = targetDepartmentId,
                CreatedByUserId = staffUserId,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                Active = true,
                DateCreated = DateTime.Now
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            if (audience == AnnouncementAudience.Department && targetDepartmentId.HasValue)
            {
                NotifyDepartmentStudents(targetDepartmentId.Value, announcement);
            }

            return (true, "Announcement created successfully.");
        }

        public List<AnnouncementViewModel> GetStaffAnnouncements(string staffUserId)
        {
            var staff = _context.ApplicationUser.FirstOrDefault(x => x.Id == staffUserId && !x.Deactivated && x.IsAdmin);
            if (staff == null)
            {
                return new List<AnnouncementViewModel>();
            }

            return _context.Announcements
                .Include(x => x.Department)
                .Include(x => x.CreatedBy)
                .Where(x => x.Active)
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(MapAnnouncement)
                .ToList();
        }

        public List<AnnouncementViewModel> GetStudentAnnouncements(string studentUserId)
        {
            var student = _context.ApplicationUser.FirstOrDefault(x =>
                x.Id == studentUserId && !x.Deactivated && x.IsStudent && !x.IsAdmin);
            if (student == null)
            {
                return new List<AnnouncementViewModel>();
            }

            var now = DateTime.Now;
            return _context.Announcements
                .Include(x => x.Department)
                .Include(x => x.CreatedBy)
                .Where(x => x.Active
                    && x.StartDateTime <= now
                    && x.EndDateTime >= now
                    && (x.Audience == AnnouncementAudience.WholeStudents
                        || (x.Audience == AnnouncementAudience.Department && x.DepartmentId == student.DepartmentId)))
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(MapAnnouncement)
                .ToList();
        }

        public int GetActiveAnnouncementCountForStudent(string studentUserId)
        {
            var student = _context.ApplicationUser.FirstOrDefault(x =>
                x.Id == studentUserId && !x.Deactivated && x.IsStudent && !x.IsAdmin);
            if (student == null)
            {
                return 0;
            }

            var now = DateTime.Now;
            return _context.Announcements.Count(x =>
                x.Active
                && x.StartDateTime <= now
                && x.EndDateTime >= now
                && (x.Audience == AnnouncementAudience.WholeStudents
                    || (x.Audience == AnnouncementAudience.Department && x.DepartmentId == student.DepartmentId)));
        }

        public int GetActiveWholeStudentAnnouncementCount()
        {
            var now = DateTime.Now;
            return _context.Announcements.Count(x =>
                x.Active
                && x.Audience == AnnouncementAudience.WholeStudents
                && x.StartDateTime <= now
                && x.EndDateTime >= now);
        }

        private void NotifyDepartmentStudents(int departmentId, Announcement announcement)
        {
            var students = _context.ApplicationUser
                .Where(x => x.IsStudent
                    && !x.IsAdmin
                    && !x.Deactivated
                    && x.DepartmentId == departmentId
                    && x.StudentId != null
                    && !string.IsNullOrWhiteSpace(x.Email))
                .ToList();

            var departmentName = _context.Departments.FirstOrDefault(d => d.Id == departmentId)?.Name ?? "your department";
            foreach (var student in students)
            {
                var subject = "New department announcement";
                var message =
                    $"Dear <b>{student.FirstName}</b>,<br/><br/>" +
                    $"A new announcement has been posted for <b>{departmentName}</b>.<br/><br/>" +
                    "Please log into the Emus Institute student portal and open the <b>Announcements</b> page to read it." +
                    "<br/><br/>Warm regards,<br/>Emus Institute Team";
                _emailService.SendEmail(student.Email!, subject, message);
            }
        }

        private static AnnouncementViewModel MapAnnouncement(Announcement item)
        {
            var now = DateTime.Now;
            var isActive = now >= item.StartDateTime && now <= item.EndDateTime;
            var isUpcoming = now < item.StartDateTime;
            var isExpired = now > item.EndDateTime;
            return new AnnouncementViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Message = item.Message,
                Audience = item.Audience,
                DepartmentId = item.DepartmentId,
                DepartmentName = item.Department?.Name,
                CreatedByUserId = item.CreatedByUserId,
                CreatedByName = item.CreatedBy == null ? null : $"{item.CreatedBy.FirstName} {item.CreatedBy.LastName}".Trim(),
                StartDateTime = item.StartDateTime,
                EndDateTime = item.EndDateTime,
                Active = item.Active,
                DateCreated = item.DateCreated,
                IsActiveNow = isActive,
                IsUpcoming = isUpcoming,
                IsExpired = isExpired,
                StatusLabel = isActive ? "Active" : (isUpcoming ? "Upcoming" : "Expired"),
                AudienceLabel = item.Audience == AnnouncementAudience.WholeStudents
                    ? "Whole Students"
                    : $"Department: {item.Department?.Name ?? "N/A"}"
            };
        }
    }
}
