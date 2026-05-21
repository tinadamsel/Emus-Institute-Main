using Core.Models;

namespace Logic.IHelpers
{
    public interface IStaffPaystackHelper
    {
        PaystackResponse MakeStaffPayment(Payment payment);
        Task<PaystackResponse> VerifyStaffPayment(PayStack payment);
    }
}
