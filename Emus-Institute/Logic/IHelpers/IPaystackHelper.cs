using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IPaystackHelper
    {
        PaystackResponse MakePayment(Payment payment);
        Task<PaystackResponse> VerifyPayment(PayStack payment);
    }
}
