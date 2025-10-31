using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IPaymentHelper
    {
        Task<PaystackResponse> CreateStudentPayment(string UserId, ApplicationUser user);
    }
}
    