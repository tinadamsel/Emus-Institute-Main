using Core.Config;
using Core.DB;
using Core.Models;
using Logic.IHelpers;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class StaffPaymentHelper : IStaffPaymentHelper
    {
        private readonly AppDbContext _context;
        private readonly IStaffPaystackHelper _staffPaystackHelper;
        private readonly IGeneralConfiguration _generalConfiguration;

        public StaffPaymentHelper(AppDbContext context, IStaffPaystackHelper staffPaystackHelper, IGeneralConfiguration generalConfiguration)
        {
            _context = context;
            _staffPaystackHelper = staffPaystackHelper;
            _generalConfiguration = generalConfiguration;
        }

        public async Task<PaystackResponse> CreateStaffPayment(string userId, ApplicationUser user)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                var staffUser = _context.ApplicationUser
                    .FirstOrDefault(x => x.Id == userId && !x.Deactivated && x.IsAdmin && !x.Paid);

                if (staffUser == null)
                {
                    return null;
                }

                var departmentId = staffUser.DepartmentId ?? _context.Departments.Select(d => d.Id).FirstOrDefault();
                if (departmentId == 0)
                {
                    departmentId = 1;
                }

                var existingPayment = _context.Payments.FirstOrDefault(x => x.UserId == userId && x.Details == "Staff Evaluation Payment");
                Payment paymentEntity;

                if (existingPayment != null)
                {
                    existingPayment.PaymentDate = DateTime.Now;
                    existingPayment.Status = PaymentStatus.Pending;
                    existingPayment.PaymentMethod = "Paystack";
                    existingPayment.Reference = GenerateStaffReference();
                    existingPayment.Amount = _generalConfiguration.StaffEvaluationAmountNgn;
                    _context.Update(existingPayment);
                    await _context.SaveChangesAsync();
                    paymentEntity = existingPayment;
                }
                else
                {
                    var payment = new Payment
                    {
                        PaymentDate = DateTime.Now,
                        Status = PaymentStatus.Pending,
                        PaymentMethod = "Paystack",
                        UserId = userId,
                        DepartmentId = departmentId,
                        Amount = _generalConfiguration.StaffEvaluationAmountNgn,
                        Reference = GenerateStaffReference(),
                        Details = "Staff Evaluation Payment"
                    };
                    var newPayment = await _context.AddAsync(payment);
                    await _context.SaveChangesAsync();
                    paymentEntity = newPayment.Entity;
                }

                if (paymentEntity.Id == Guid.Empty)
                {
                    return null;
                }

                var paystackResponse = _staffPaystackHelper.MakeStaffPayment(paymentEntity);
                if (paystackResponse == null)
                {
                    return null;
                }

                var paystack = new PayStack
                {
                    PaymentId = paymentEntity.Id,
                    Payment = paymentEntity,
                    Authorization_url = paystackResponse.data.authorization_url,
                    Access_code = paystackResponse.data.access_code,
                    Amount = paystackResponse.data.amount,
                    Reference = paystackResponse.data.reference,
                    Transaction_date = DateTime.Now,
                    Currency = paystackResponse.data.currency ?? "NGN"
                };
                _context.PayStackpayments.Add(paystack);
                await _context.SaveChangesAsync();
                return paystackResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string GenerateStaffReference()
        {
            return "EmusStaff_" + DateTime.Now.ToString().ToLower().Replace("am", "").Replace("pm", " ").Replace(":", "").Replace("/", "").Replace(" ", "");
        }
    }
}
