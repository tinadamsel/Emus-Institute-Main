using Core.ViewModels;
using Core.Models;
using Microsoft.AspNetCore.Http;

namespace Logic.IHelpers
{
    public interface ITextbookHelper
    {
        List<TextbookViewModel> GetStaffDepartmentTextbooks(string userId);
        List<TextbookViewModel> GetPendingTextbooks();
        List<TextbookViewModel> GetApprovedTextbooksForStudent(string userId);
        Task<bool> CreateTextbookAsync(TextbookViewModel model, IFormFile pdfFile, string staffUserId, string webRootPath);
        bool ApproveTextbook(int textbookId);
        bool DeclineTextbook(int textbookId);
        Textbooks? GetTextbookForDownload(int textbookId, string userId, bool isStudent);
    }
}
