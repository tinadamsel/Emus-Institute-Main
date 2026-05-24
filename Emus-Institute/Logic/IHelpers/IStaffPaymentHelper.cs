using Core.Models;

namespace Logic.IHelpers
{
    public interface IStaffPaymentHelper
    {
        Task<PaystackResponse> CreateStaffPayment(string userId, ApplicationUser user);
    }
}
