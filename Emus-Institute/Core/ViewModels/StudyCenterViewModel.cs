using static Core.DB.ECollegeEnums;

namespace Core.ViewModels
{
    public class StudyCenterViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? ContactPerson { get; set; }
        public string? ImagePath { get; set; }
        public StudyCenterApprovalStatus Status { get; set; }
        public string? StatusLabel { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateApproved { get; set; }
        public string? ApprovedByName { get; set; }
    }
}
