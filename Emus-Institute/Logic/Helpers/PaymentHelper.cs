using Core.Config;
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
        private readonly IGeneralConfiguration _generalConfiguration;

        public PaymentHelper(AppDbContext context, IPaystackHelper paystackHelper, IGeneralConfiguration generalConfiguration)
        {
            _context = context;
            _paystackHelper = paystackHelper;
            _generalConfiguration = generalConfiguration;
        }

        private decimal GetStudentEvaluationAmount(ApplicationUser user)
        {
            if (user.IsCohort == true)
            {
                return _generalConfiguration.CohortEvaluationAmountNgn > 0
                    ? _generalConfiguration.CohortEvaluationAmountNgn
                    : 200000m;
            }

            return _generalConfiguration.StudentEvaluationAmountNgn > 0
                ? _generalConfiguration.StudentEvaluationAmountNgn
                : 400000m;
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
                        var evaluationAmount = GetStudentEvaluationAmount(getUser);
                        var checkIfUserPaymentExist = _context.Payments.Where(x => x.UserId == UserId).FirstOrDefault();
                        if (checkIfUserPaymentExist != null)
                        {
                            checkIfUserPaymentExist.PaymentDate = DateTime.Now;
                            checkIfUserPaymentExist.Status = PaymentStatus.Pending;
                            checkIfUserPaymentExist.PaymentMethod = "Paystack";
                            checkIfUserPaymentExist.Reference = GenerateNumber();
                            checkIfUserPaymentExist.Amount = evaluationAmount;
                            
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
                                Amount = evaluationAmount,
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

        public async Task<PaystackResponse> CreateAssessmentPayment(AssessmentRegistration registration, decimal amountNgn)
        {
            var payment = new Payment
            {
                PaymentDate = DateTime.Now,
                Status = PaymentStatus.Pending,
                PaymentMethod = "Paystack",
                UserId = null,
                DepartmentId = null,
                Amount = amountNgn,
                Reference = "EmusAssessment_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Details = "Public Assessment Payment"
            };

            var newPayment = await _context.AddAsync(payment);
            await _context.SaveChangesAsync();

            var paystackResponse = _paystackHelper.MakeAssessmentPayment(newPayment.Entity, registration.Email);
            if (paystackResponse?.data?.authorization_url == null)
            {
                return paystackResponse;
            }

            registration.PaymentId = newPayment.Entity.Id;
            _context.Update(registration);

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

        private string GenerateNumber()
        {
            return "Emusinstitute_" + DateTime.Now.ToString().ToLower().Replace("am", "").Replace("pm", " ").Replace(":", "").Replace("/", "").Replace(" ", "");
        }


    }
}
