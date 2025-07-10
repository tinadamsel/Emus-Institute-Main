using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Config
{
    public class GeneralConfiguration : IGeneralConfiguration
    {
        public string AdminEmail { get; set; }
        public string DeveloperEmail { get; set; }
    }
}
