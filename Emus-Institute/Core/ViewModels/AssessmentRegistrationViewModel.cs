using static Core.DB.ECollegeEnums;

namespace Core.ViewModels
{
    public class AssessmentRegistrationViewModel
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public AssessmentProgramType ProgramType { get; set; }
        public string? Country { get; set; }
        public AssessmentScholarshipType ScholarshipType { get; set; }
        public int CbtTestId { get; set; }
        public string? TestTitle { get; set; }
        public decimal AmountGbp { get; set; } = 6m;
    }
}
