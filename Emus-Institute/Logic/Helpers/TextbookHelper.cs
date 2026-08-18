using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class TextbookHelper : ITextbookHelper
    {
        private readonly AppDbContext _context;

        public TextbookHelper(AppDbContext context)
        {
            _context = context;
        }

        public List<TextbookViewModel> GetStaffDepartmentTextbooks(string userId)
        {
            var staff = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && !x.Deactivated);
            if (staff?.DepartmentId == null)
            {
                return new List<TextbookViewModel>();
            }

            return _context.Textbooks
                .Include(x => x.Department)
                .Where(x => x.Active && x.DepartmentId == staff.DepartmentId)
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(MapTextbook)
                .ToList();
        }

        public List<TextbookViewModel> GetPendingTextbooks()
        {
            return _context.Textbooks
                .Include(x => x.Department)
                .Where(x => x.Active && x.Status == TextbookApprovalStatus.Pending)
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(MapTextbook)
                .ToList();
        }

        public List<TextbookViewModel> GetApprovedTextbooksForStudent(string userId)
        {
            var student = _context.ApplicationUser
                .Include(x => x.Department)
                .FirstOrDefault(x => x.Id == userId && !x.Deactivated && x.IsStudent && !x.IsAdmin);

            if (student?.DepartmentId == null)
            {
                return new List<TextbookViewModel>();
            }

            return _context.Textbooks
                .Include(x => x.Department)
                .Where(x => x.Active
                    && x.Status == TextbookApprovalStatus.Approved
                    && x.DepartmentId == student.DepartmentId)
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(MapTextbook)
                .ToList();
        }

        public async Task<bool> CreateTextbookAsync(TextbookViewModel model, IFormFile pdfFile, string staffUserId, string webRootPath)
        {
            if (model == null || pdfFile == null || string.IsNullOrWhiteSpace(staffUserId))
            {
                return false;
            }

            var staff = await _context.ApplicationUser
                .FirstOrDefaultAsync(x => x.Id == staffUserId && !x.Deactivated && x.IsAdmin)
                .ConfigureAwait(false);

            if (staff?.DepartmentId == null || staff.DepartmentId <= 0)
            {
                return false;
            }

            if (pdfFile.Length == 0 || !pdfFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "textbooks");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(pdfFile.FileName)}";
            var absolutePath = Path.Combine(uploadsFolder, uniqueFileName);
            await using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await pdfFile.CopyToAsync(stream).ConfigureAwait(false);
            }

            var relativePath = $"/uploads/textbooks/{uniqueFileName}";
            var textbookCode = string.IsNullOrWhiteSpace(model.TextbookCode)
                ? GenerateTextbookCode()
                : model.TextbookCode.Trim();

            var textbook = new Textbooks
            {
                Name = model.Name?.Trim(),
                Description = model.Description?.Trim(),
                TextbookCode = textbookCode,
                Price = 0,
                DepartmentId = staff.DepartmentId,
                TextbookForEachSession = model.TextbookForEachSession,
                UserId = staff.Id,
                AddedBy = $"{staff.FirstName} {staff.LastName}".Trim(),
                Active = true,
                DateCreated = DateTime.Now,
                Status = TextbookApprovalStatus.Pending,
                IsApproved = null,
                FilePath = relativePath,
                CoverImagePath = "/assets/img/grad.jpg"
            };

            _context.Textbooks.Add(textbook);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        public bool ApproveTextbook(int textbookId)
        {
            var textbook = _context.Textbooks.FirstOrDefault(x => x.Id == textbookId && x.Active);
            if (textbook == null)
            {
                return false;
            }

            textbook.Status = TextbookApprovalStatus.Approved;
            textbook.IsApproved = true;
            _context.Update(textbook);
            _context.SaveChanges();
            return true;
        }

        public bool DeclineTextbook(int textbookId)
        {
            var textbook = _context.Textbooks.FirstOrDefault(x => x.Id == textbookId && x.Active);
            if (textbook == null)
            {
                return false;
            }

            textbook.Status = TextbookApprovalStatus.Declined;
            textbook.IsApproved = false;
            _context.Update(textbook);
            _context.SaveChanges();
            return true;
        }

        public Textbooks? GetTextbookForDownload(int textbookId, string userId, bool isStudent)
        {
            var textbook = _context.Textbooks
                .Include(x => x.Department)
                .FirstOrDefault(x => x.Id == textbookId && x.Active && !string.IsNullOrEmpty(x.FilePath));

            if (textbook == null)
            {
                return null;
            }

            var user = _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && !x.Deactivated);
            if (user == null)
            {
                return null;
            }

            if (isStudent)
            {
                if (!user.IsStudent || user.IsAdmin || user.DepartmentId != textbook.DepartmentId
                    || textbook.Status != TextbookApprovalStatus.Approved)
                {
                    return null;
                }
            }
            else
            {
                if (!user.IsAdmin || user.DepartmentId != textbook.DepartmentId)
                {
                    return null;
                }
            }

            return textbook;
        }

        private static TextbookViewModel MapTextbook(Textbooks x)
        {
            return new TextbookViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                TextbookCode = x.TextbookCode,
                DepartmentId = x.DepartmentId,
                DepartmentName = x.Department != null ? x.Department.Name : "—",
                TextbookForEachSession = x.TextbookForEachSession,
                UserId = x.UserId,
                AddedBy = x.AddedBy,
                Active = x.Active,
                DateCreated = x.DateCreated,
                IsApproved = x.IsApproved,
                Status = x.Status,
                FilePath = x.FilePath,
                CoverImagePath = string.IsNullOrWhiteSpace(x.CoverImagePath) ? "/assets/img/grad.jpg" : x.CoverImagePath,
                StatusLabel = x.Status.ToString()
            };
        }

        private static string GenerateTextbookCode()
        {
            return "TB-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
