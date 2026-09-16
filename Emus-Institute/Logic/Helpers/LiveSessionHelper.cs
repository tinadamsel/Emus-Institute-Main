using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class LiveSessionHelper : ILiveSessionHelper
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public LiveSessionHelper(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public List<LiveSessionViewModel> GetStaffDepartmentLiveSessions(string userId)
        {
            var staff = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && !x.Deactivated);
            if (staff?.DepartmentId == null)
            {
                return new List<LiveSessionViewModel>();
            }

            return _context.LiveSessions
                .Include(x => x.Department)
                .Include(x => x.CreatedBy)
                .Where(x => x.Active && x.DepartmentId == staff.DepartmentId)
                .OrderByDescending(x => x.StartDateTime)
                .AsEnumerable()
                .Select(x => MapSession(x, staff.Id, isStaff: true))
                .ToList();
        }

        public List<LiveSessionViewModel> GetStudentLiveSessions(string userId)
        {
            var student = _context.ApplicationUser
                .Include(x => x.Department)
                .FirstOrDefault(x => x.Id == userId && !x.Deactivated && x.IsStudent && !x.IsAdmin);

            if (student?.DepartmentId == null)
            {
                return new List<LiveSessionViewModel>();
            }

            return _context.LiveSessions
                .Include(x => x.Department)
                .Include(x => x.CreatedBy)
                .Where(x => x.Active
                    && x.Status != LiveSessionStatus.Cancelled
                    && x.DepartmentId == student.DepartmentId
                    && x.EndDateTime >= AppTime.Now.AddDays(-1))
                .OrderBy(x => x.StartDateTime)
                .AsEnumerable()
                .Select(x => MapSession(x, student.Id, isStaff: false))
                .ToList();
        }

        public LiveSessionViewModel? GetLiveSessionForJoin(int sessionId, string userId, bool isStaff)
        {
            var session = _context.LiveSessions
                .Include(x => x.Department)
                .Include(x => x.CreatedBy)
                .FirstOrDefault(x => x.Id == sessionId && x.Active);

            if (session == null)
            {
                return null;
            }

            var user = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && !x.Deactivated);
            if (user == null)
            {
                return null;
            }

            if (isStaff)
            {
                if (!user.IsAdmin || user.DepartmentId != session.DepartmentId)
                {
                    var denied = MapSession(session, userId, isStaff);
                    denied.CanJoin = false;
                    return denied;
                }
            }
            else if (!user.IsStudent || user.IsAdmin || user.DepartmentId != session.DepartmentId || user.StudentId == null)
            {
                var denied = MapSession(session, userId, isStaff);
                denied.CanJoin = false;
                return denied;
            }

            var mapped = MapSession(session, userId, isStaff);
            if (!mapped.CanJoin)
            {
                return mapped;
            }

            mapped.JitsiDisplayName = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(mapped.JitsiDisplayName))
            {
                mapped.JitsiDisplayName = isStaff ? "Staff Host" : "Student";
            }

            mapped.IsHost = isStaff;
            mapped.JitsiJoinUrl = BuildJitsiJoinUrl(mapped.RoomCode, mapped.JitsiDisplayName, mapped.IsHost, mapped.Id);
            return mapped;
        }

        public async Task<(bool Success, string Message)> CreateLiveSessionAsync(LiveSessionViewModel model, string staffUserId)
        {
            if (model == null || string.IsNullOrWhiteSpace(staffUserId))
            {
                return (false, "Invalid session details.");
            }

            if (string.IsNullOrWhiteSpace(model.RoomName))
            {
                return (false, "Room name is required.");
            }

            if (model.DurationMinutes <= 0)
            {
                return (false, "Duration must be greater than zero.");
            }

            if (model.StartDateTime <= AppTime.Now)
            {
                return (false, "Start time must be in the future.");
            }

            var staff = await _context.ApplicationUser
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == staffUserId && !x.Deactivated && x.IsAdmin)
                .ConfigureAwait(false);

            if (staff?.DepartmentId == null || staff.DepartmentId <= 0)
            {
                return (false, "You must be assigned to a department before scheduling live sessions.");
            }

            var endDateTime = model.StartDateTime.AddMinutes(model.DurationMinutes);
            if (HasDepartmentScheduleConflict(staff.DepartmentId.Value, model.StartDateTime, endDateTime))
            {
                return (false, "Another live session is already scheduled for your department during this time. Only one session per department can run at a time.");
            }

            var session = new LiveSession
            {
                RoomName = model.RoomName.Trim(),
                WelcomeMessage = string.IsNullOrWhiteSpace(model.WelcomeMessage) ? null : model.WelcomeMessage.Trim(),
                DepartmentId = staff.DepartmentId.Value,
                CreatedByUserId = staffUserId,
                StartDateTime = model.StartDateTime,
                EndDateTime = endDateTime,
                DurationMinutes = model.DurationMinutes,
                RoomCode = "pending",
                Status = LiveSessionStatus.Scheduled,
                Active = true,
                DateCreated = AppTime.Now
            };

            _context.LiveSessions.Add(session);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            session.RoomCode = BuildRoomCode(session.Id, staff.DepartmentId.Value);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            NotifyDepartmentStudents(session, staff);

            return (true, "Live session scheduled successfully. Students in your department have been notified by email.");
        }

        public bool HasDepartmentScheduleConflict(int departmentId, DateTime startDateTime, DateTime endDateTime, int? excludeSessionId = null)
        {
            return _context.LiveSessions.Any(x =>
                x.Active
                && x.Status != LiveSessionStatus.Cancelled
                && x.DepartmentId == departmentId
                && (!excludeSessionId.HasValue || x.Id != excludeSessionId.Value)
                && startDateTime < x.EndDateTime
                && endDateTime > x.StartDateTime);
        }

        private void NotifyDepartmentStudents(LiveSession session, ApplicationUser staff)
        {
            var students = _context.ApplicationUser
                .Where(x => x.DepartmentId == session.DepartmentId
                    && x.IsStudent
                    && !x.IsAdmin
                    && !x.Deactivated
                    && x.StudentId != null
                    && !string.IsNullOrWhiteSpace(x.Email))
                .ToList();

            if (!students.Any())
            {
                return;
            }

            var departmentName = staff.Department?.Name
                ?? _context.Departments.FirstOrDefault(d => d.Id == session.DepartmentId)?.Name
                ?? "your department";

            var scheduleDisplay = session.StartDateTime.ToString("dddd, MMMM d, yyyy 'at' h:mm tt");
            var subject = $"Live Class Scheduled: {session.RoomName}";
            var welcomeBlock = string.IsNullOrWhiteSpace(session.WelcomeMessage)
                ? string.Empty
                : $"<br/><br/><strong>Message from your lecturer:</strong><br/>{session.WelcomeMessage}";

            foreach (var student in students)
            {
                var message =
                    $"Dear <b>{student.FirstName}</b>,<br/><br/>" +
                    $"A live virtual class has been scheduled for <b>{departmentName}</b>.<br/><br/>" +
                    $"<strong>Class:</strong> {session.RoomName}<br/>" +
                    $"<strong>Date &amp; time:</strong> {scheduleDisplay}<br/>" +
                    $"<strong>Duration:</strong> {session.DurationMinutes} minutes<br/>" +
                    welcomeBlock +
                    "<br/><br/>When the session begins, log in to the Emus Institute student portal, open " +
                    "<strong>Virtual Class</strong>, and click <strong>Attend Class</strong> to join from the website." +
                    "<br/><br/>Warm regards,<br/>Emus Institute Team";

                _emailService.SendEmail(student.Email!, subject, message);
            }
        }

        private static string BuildRoomCode(int sessionId, int departmentId)
        {
            var suffix = Guid.NewGuid().ToString("N")[..8];
            return $"emus-inst-dept{departmentId}-session{sessionId}-{suffix}";
        }

        public static string BuildJitsiJoinUrl(string? roomCode, string? displayName, bool isHost, int sessionId)
        {
            var room = string.IsNullOrWhiteSpace(roomCode) || roomCode == "pending"
                ? $"emus-inst-session{sessionId}"
                : roomCode.Trim();
            var name = string.IsNullOrWhiteSpace(displayName) ? "Participant" : displayName.Trim();
            if (isHost && !name.Contains("(Host)", StringComparison.OrdinalIgnoreCase))
            {
                name += " (Host)";
            }

            name = name.Replace("\"", string.Empty);
            var hash = string.Join("&",
                "config.prejoinPageEnabled=true",
                "config.prejoinConfig.enabled=true",
                "config.startWithAudioMuted=true",
                "config.startWithVideoMuted=true",
                "config.disableDeepLinking=true",
                "config.enableWelcomePage=false",
                "userInfo.displayName=" + Uri.EscapeDataString("\"" + name + "\""));

            return $"https://meet.jit.si/{Uri.EscapeDataString(room)}#{hash}";
        }

        private LiveSessionViewModel MapSession(LiveSession session, string userId, bool isStaff)
        {
            var now = AppTime.Now;
            var isActiveNow = now >= session.StartDateTime && now <= session.EndDateTime;
            var isUpcoming = now < session.StartDateTime;
            var isCompleted = now > session.EndDateTime;

            var canJoin = CanUserJoin(session, isStaff, now);
            var displayStatus = GetDisplayStatus(session, isActiveNow, isCompleted);

            return new LiveSessionViewModel
            {
                Id = session.Id,
                RoomName = session.RoomName,
                WelcomeMessage = session.WelcomeMessage,
                DepartmentId = session.DepartmentId,
                DepartmentName = session.Department?.Name,
                CreatedByUserId = session.CreatedByUserId,
                StaffName = session.CreatedBy == null
                    ? null
                    : $"{session.CreatedBy.FirstName} {session.CreatedBy.LastName}".Trim(),
                StartDateTime = session.StartDateTime,
                EndDateTime = session.EndDateTime,
                DurationMinutes = session.DurationMinutes,
                RoomCode = session.RoomCode,
                Status = session.Status,
                Active = session.Active,
                DateCreated = session.DateCreated,
                IsActiveNow = isActiveNow,
                CanJoin = canJoin,
                IsUpcoming = isUpcoming,
                StatusLabel = displayStatus,
                ScheduleDisplay = $"{session.StartDateTime:dddd, MMM d, yyyy · h:mm tt} ({session.DurationMinutes} min)"
            };
        }

        private static bool CanUserJoin(LiveSession session, bool isStaff, DateTime now)
        {
            if (!session.Active || session.Status == LiveSessionStatus.Cancelled)
            {
                return false;
            }

            if (now > session.EndDateTime)
            {
                return false;
            }

            if (isStaff)
            {
                return now >= session.StartDateTime.AddMinutes(-10);
            }

            return now >= session.StartDateTime && now <= session.EndDateTime;
        }

        private static string GetDisplayStatus(LiveSession session, bool isActiveNow, bool isCompleted)
        {
            if (session.Status == LiveSessionStatus.Cancelled)
            {
                return "Cancelled";
            }

            if (isCompleted)
            {
                return "Completed";
            }

            if (isActiveNow)
            {
                return "Live now";
            }

            return "Scheduled";
        }
    }
}
