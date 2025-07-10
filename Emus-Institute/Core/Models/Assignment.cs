using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Assignment : Basemodel
    {
        public string Description { get; set; }
        public bool? IsSubmitted { get; set; }
        public DateTime ValidUntilDate { get; set; }
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Department? Courses { get; set; }
    }
}
