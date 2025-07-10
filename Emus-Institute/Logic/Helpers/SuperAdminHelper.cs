using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Helpers
{
    public class SuperAdminHelper : ISuperAdminHelper
    {
        private readonly AppDbContext _context;

        public SuperAdminHelper(AppDbContext context)
        {
            _context = context;
        }

        public bool CheckExistingDeptName(string name)
        {
            if (name != null)
            {
                var checkDeptName = _context.Departments.Where(x => x.Name == name && x.Active && !x.Deleted).FirstOrDefault();
                if (checkDeptName != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CreateDepartment(DepartmentViewModel departmentViewModel)
        {
            if (departmentViewModel != null)
            {
                var department = new Department()
                {
                    Name = departmentViewModel.Name,
                    Description = departmentViewModel.Description,
                    DateCreated = DateTime.Now,
                    Active = true,
                    Deleted = false,
                };
                _context.Departments.Add(department);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<DepartmentViewModel> GetDepartments()
        {
            var departmentViewModel = new List<DepartmentViewModel>();
            departmentViewModel = _context.Departments.Where(x => x.Id > 0 && x.Active && !x.Deleted)
                .Select(x => new DepartmentViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    DateCreated = x.DateCreated,
                }).ToList();
            return departmentViewModel;
        }

        public int GetTotalStudents()
        {
            return _context.ApplicationUser.Where(a => a.Id != null && !a.Deactivated && a.IsStudent == true).Count();
        }
        public int GetTotalDepartments()
        {
            return _context.Departments.Where(x => x.Id > 0 && x.Active).Count();
        }
        public int GetTotalStaff()
        {
            return _context.StaffDocuments.Where(a => a.Id > 0 && a.Active).Count();
        }
       
        public DepartmentViewModel GetDeptToEdit(int id)
        {
            var departmentToEdit = _context.Departments.Where(a => a.Id == id && a.Active && !a.Deleted)
                .Select(a => new DepartmentViewModel()
                {
                    Name = a.Name,
                    Id = a.Id,
                    Description = a.Description,
                    DateCreated = a.DateCreated,
                }).FirstOrDefault();
                if (departmentToEdit != null)
                {
                    return departmentToEdit;
                }
                return null;
        }
        
        public bool SaveEditedDept(DepartmentViewModel departmentViewModel)
        {
            if (departmentViewModel != null) 
            { 
                var editDept = _context.Departments.Where(x => x.Id == departmentViewModel.Id && x.Active && !x.Deleted).FirstOrDefault();
                if (editDept != null)
                {
                    editDept.Name = departmentViewModel.Name;
                    editDept.Description = departmentViewModel.Description;
                }
                _context.Update(editDept);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        
        public bool DeleteDept(int id)
        {
            var deptToDelete = _context.Departments.Where(a => a.Id == id && a.Active && !a.Deleted).FirstOrDefault();
            if (deptToDelete != null)
            {
                deptToDelete.Active = false;
                deptToDelete.Deleted = true;
                _context.Update(deptToDelete);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

    }
}
