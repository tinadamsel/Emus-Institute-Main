using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models
{
    public class PaystackResponse
    {
        
        public bool status { get; set; }
        public string message { get; set; }
        public PaystackData data { get; set; }
        public PayStack Paystacks { get; set; }

        public class PaystackData
        {
            public string id { get; set; }
            public int amount { get; set; }
            public string currency { get; set; }
            public DateTime transaction_date { get; set; }
            public string status { get; set; }
            public string reference { get; set; }
            public string domain { get; set; }
            public string gateway_response { get; set; }
            public string message { get; set; }
            public string channel { get; set; }
            public string ip_address { get; set; }
            public int? fees { get; set; }
            public string authorization_url { get; set; }
            public string access_code { get; set; }
            public PaystackLog log { get; set; }
            public PaystackAuthorization authorization { get; set; }
            public PaystackCustomer customer { get; set; }
            public PaystackMetadata metadata { get; set; }
        }

        public class PaystackAuthorization
        {
            public string authorization_code { get; set; }
            public string bin { get; set; }
            public string last4 { get; set; }
            public string exp_month { get; set; }
            public string exp_year { get; set; }
            public string channel { get; set; }
            public string card_type { get; set; }
            public string bank { get; set; }
            public string country_code { get; set; }
            public string brand { get; set; }
            public bool reusable { get; set; }
            public string signature { get; set; }
        }

        public class PaystackCustomer
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string customer_code { get; set; }
            public string phone { get; set; }
            public string risk_action { get; set; }
        }

        public class PaystackLog
        {
            public int time_spent { get; set; }
            public int attempts { get; set; }
            public object authentication { get; set; }
            public int errors { get; set; }
            public bool success { get; set; }
            public bool mobile { get; set; }
            public List<object> input { get; set; }
            public string channel { get; set; }
            public List<PaystackHistory> history { get; set; }
        }

        public class PaystackHistory
        {
            public string type { get; set; }
            public string message { get; set; }
            public int time { get; set; }
        }

        public class PaystackMetadata
        {
            public string referrer { get; set; }
            public List<PaystackCustomField> custom_fields { get; set; }
        }

        public class PaystackCustomField
        {
            public string display_name { get; set; }
            public string variable_name { get; set; }
            public string value { get; set; }
        }

    }
}
