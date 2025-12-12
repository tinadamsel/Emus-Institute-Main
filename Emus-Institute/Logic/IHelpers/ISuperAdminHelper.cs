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
        List<StaffDocumentationViewModel> GetApprovedStaff();
        bool SuspendUser(string userId);
        List<SuspensionViewModel> GetSuspendedUsers();
        bool DeactivateUser(string userId);
        bool RemoveSuspension(int id);
        
    }
}
