using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ExamQuestions : Basemodel
    {
        public string Question { get; set; }
        public string Answer { get; set; }

    }
}
