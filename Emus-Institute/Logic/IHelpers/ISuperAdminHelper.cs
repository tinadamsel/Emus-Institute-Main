using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface ISuperAdminHelper
    {
        bool CheckExistingDeptName(string name);
        bool CreateDepartment(DepartmentViewModel departmentViewModel);
        bool DeleteDept(int id);
        List<DepartmentViewModel> GetDepartments();
        DepartmentViewModel GetDeptToEdit(int id);
        int GetTotalDepartments();
        int GetTotalStaff();
        int GetTotalApprovedStudents();
        bool SaveEditedDept(DepartmentViewModel departmentViewModel);
        int GetTotalRegisteredStudents();
        List<ApplicationUserViewModel> GetAllRegisteredStudents();
        List<ApplicationUserViewModel> GetAllApprovedStudents();
        int GetTotalPaidStudents();
        List<ApplicationUserViewModel> GetAllPaidStudents();
        bool CheckIfStudentIsApproved(string userId);
        bool ApproveStudent(string userId);
        bool DeclineStudent(string userId);
        List<StaffDocumentationViewModel> GetPendingApplications();
        bool ApproveApplication(int id);
        bool RejectApplication(int id);
        //bool CheckExistingSubjectName(string name);
        //bool CreateSubject(SubjectViewModel subjectViewModel);
        //List<SubjectViewModel> GetSubjects();
        //SubjectViewModel GetSubjectToEdit(int id);
        //bool SaveEditedSubject(SubjectViewModel subjectViewModel);
        //bool DeleteSubject(int id);

    }
}
