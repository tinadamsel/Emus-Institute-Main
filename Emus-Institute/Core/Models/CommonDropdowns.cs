
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Core.Models
{
	public class CommonDropdowns : Basemodel
    {
        public int DropdownKey { get; set; }
        [Display(Name = " Code")]
        public int? Code { get; set; }
    }

}
