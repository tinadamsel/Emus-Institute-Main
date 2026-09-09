using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class StudyCenterHelper : IStudyCenterHelper
    {
        private readonly AppDbContext _context;

        public StudyCenterHelper(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> SubmitStudyCenterAsync(
            string name,
            string address,
            string country,
            string contactPerson,
            IFormFile imageFile,
            string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Study center name is required.");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                return (false, "Address is required.");
            }

            if (string.IsNullOrWhiteSpace(country))
            {
                return (false, "Country is required.");
            }

            if (string.IsNullOrWhiteSpace(contactPerson))
            {
                return (false, "Contact person is required.");
            }

            if (imageFile == null || imageFile.Length == 0)
            {
                return (false, "Please upload an image for the study center.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName);
            if (string.IsNullOrWhiteSpace(extension) ||
                !allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return (false, "Image must be JPG, PNG, or WEBP.");
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "study-centers");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(imageFile.FileName)}";
            var absolutePath = Path.Combine(uploadsFolder, uniqueFileName);
            await using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream).ConfigureAwait(false);
            }

            var relativePath = $"/uploads/study-centers/{uniqueFileName}";

            var studyCenter = new StudyCenter
            {
                Name = name.Trim(),
                Address = address.Trim(),
                Country = country.Trim(),
                ContactPerson = contactPerson.Trim(),
                ImagePath = relativePath,
                Status = StudyCenterApprovalStatus.Pending,
                Active = true,
                DateCreated = DateTime.Now
            };

            _context.StudyCenters.Add(studyCenter);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return (true, "Your study center has been submitted for approval. You will be notified once it is reviewed.");
        }

        public List<StudyCenterViewModel> GetApprovedStudyCenters()
        {
            return _context.StudyCenters
                .Where(x => x.Active && x.Status == StudyCenterApprovalStatus.Approved)
                .OrderByDescending(x => x.DateApproved ?? x.DateCreated)
                .AsEnumerable()
                .Select(MapStudyCenter)
                .ToList();
        }

        public List<StudyCenterViewModel> GetPendingStudyCenters()
        {
            return _context.StudyCenters
                .Where(x => x.Active && x.Status == StudyCenterApprovalStatus.Pending)
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(MapStudyCenter)
                .ToList();
        }

        public List<StudyCenterViewModel> GetApprovedStudyCentersForAdmin()
        {
            return _context.StudyCenters
                .Include(x => x.ApprovedBy)
                .Where(x => x.Active && x.Status == StudyCenterApprovalStatus.Approved)
                .OrderByDescending(x => x.DateApproved ?? x.DateCreated)
                .AsEnumerable()
                .Select(MapStudyCenter)
                .ToList();
        }

        public bool ApproveStudyCenter(int id, string superAdminUserId)
        {
            var center = _context.StudyCenters.FirstOrDefault(x => x.Id == id && x.Active);
            if (center == null)
            {
                return false;
            }

            if (center.Status == StudyCenterApprovalStatus.Approved)
            {
                return false;
            }

            center.Status = StudyCenterApprovalStatus.Approved;
            center.ApprovedByUserId = superAdminUserId;
            center.DateApproved = DateTime.Now;
            _context.StudyCenters.Update(center);
            _context.SaveChanges();
            return true;
        }

        public bool DeclineStudyCenter(int id)
        {
            var center = _context.StudyCenters.FirstOrDefault(x => x.Id == id && x.Active);
            if (center == null)
            {
                return false;
            }

            center.Status = StudyCenterApprovalStatus.Declined;
            _context.StudyCenters.Update(center);
            _context.SaveChanges();
            return true;
        }

        private static StudyCenterViewModel MapStudyCenter(StudyCenter center)
        {
            return new StudyCenterViewModel
            {
                Id = center.Id,
                Name = center.Name,
                Address = center.Address,
                Country = center.Country,
                ContactPerson = center.ContactPerson,
                ImagePath = center.ImagePath,
                Status = center.Status,
                StatusLabel = center.Status.ToString(),
                Active = center.Active,
                DateCreated = center.DateCreated,
                DateApproved = center.DateApproved,
                ApprovedByName = center.ApprovedBy != null
                    ? $"{center.ApprovedBy.FirstName} {center.ApprovedBy.LastName}".Trim()
                    : null
            };
        }
    }
}
