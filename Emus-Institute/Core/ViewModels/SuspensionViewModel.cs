using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class SuspensionViewModel
    {
        public int Id { get; set; }
        public string SuspensionReason { get; set; }
        public string Name { get; set; }
        public string SuspensionDuration { get; set; }
        public bool IsSuspended { get; set; }
        public bool? IsRemoved { get; set; }
        public DateTime DateSuspended { get; set; }
        public DateTime? DateRemoved { get; set; }
        public DateTime? ValidUntilDate { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser Users { get; set; }
    }
}
