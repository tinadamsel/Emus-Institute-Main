using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Config
{
    public interface IGeneralConfiguration
    {
        public string AdminEmail { get; set; }
        public string DeveloperEmail { get; set; }
        public string PayStakApiKey { get; set; }
        public string PayStackBase { get; set; }
        public string CallbackUrl { get; set; }
    }
}
