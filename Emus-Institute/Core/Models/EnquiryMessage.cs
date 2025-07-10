using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class EnquiryMessage : Basemodel
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}
