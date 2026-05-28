using Core.Config;
using Core.DB;
using Core.Models;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using static Core.DB.ECollegeEnums;
using static Core.Models.PaystackResponse;
using Formatting = Newtonsoft.Json.Formatting;
using Method = RestSharp.Method;

namespace Logic.Helpers
{
    public class StaffPaystackHelper : IStaffPaystackHelper
    {
        private readonly AppDbContext _context;
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly RestClient _client;

        public StaffPaystackHelper(
            AppDbContext context,
            IGeneralConfiguration generalConfiguration,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _generalConfiguration = generalConfiguration;
            _userManager = userManager;
            _emailService = emailService;
            _client = new RestClient(_generalConfiguration.PayStackBase);
        }

        public PaystackResponse MakeStaffPayment(Payment payment)
        {
            try
            {
                if (payment.Amount <= 0 || string.IsNullOrEmpty(payment.UserId))
                {
                    return null;
                }

                var referenceId = payment.Reference;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = new RestRequest("/transaction/initialize", Method.Post);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", "Bearer " + _generalConfiguration.PayStakApiKey);
                request.AddParameter("reference", referenceId);
                request.AddParameter("callback_url", _generalConfiguration.StaffCallbackUrl);
                request.AddParameter("currency", "NGN");
                request.AddParameter("amount", ((payment.Amount ?? 0) * 100).ToString());

                var user = FindByIdAsync(payment.UserId).Result;
                if (user == null)
                {
                    return null;
                }

                request.AddParameter("email", user.Email);
                request.AddParameter("customer", user.Email);

                var customFields = new List<PaystackCustomField>
                {
                    new PaystackCustomField
                    {
                        display_name = "Staff Email",
                        variable_name = "StaffEmail",
                        value = user.Email,
                    }
                };
                var metadata = new Dictionary<string, List<PaystackCustomField>>
                {
                    { "custom_fields", customFields }
                };
                request.AddParameter("metadata", JsonConvert.SerializeObject(metadata));

                var result = _client.ExecuteAsync(request).Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<PaystackResponse>(result.Content);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaystackResponse> VerifyStaffPayment(PayStack payment)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = new RestRequest("/transaction/verify/" + payment.Reference, Method.Get);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", "Bearer " + _generalConfiguration.PayStakApiKey);

                var result = await _client.ExecuteAsync(request);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                var paystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(result.Content);
                paystackResponse.Paystacks = UpdateStaffPaymentResponse(paystackResponse);
                return paystackResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private PayStack UpdateStaffPaymentResponse(PaystackResponse paystackResponse)
        {
            var paystackEntity = _context.PayStackpayments
                .FirstOrDefault(p => p.Reference == paystackResponse.data.reference);

            if (paystackEntity == null)
            {
                return null;
            }

            paystackEntity.Bank = paystackResponse.data.authorization.bank;
            paystackEntity.Brand = paystackResponse.data.authorization.brand;
            paystackEntity.Card_type = paystackResponse.data.authorization.card_type;
            paystackEntity.Channel = paystackResponse.data.channel;
            paystackEntity.Country_code = paystackResponse.data.authorization.country_code;
            paystackEntity.Currency = paystackResponse.data.currency;
            paystackEntity.Domain = paystackResponse.data.domain;
            paystackEntity.Exp_month = paystackResponse.data.authorization.exp_month;
            paystackEntity.Exp_year = paystackResponse.data.authorization.exp_year;
            paystackEntity.Fees = paystackResponse.data.fees.ToString();
            paystackEntity.Gateway_response = paystackResponse.data.gateway_response;
            paystackEntity.Ip_Address = paystackResponse.data.ip_address;
            paystackEntity.Last4 = paystackResponse.data.authorization.last4;
            paystackEntity.Message = paystackResponse.message;
            paystackEntity.Reference = paystackResponse.data.reference;
            paystackEntity.Reusable = paystackResponse.data.authorization.reusable;
            paystackEntity.Signature = paystackResponse.data.authorization.signature;
            paystackEntity.Transaction_date = paystackResponse.data.transaction_date;
            _context.Update(paystackEntity);
            _context.SaveChanges();

            if (paystackEntity.PaymentId == Guid.Empty)
            {
                return paystackEntity;
            }

            var staffPayment = _context.Payments
                .FirstOrDefault(x => x.Id == paystackEntity.PaymentId
                    && x.Status == PaymentStatus.Pending
                    && x.Details == "Staff Evaluation Payment");

            if (staffPayment == null)
            {
                return paystackEntity;
            }

            staffPayment.Status = PaymentStatus.Approved;
            staffPayment.ApprovedById = "Admin";
            _context.Update(staffPayment);
            _context.SaveChanges();

            var staffUser = _context.ApplicationUser
                .FirstOrDefault(a => a.Id == staffPayment.UserId && !a.Deactivated && a.IsAdmin);

            if (staffUser != null)
            {
                staffUser.Paid = true;
                staffUser.DateModified = DateTime.Now;
                _context.Update(staffUser);
                _context.SaveChanges();
            }

            var staffEvaluation = _context.StaffEvaluationDetails
                .FirstOrDefault(x => x.UserId == staffPayment.UserId);

            if (staffEvaluation != null)
            {
                staffEvaluation.IsApproved = true;
                staffEvaluation.DateApproved = DateTime.Now;
                _context.Update(staffEvaluation);
                _context.SaveChanges();
            }

            var updateStaffDocument = _context.StaffDocuments.FirstOrDefault(x => x.UserId == staffEvaluation.UserId);
            if (updateStaffDocument != null)
            {
                updateStaffDocument.IsApproved = true;
                updateStaffDocument.DateOfApproval = DateTime.Now;
                updateStaffDocument.ApprovedbyId = "Admin";
                updateStaffDocument.StaffStatus = StaffStatus.Approved;
                _context.Update(updateStaffDocument);
                _context.SaveChanges();
            }

            return paystackEntity;
        }

        private async Task<ApplicationUser> FindByIdAsync(string id)
        {
            return await _userManager.Users.Where(s => s.Id == id).FirstOrDefaultAsync();
        }
    }
}
