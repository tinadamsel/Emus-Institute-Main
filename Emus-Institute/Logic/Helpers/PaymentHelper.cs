using Core.DB;
using Core.Models;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class PaymentHelper :IPaymentHelper
    {
        private readonly AppDbContext _context;
        private readonly IPaystackHelper _paystackHelper;

        public PaymentHelper(AppDbContext context, IPaystackHelper paystackHelper)
        {
            _context = context;
            _paystackHelper = paystackHelper;
        }
        public async Task<PaystackResponse> CreateStudentPayment(string UserId, ApplicationUser users)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserId))
                {
                    var getUser = _context.ApplicationUser.Where(x => x.Id == UserId && !x.Deactivated && !x.IsStudent && !x.Paid).FirstOrDefault();
                    if (getUser != null) 
                    {
                        //check if user payment exists
                        var checkIfUserPaymentExist = _context.Payments.Where(x => x.UserId == UserId).FirstOrDefault();
                        if (checkIfUserPaymentExist != null)
                        {
                            checkIfUserPaymentExist.PaymentDate = DateTime.Now;
                            checkIfUserPaymentExist.Status = PaymentStatus.Pending;
                            checkIfUserPaymentExist.PaymentMethod = "Paystack";
                            checkIfUserPaymentExist.Reference = GenerateNumber();
                            
                            var newPayment = _context.Update(checkIfUserPaymentExist);
                                             await _context.SaveChangesAsync();

                            if (newPayment.Entity.Id != Guid.Empty)
                            {
                                var paystackResponse = _paystackHelper.MakePayment(newPayment.Entity);
                                if (paystackResponse != null)
                                {
                                    var paystack = new PayStack
                                    {
                                        PaymentId = newPayment.Entity.Id,
                                        Payment = newPayment.Entity,
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
                            }
                        }
                        else
                        {
                            var payment = new Payment
                            {
                                PaymentDate = DateTime.Now,
                                Status = PaymentStatus.Pending,
                                PaymentMethod = "Paystack",
                                UserId = UserId,
                                DepartmentId = (int)getUser.DepartmentId,
                                Amount = (decimal?)400000.00,
                                Reference = GenerateNumber(),
                            };
                            var newPayment = await _context.AddAsync(payment);
                            await _context.SaveChangesAsync();

                            if (newPayment.Entity.Id != Guid.Empty)
                            {
                                var paystackResponse = _paystackHelper.MakePayment(newPayment.Entity);
                                if (paystackResponse != null)
                                {
                                    var paystack = new PayStack
                                    {
                                        PaymentId = newPayment.Entity.Id,
                                        Payment = newPayment.Entity,
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
                            }
                        }

                    }  
                }
                return null;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        private string GenerateNumber()
        {
            return "Emusinstitute_" + DateTime.Now.ToString().ToLower().Replace("am", "").Replace("pm", " ").Replace(":", "").Replace("/", "").Replace(" ", "");
        }


    }
}
