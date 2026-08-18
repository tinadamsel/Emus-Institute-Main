using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DB
{
    public class ECollegeEnums
    {
        public enum AcademicLevel
        {
            [Description("For Primary")]
            Primary = 1,
            [Description("For Secondary")]
            Secondary,
            [Description("For Tertiary")]
            Tertiary,
        }
        public enum StaffType
        {
            [Description("Academic Staff")]
            AcademicStaff = 1,
            [Description("Non Academic Staff")]
            NonAcademicStaff,
        }
        public enum CurrentSession
        {
            //[Description("For Primary one")]
            //PrimaryOne = 1,
            //[Description("For Primary Two")]
            //PrimaryTwo,
            //[Description("For Primary Three")]
            //PrimaryThree,
            //[Description("For Primary Four")]
            //PrimaryFour,
            //[Description("For Primary Five")]
            //PrimaryFive,
            //[Description("For Primary Six")]
            //PrimarySix,
            //[Description("For JS One")]
            //JSOne,
            //[Description("For JS Two")]
            //JSTwo,
            //[Description("For JS Three")]
            //JSThree,
            //[Description("For SS One")]
            //SSOne,
            //[Description("For SS Two")]
            //SSTwo,
            //[Description("For SS Three")]
            //SSThree,
            [Description("For Year One")]
            YearOne = 1,
            [Description("For Year Two")]
            YearTwo,
            [Description("For Year Three")]
            YearThree,
            [Description("For Year Four")]
            YearFour,
        }
        public enum DropdownEnums
        {
            [Description("For returning the user gender")]
            GenderKey = 1,
            //[Description("For returning the List of Banks")]
            //BankList = 2,
            //[Description("For returning the TertiaryAcademicSession")]
            //TertiaryAcademicSession = 3,
        }
        public enum TextbookEnum
        {
            [Description("For Year1")]
            Year1 = 1,
            [Description("For Year2")]
            Year2,
            [Description("For Year3")]
            Year3,
            [Description("For Year4")]
            Year4,
        }

        public enum TextbookApprovalStatus
        {
            [Description("Pending")]
            Pending = 1,
            [Description("Approved")]
            Approved = 2,
            [Description("Declined")]
            Declined = 3,
        }

        public enum LiveSessionStatus
        {
            [Description("Scheduled")]
            Scheduled = 1,
            [Description("Live")]
            Live = 2,
            [Description("Completed")]
            Completed = 3,
            [Description("Cancelled")]
            Cancelled = 4,
        }

        public enum CbtQuestionType
        {
            [Description("Multiple Choice (Single Answer)")]
            SingleChoice = 1,
            [Description("Multiple Correct Answers")]
            MultipleChoice = 2,
            [Description("True/False")]
            TrueFalse = 3,
        }

        public enum AnnouncementAudience
        {
            [Description("Whole Students")]
            WholeStudents = 1,
            [Description("Department")]
            Department = 2,
        }
        public enum PaymentStatus
        {
            [Description("For Pending")]
            Pending = 1,
            [Description("For Approved")]
            Approved = 2,
            [Description("For Rejected")]
            Rejected = 3,
        }
        public enum StaffStatus
        {
            [Description("For Pending")]
            Pending = 1,
            [Description("For Approved")]
            Approved = 2,
            [Description("For Rejected")]
            Rejected = 3,
        }
        public enum StaffPosition
        {
            [Description("Human Resource")]
            HumanResource = 1,
            [Description("ViceChancellor Academics")]
            ViceChancellorAcademics,
            [Description("Admission Officer")]
            AdmissionOfficer,
            [Description("Librarian Officer")]
            LibrarianOfficer,
            [Description("Account Officer")]
            AccountOfficer,
            [Description("Marketing Officer")]
            MarketingOfficer,
            [Description("Exams Officer")]
            ExamsOfficer,
            [Description("Business Development Officer")]
            BusinessDevOfficer,
            [Description("Public Relation Officer")]
            PublicRelOfficer,
            [Description("ViceChancellor Student Affairs")]
            ViceChancellorStudentAffairs,
            [Description("AcademicStaff")]
            AcademicStaff,

        }
        
        //public enum PrimaryYears
        //{
        //    [Description("For Primary One")]
        //    PrimaryOne = 1,
        //    [Description("For Primary Two")]
        //    PrimaryTwo,
        //    [Description("For Primary Three")]
        //    PrimaryThree,
        //    [Description("For Primary Four")]
        //    PrimaryFour,
        //    [Description("For Primary Five")]
        //    PrimaryFive,
        //    [Description("For Primary Six")]
        //    PrimarySix,
        //}
    }
}
