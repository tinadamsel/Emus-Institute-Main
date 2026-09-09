using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
    public class AssignmentHelper : IAssignmentHelper
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".txt", ".zip", ".png", ".jpg", ".jpeg", ".ppt", ".pptx"
        };

        private const long MaxFileBytes = 15 * 1024 * 1024;

        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AssignmentHelper(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public List<AssignmentViewModel> GetStaffDepartmentAssignments(string staffUserId)
        {
            var staff = _context.ApplicationUser.FirstOrDefault(x => x.Id == staffUserId && !x.Deactivated);
            if (staff?.DepartmentId == null)
            {
                return new List<AssignmentViewModel>();
            }

            return _context.Assignments
                .Include(x => x.Departments)
                .Include(x => x.CreatedBy)
                .Include(x => x.Submissions)
                .Where(x => x.Active && x.DepartmentId == staff.DepartmentId)
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(x => MapAssignment(x, student: null))
                .ToList();
        }

        public List<AssignmentViewModel> GetStudentAssignments(string studentUserId)
        {
            var student = GetActiveStudent(studentUserId);
            if (student?.DepartmentId == null)
            {
                return new List<AssignmentViewModel>();
            }

            return _context.Assignments
                .Include(x => x.Departments)
                .Include(x => x.CreatedBy)
                .Include(x => x.Submissions)
                .Where(x => x.Active && x.DepartmentId == student.DepartmentId)
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(x => MapAssignment(x, student))
                .ToList();
        }

        public AssignmentViewModel? GetStaffAssignment(int assignmentId, string staffUserId)
        {
            var staff = _context.ApplicationUser.FirstOrDefault(x => x.Id == staffUserId && !x.Deactivated && x.IsAdmin);
            if (staff?.DepartmentId == null)
            {
                return null;
            }

            var assignment = LoadAssignment(assignmentId);
            if (assignment == null || assignment.DepartmentId != staff.DepartmentId)
            {
                return null;
            }

            return MapAssignment(assignment, student: null);
        }

        public AssignmentViewModel? GetStudentAssignment(int assignmentId, string studentUserId)
        {
            var student = GetActiveStudent(studentUserId);
            if (student?.DepartmentId == null)
            {
                return null;
            }

            var assignment = LoadAssignment(assignmentId);
            if (assignment == null || assignment.DepartmentId != student.DepartmentId)
            {
                return null;
            }

            return MapAssignment(assignment, student);
        }

        public AssignmentSubmissionsPageViewModel? GetAssignmentSubmissions(int assignmentId, string staffUserId)
        {
            var assignment = GetStaffAssignment(assignmentId, staffUserId);
            if (assignment == null)
            {
                return null;
            }

            var submissions = _context.AssignmentSubmissions
                .Include(x => x.Student)
                .Where(x => x.Active && x.AssignmentId == assignmentId)
                .OrderByDescending(x => x.SubmittedAt)
                .AsEnumerable()
                .Select(MapSubmission)
                .ToList();

            return new AssignmentSubmissionsPageViewModel
            {
                Assignment = assignment,
                Submissions = submissions
            };
        }

        public async Task<(bool Success, string Message)> CreateAssignmentAsync(
            AssignmentViewModel model, IFormFile? file, string staffUserId, string webRootPath)
        {
            if (model == null || string.IsNullOrWhiteSpace(staffUserId))
            {
                return (false, "Invalid assignment details.");
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return (false, "Assignment title is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Description))
            {
                return (false, "Assignment description is required.");
            }

            if (model.ValidUntilDate <= DateTime.Now)
            {
                return (false, "Due date must be in the future.");
            }

            if (model.TotalMarks <= 0)
            {
                return (false, "Total marks must be greater than zero.");
            }

            var staff = await _context.ApplicationUser
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == staffUserId && !x.Deactivated && x.IsAdmin)
                .ConfigureAwait(false);

            if (staff?.DepartmentId == null || staff.DepartmentId <= 0)
            {
                return (false, "You must be assigned to a department before creating assignments.");
            }

            string? savedFilePath = null;
            if (file != null && file.Length > 0)
            {
                var saveResult = await SaveFileAsync(file, webRootPath, "briefs").ConfigureAwait(false);
                if (!saveResult.Success)
                {
                    return (false, saveResult.Message);
                }
                savedFilePath = saveResult.RelativePath;
            }

            var assignment = new Assignment
            {
                Name = model.Name.Trim(),
                Description = model.Description.Trim(),
                ValidUntilDate = model.ValidUntilDate,
                DepartmentId = staff.DepartmentId.Value,
                CreatedByUserId = staff.Id,
                FilePath = savedFilePath,
                TotalMarks = model.TotalMarks,
                IsSubmitted = false,
                Active = true,
                DateCreated = DateTime.Now
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            NotifyDepartmentStudents(assignment, staff);

            return (true, "Assignment created successfully. Students in your department have been notified by email.");
        }

        public async Task<(bool Success, string Message)> SubmitAssignmentAsync(
            int assignmentId, string? comment, IFormFile? file, string studentUserId, string webRootPath)
        {
            var student = GetActiveStudent(studentUserId);
            if (student?.DepartmentId == null)
            {
                return (false, "Your student account could not be verified.");
            }

            var assignment = LoadAssignment(assignmentId);
            if (assignment == null || assignment.DepartmentId != student.DepartmentId)
            {
                return (false, "This assignment is not available.");
            }

            if (DateTime.Now > assignment.ValidUntilDate)
            {
                return (false, "The deadline for this assignment has passed.");
            }

            var existing = assignment.Submissions.FirstOrDefault(x => x.Active && x.StudentUserId == student.Id);
            if (existing?.GradedAt != null)
            {
                return (false, "This assignment has already been graded and cannot be changed.");
            }

            if ((file == null || file.Length == 0) && existing == null)
            {
                return (false, "Please upload your assignment file.");
            }

            string? savedFilePath = existing?.FilePath;
            string? originalFileName = existing?.OriginalFileName;
            if (file != null && file.Length > 0)
            {
                var saveResult = await SaveFileAsync(file, webRootPath, "submissions").ConfigureAwait(false);
                if (!saveResult.Success)
                {
                    return (false, saveResult.Message);
                }
                savedFilePath = saveResult.RelativePath;
                originalFileName = Path.GetFileName(file.FileName);
            }

            if (existing == null)
            {
                existing = new AssignmentSubmission
                {
                    AssignmentId = assignment.Id,
                    StudentUserId = student.Id,
                    Active = true,
                    DateCreated = DateTime.Now
                };
                _context.AssignmentSubmissions.Add(existing);
            }

            existing.FilePath = savedFilePath;
            existing.OriginalFileName = originalFileName;
            existing.Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            existing.SubmittedAt = DateTime.Now;
            existing.Name = assignment.Name;

            assignment.IsSubmitted = true;
            await _context.SaveChangesAsync().ConfigureAwait(false);

            NotifyStaffOfSubmission(assignment, student);

            return (true, "Your assignment has been submitted successfully.");
        }

        public async Task<(bool Success, string Message)> GradeSubmissionAsync(
            int submissionId, decimal score, string? feedback, string staffUserId)
        {
            var staff = await _context.ApplicationUser
                .FirstOrDefaultAsync(x => x.Id == staffUserId && !x.Deactivated && x.IsAdmin)
                .ConfigureAwait(false);

            if (staff?.DepartmentId == null)
            {
                return (false, "Please login again.");
            }

            var submission = await _context.AssignmentSubmissions
                .Include(x => x.Assignment)
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.Id == submissionId && x.Active)
                .ConfigureAwait(false);

            if (submission?.Assignment == null || submission.Assignment.DepartmentId != staff.DepartmentId)
            {
                return (false, "This submission was not found.");
            }

            if (score < 0 || score > submission.Assignment.TotalMarks)
            {
                return (false, $"Score must be between 0 and {submission.Assignment.TotalMarks}.");
            }

            submission.Score = score;
            submission.Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
            submission.GradedAt = DateTime.Now;
            submission.GradedByUserId = staff.Id;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            NotifyStudentOfGrade(submission, staff);

            return (true, "Submission graded successfully.");
        }

        public Assignment? GetAssignmentFileForDownload(int assignmentId, string userId, bool isStaff)
        {
            var assignment = _context.Assignments.FirstOrDefault(x => x.Id == assignmentId && x.Active);
            if (assignment == null || string.IsNullOrWhiteSpace(assignment.FilePath))
            {
                return null;
            }

            var user = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && !x.Deactivated);
            if (user?.DepartmentId != assignment.DepartmentId)
            {
                return null;
            }

            if (isStaff && !user.IsAdmin)
            {
                return null;
            }

            if (!isStaff && (!user.IsStudent || user.IsAdmin))
            {
                return null;
            }

            return assignment;
        }

        public AssignmentSubmission? GetSubmissionFileForDownload(int submissionId, string userId, bool isStaff)
        {
            var submission = _context.AssignmentSubmissions
                .Include(x => x.Assignment)
                .FirstOrDefault(x => x.Id == submissionId && x.Active);

            if (submission?.Assignment == null || string.IsNullOrWhiteSpace(submission.FilePath))
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
                if (!user.IsAdmin || user.DepartmentId != submission.Assignment.DepartmentId)
                {
                    return null;
                }
            }
            else if (submission.StudentUserId != user.Id || !user.IsStudent || user.IsAdmin)
            {
                return null;
            }

            return submission;
        }

        public int GetStaffAssignmentCount(string staffUserId)
        {
            return GetStaffDepartmentAssignments(staffUserId).Count;
        }

        public int GetStudentAssignmentCount(string studentUserId)
        {
            return GetStudentAssignments(studentUserId).Count;
        }

        private Assignment? LoadAssignment(int assignmentId)
        {
            return _context.Assignments
                .Include(x => x.Departments)
                .Include(x => x.CreatedBy)
                .Include(x => x.Submissions)
                .FirstOrDefault(x => x.Id == assignmentId && x.Active);
        }

        private ApplicationUser? GetActiveStudent(string studentUserId)
        {
            return _context.ApplicationUser
                .Include(x => x.Department)
                .FirstOrDefault(x => x.Id == studentUserId && !x.Deactivated && x.IsStudent && !x.IsAdmin && x.StudentId != null);
        }

        private static AssignmentViewModel MapAssignment(Assignment assignment, ApplicationUser? student)
        {
            var now = DateTime.Now;
            var isOverdue = now > assignment.ValidUntilDate;
            var submissions = assignment.Submissions?.Where(x => x.Active).ToList() ?? new List<AssignmentSubmission>();
            var studentSubmission = student == null
                ? null
                : submissions.FirstOrDefault(x => x.StudentUserId == student.Id);
            var isGraded = studentSubmission?.GradedAt != null;

            return new AssignmentViewModel
            {
                Id = assignment.Id,
                Name = assignment.Name,
                Description = assignment.Description,
                ValidUntilDate = assignment.ValidUntilDate,
                DepartmentId = assignment.DepartmentId,
                DepartmentName = assignment.Departments?.Name,
                CreatedByUserId = assignment.CreatedByUserId,
                StaffName = assignment.CreatedBy == null
                    ? null
                    : $"{assignment.CreatedBy.FirstName} {assignment.CreatedBy.LastName}".Trim(),
                FilePath = assignment.FilePath,
                TotalMarks = assignment.TotalMarks,
                DateCreated = assignment.DateCreated,
                Active = assignment.Active,
                SubmissionCount = submissions.Count,
                GradedCount = submissions.Count(x => x.GradedAt != null),
                IsOverdue = isOverdue,
                HasSubmitted = studentSubmission != null,
                IsGraded = isGraded,
                CanSubmit = student != null && !isOverdue && !isGraded,
                SubmittedAt = studentSubmission?.SubmittedAt,
                Score = studentSubmission?.Score,
                Feedback = studentSubmission?.Feedback,
                SubmissionFilePath = studentSubmission?.FilePath,
                SubmissionFileName = studentSubmission?.OriginalFileName,
                SubmissionComment = studentSubmission?.Comment,
                SubmissionId = studentSubmission?.Id,
                StatusLabel = GetStatusLabel(isOverdue, studentSubmission != null, isGraded, student != null)
            };
        }

        private static AssignmentSubmissionViewModel MapSubmission(AssignmentSubmission submission)
        {
            var student = submission.Student;
            var isGraded = submission.GradedAt != null;
            return new AssignmentSubmissionViewModel
            {
                Id = submission.Id,
                AssignmentId = submission.AssignmentId,
                AssignmentName = submission.Assignment?.Name ?? submission.Name ?? string.Empty,
                TotalMarks = submission.Assignment?.TotalMarks ?? 0,
                StudentUserId = submission.StudentUserId,
                StudentName = student == null ? null : $"{student.FirstName} {student.LastName}".Trim(),
                StudentEmail = student?.Email,
                StudentIdNumber = student?.StudentId,
                FilePath = submission.FilePath,
                OriginalFileName = submission.OriginalFileName,
                Comment = submission.Comment,
                SubmittedAt = submission.SubmittedAt,
                Score = submission.Score,
                Feedback = submission.Feedback,
                GradedAt = submission.GradedAt,
                IsGraded = isGraded,
                StatusLabel = isGraded ? "Graded" : "Submitted"
            };
        }

        private static string GetStatusLabel(bool isOverdue, bool hasSubmitted, bool isGraded, bool isStudentView)
        {
            if (isGraded)
            {
                return "Graded";
            }

            if (hasSubmitted)
            {
                return isOverdue ? "Submitted" : "Submitted";
            }

            if (isOverdue)
            {
                return "Closed";
            }

            return isStudentView ? "Open" : "Open";
        }

        private async Task<(bool Success, string Message, string? RelativePath)> SaveFileAsync(
            IFormFile file, string webRootPath, string folder)
        {
            if (file.Length > MaxFileBytes)
            {
                return (false, "File size must be 15MB or less.", null);
            }

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            {
                return (false, "Allowed files: PDF, Word, PowerPoint, ZIP, TXT, PNG, or JPG.", null);
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "assignments", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}";
            var absolutePath = Path.Combine(uploadsFolder, uniqueFileName);
            await using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(stream).ConfigureAwait(false);
            }

            return (true, string.Empty, $"/uploads/assignments/{folder}/{uniqueFileName}");
        }

        private void NotifyDepartmentStudents(Assignment assignment, ApplicationUser staff)
        {
            var students = _context.ApplicationUser
                .Where(x => x.DepartmentId == assignment.DepartmentId
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
                ?? _context.Departments.FirstOrDefault(d => d.Id == assignment.DepartmentId)?.Name
                ?? "your department";
            var dueDisplay = assignment.ValidUntilDate.ToString("dddd, MMMM d, yyyy 'at' h:mm tt");
            var subject = $"New Assignment: {assignment.Name}";

            foreach (var student in students)
            {
                var message =
                    $"Dear <b>{student.FirstName}</b>,<br/><br/>" +
                    $"A new assignment has been posted for <b>{departmentName}</b>.<br/><br/>" +
                    $"<strong>Assignment:</strong> {assignment.Name}<br/>" +
                    $"<strong>Due date:</strong> {dueDisplay}<br/>" +
                    $"<strong>Total marks:</strong> {assignment.TotalMarks}<br/><br/>" +
                    "Please log in to the Emus Institute platform, open <strong>Assignments</strong>, " +
                    "and check the task so you can submit your work before the deadline." +
                    "<br/><br/>Warm regards,<br/>Emus Institute Team";

                _emailService.SendEmail(student.Email!, subject, message);
            }
        }

        private void NotifyStaffOfSubmission(Assignment assignment, ApplicationUser student)
        {
            var staff = assignment.CreatedBy
                ?? _context.ApplicationUser.FirstOrDefault(x => x.Id == assignment.CreatedByUserId && !x.Deactivated);

            if (staff == null || string.IsNullOrWhiteSpace(staff.Email))
            {
                return;
            }

            var studentName = $"{student.FirstName} {student.LastName}".Trim();
            var subject = $"Assignment submitted: {assignment.Name}";
            var message =
                $"Dear <b>{staff.FirstName}</b>,<br/><br/>" +
                $"<b>{studentName}</b> has submitted the assignment <b>{assignment.Name}</b>.<br/><br/>" +
                "Log in to the Emus Institute staff dashboard, open <strong>Assignments</strong>, " +
                "and grade the submission." +
                "<br/><br/>Warm regards,<br/>Emus Institute Team";

            _emailService.SendEmail(staff.Email, subject, message);
        }

        private void NotifyStudentOfGrade(AssignmentSubmission submission, ApplicationUser staff)
        {
            var student = submission.Student;
            if (student == null || string.IsNullOrWhiteSpace(student.Email))
            {
                return;
            }

            var assignmentName = submission.Assignment?.Name ?? "your assignment";
            var totalMarks = submission.Assignment?.TotalMarks ?? 0;
            var subject = $"Assignment graded: {assignmentName}";
            var message =
                $"Dear <b>{student.FirstName}</b>,<br/><br/>" +
                $"Your assignment <b>{assignmentName}</b> has been graded by {staff.FirstName}.<br/><br/>" +
                $"<strong>Score:</strong> {submission.Score} / {totalMarks}<br/>" +
                (string.IsNullOrWhiteSpace(submission.Feedback)
                    ? string.Empty
                    : $"<strong>Feedback:</strong> {submission.Feedback}<br/>") +
                "<br/>Log in to the Emus Institute platform and open <strong>Assignments</strong> to view the full result." +
                "<br/><br/>Warm regards,<br/>Emus Institute Team";

            _emailService.SendEmail(student.Email, subject, message);
        }
    }
}
