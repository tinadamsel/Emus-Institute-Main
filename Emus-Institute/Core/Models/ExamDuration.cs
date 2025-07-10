using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ExamDuration
    {
        public int Id { get; set; }
        public ExamType Type { get; set; }
        public int Duration { get; set; }
    }
    public enum ExamType
    {
        [Description("Student Promotional Exam")]
        StudentPromotionalExam = 1,
        [Description("Staff Promotional Exam")]
        StaffPromotionalExam = 2,
    }
}
