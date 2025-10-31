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
    public class EvaluationDetailsViewModel
    {
        public int Id { get; set; }
        public string? Passport { get; set; }
        public string? SchoolTranscript { get; set; }
        public string? HighSchoolCompletionCertificate { get; set; }
        public string? OtherCertificate { get; set; }
        public string? WAECScratchCard { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser? Users { get; set; }
        public DateTime? DateAdded { get; set; }
        public bool? isApproved { get; set; }
        public DateTime? DateApproved { get; set; }
    }
}
