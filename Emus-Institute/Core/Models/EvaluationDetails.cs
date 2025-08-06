using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class EvaluationDetails
    {
        public int Id { get; set; }
        public string? SchoolTranscript { get; set; }
        public string? HighSchoolCompletionCertificate { get; set; }
        public string? OtherCertificate { get; set; }
        public string? WAECScratchCard { get; set; }
        public string UserId { get; set; }
        [Display(Name = "User")]
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Users { get; set; }
        public DateTime? DateAdded { get; set; }
        public bool? isApproved { get; set; }
        public DateTime? DateApproved { get; set; }
    }
}
