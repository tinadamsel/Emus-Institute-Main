using Core.Config;
using Core.DB;
using Core.Models;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Core.DB.ECollegeEnums;
using static Core.Models.PaystackResponse;
using Method = RestSharp.Method;
using Formatting = Newtonsoft.Json.Formatting;


namespace Logic.Helpers
{
    public class PaystackHelper : IPaystackHelper
    {
        private readonly AppDbContext _context;
        protected RestRequest request;
        private RestClient client;
        static string ApiEndPoint = "";
        private readonly IGeneralConfiguration _generalConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaystackHelper(AppDbContext context, IGeneralConfiguration generalConfiguration, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _generalConfiguration = generalConfiguration;
            client = new RestClient(_generalConfiguration.PayStackBase);
            _userManager = userManager;
        }

        public PaystackResponse MakePayment(Payment payment)
        {
            try
            {
                PaystackResponse PaystackResponse = null;
                if (payment.Amount > 0 && !string.IsNullOrEmpty(payment.UserId))
                {
                    var referenceId = payment.Reference;
                    long milliseconds = DateTime.Now.Ticks;
                    string testid = milliseconds.ToString();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ApiEndPoint = "/transaction/initialize";
                    request = new RestRequest(ApiEndPoint, Method.Post);
                    request.AddHeader("accept", "application/json");
                    request.AddHeader("Authorization", "Bearer " + _generalConfiguration.PayStakApiKey);
                    request.AddParameter("reference", referenceId);
                    request.AddParameter("callback_url", _generalConfiguration.CallbackUrl);
                    request.AddParameter("currency", "NGN");
                    var total = payment.Amount;
                    request.AddParameter("amount", (total * 100).ToString());
                    var user = FindByIdAsync(payment.UserId).Result;
                    request.AddParameter("email", user.Email);
                    request.AddParameter("customer", user.Email);
                    List<PaystackCustomField> myCustomfields = new List<PaystackCustomField>();
                    PaystackCustomField nameCustomeField = new PaystackCustomField()
                    {
                        display_name = "Email",
                        variable_name = "Email",
                        value = user?.Email,
                    };
                    myCustomfields.Add(nameCustomeField);
                    Dictionary<string, List<PaystackCustomField>> metadata = new Dictionary<string, List<PaystackCustomField>>();
                    metadata.Add("custom_fields", myCustomfields);
                    var serializedMetadata = JsonConvert.SerializeObject(metadata);
                    request.AddParameter("metadata", serializedMetadata);
                    request.AddParameter("email", user?.Email);
                    var serializedRequest = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    var result = client.ExecuteAsync(request).Result;
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        PaystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(result.Content);
                    }
                }
                return PaystackResponse;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public async Task<PaystackResponse> VerifyPayment(PayStack payment)
        {
            PaystackResponse PaystackResponse = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ApiEndPoint = "/transaction/verify/" + payment.Reference;
                request = new RestRequest(ApiEndPoint, Method.Get);
                request.AddHeader("accept", "application/json");
                request.AddHeader("Authorization", "Bearer " + _generalConfiguration.PayStakApiKey);
                var result = client.ExecuteAsync(request).Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    PaystackResponse = JsonConvert.DeserializeObject<PaystackResponse>(result.Content);
                    var payStack = UpdateResponse(PaystackResponse);
                    PaystackResponse.Paystacks = payStack;
                }
                return PaystackResponse;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        public PayStack UpdateResponse(PaystackResponse paystackResponse)
        {
            try
            {
                PayStack _paystackEntity = _context.PayStackpayments.Where(p => p.Reference == paystackResponse.data.reference).FirstOrDefault();
                if (_paystackEntity != null)
                {
                    _paystackEntity.Bank = paystackResponse.data.authorization.bank;
                    _paystackEntity.Brand = paystackResponse.data.authorization.brand;
                    _paystackEntity.Card_type = paystackResponse.data.authorization.card_type;
                    _paystackEntity.Channel = paystackResponse.data.channel;
                    _paystackEntity.Country_code = paystackResponse.data.authorization.country_code;
                    _paystackEntity.Currency = paystackResponse.data.currency;
                    _paystackEntity.Domain = paystackResponse.data.domain;
                    _paystackEntity.Exp_month = paystackResponse.data.authorization.exp_month;
                    _paystackEntity.Exp_year = paystackResponse.data.authorization.exp_year;
                    _paystackEntity.Fees = paystackResponse.data.fees.ToString();
                    _paystackEntity.Gateway_response = paystackResponse.data.gateway_response;
                    _paystackEntity.Ip_Address = paystackResponse.data.ip_address;
                    _paystackEntity.Last4 = paystackResponse.data.authorization.last4;
                    _paystackEntity.Message = paystackResponse.message;
                    _paystackEntity.Reference = paystackResponse.data.reference;
                    _paystackEntity.Reusable = paystackResponse.data.authorization.reusable;
                    _paystackEntity.Signature = paystackResponse.data.authorization.signature;
                    _paystackEntity.Transaction_date = paystackResponse.data.transaction_date;
                    _context.Update(_paystackEntity);
                    _context.SaveChanges();
                    if (_paystackEntity.PaymentId != Guid.Empty)
                    {
                        var payment = _context.Payments.Where(x => x.Id == _paystackEntity.PaymentId && x.Status == PaymentStatus.Pending).FirstOrDefault();
                        if (payment != null)
                        {
                            payment.Status = PaymentStatus.Approved;
                            payment.IsTextbookFeePaid = true;
                            payment.ApprovedById = "Admin";
                            payment.Details = "School fees & Course Payment";
                            _context.Update(payment);
                            _context.SaveChanges();

                            var updateUserDetails = _context.ApplicationUser.Where(a => a.Id == payment.UserId && !a.Deactivated && !a.IsStudent).FirstOrDefault();
                            if (updateUserDetails != null)
                            {
                                updateUserDetails.Paid = true;
                                updateUserDetails.IsStudent = true;
                                updateUserDetails.DateModified = DateTime.Now;
                                updateUserDetails.DateOfApproval = DateTime.Now;

                                _context.Update(updateUserDetails);
                                _context.SaveChanges();
                            }

                            var updateEvaluationDetails = _context.EvaluationDetails.Where(x => x.UserId == payment.UserId).FirstOrDefault();
                            if (updateEvaluationDetails != null)
                            {
                                updateEvaluationDetails.isApproved = true;
                                updateEvaluationDetails.DateApproved = DateTime.Now;

                                _context.Update(updateEvaluationDetails);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
                return _paystackEntity;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string Id)
        {
            return await _userManager.Users.Where(s => s.Id == Id)?.FirstOrDefaultAsync();
        }

    }
}
