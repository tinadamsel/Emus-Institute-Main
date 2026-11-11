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

        public async Task<(bool Success, string Message, string? AuthorizationUrl)> CreateIntakeTextbookPayment(string userId, string dataPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(dataPhoneNumber))
            {
                return (false, "Please enter a valid mobile number for data.", null);
            }

            var user = _context.ApplicationUser.FirstOrDefault(x =>
                x.Id == userId && x.IsIntake && x.IsStudent && !x.Deactivated && !x.IsAdmin);
            if (user == null)
            {
                return (false, "Only intake students can make this payment.", null);
            }

            if (_context.IntakeTextbookPayments.Any(x => x.UserId == userId && x.IsPaid))
            {
                return (false, "You have already paid for textbook and data.", null);
            }

            var amount = _generalConfiguration.IntakeTextbookAmountNgn > 0
                ? _generalConfiguration.IntakeTextbookAmountNgn
                : 12000m;

            var payment = new Payment
            {
                PaymentDate = DateTime.Now,
                Status = PaymentStatus.Pending,
                PaymentMethod = "Paystack",
                UserId = userId,
                DepartmentId = user.DepartmentId,
                Amount = amount,
                Reference = "EmusIntake_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Details = "Intake Textbook and Data Payment"
            };

            var newPayment = await _context.AddAsync(payment);
            await _context.SaveChangesAsync();

            var paystackResponse = _paystackHelper.MakeIntakeTextbookPayment(newPayment.Entity, user.Email);
            if (paystackResponse?.data?.authorization_url == null)
            {
                var paystackMsg = paystackResponse?.message;
                return (false, string.IsNullOrWhiteSpace(paystackMsg)
                    ? "Unable to start payment. Please try again or contact admin."
                    : $"Unable to start payment: {paystackMsg}", null);
            }

            var intakeRecord = _context.IntakeTextbookPayments.FirstOrDefault(x => x.UserId == userId && !x.IsPaid);
            if (intakeRecord == null)
            {
                intakeRecord = new IntakeTextbookPayment
                {
                    UserId = userId,
                    DateCreated = DateTime.Now
                };
                _context.IntakeTextbookPayments.Add(intakeRecord);
            }

            intakeRecord.DataPhoneNumber = dataPhoneNumber.Trim();
            intakeRecord.PaymentId = newPayment.Entity.Id;
            intakeRecord.Amount = amount;
            intakeRecord.IsPaid = false;
            intakeRecord.PaidAt = null;

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

            return (true, "Redirecting to Paystack to complete your textbook and data payment.", paystackResponse.data.authorization_url);
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
