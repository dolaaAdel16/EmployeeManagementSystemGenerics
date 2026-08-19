using EmployeeManagementSystemGenerics.Common;
using EmployeeManagementSystemGenerics.Events;
using EmployeeManagementSystemGenerics.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystemGenerics.Services
{
    public class Company
    {
        private readonly List<Employee> _employees;
        private readonly Dictionary<int, Department> _departments;
        private readonly Queue<Employee> _onboardingQueue;
        private readonly Stack<string> _actionHistory;
        private readonly HashSet<string> _uniqueSkills;

        private event EventHandler<EmployeeEventArgs>? EmployeeOnboarded;
        private event EventHandler<EmployeeEventArgs>? EmployeePromoted;

        public Company()
        {
            _employees = new List<Employee>();  
            _departments = new Dictionary<int, Department>();   
            _onboardingQueue = new Queue<Employee>();   
            _actionHistory = new Stack<string>();
            _uniqueSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        // =====================
        // DEPARTMENTS
        // =====================

        public Result<Department> AddDepartment(Department department)
        {
            if (department.Id <= 0)
            {
                return Result<Department>.Fail("Department Id must be greater than zero");
            }
            if (string.IsNullOrWhiteSpace(department.Name))
            {
                return Result<Department>.Fail("Department name is required");
            }
            if(_departments.ContainsKey(department.Id))
            {
                return Result<Department>.Fail($"Department with Id : {department.Id} already exists");
            }

            _departments.Add(department.Id, department);

            AddToHistory($"Department addded: {department.Name}");

            return Result<Department>.Ok(department, "Department added successfully.");
        }

        public Department? GetDepartmentById(int id)
        {
            Department? department;

            if (_departments.TryGetValue(id, out department))
            {
                return department;
            }

            return null;
        }

        public List<Department> GetAllDepartments()
        {
            List<Department> result = new List<Department>();

            foreach(KeyValuePair<int, Department> item in _departments)
            {
                result.Add(item.Value);
            }

            return result;
        }



        private void AddToHistory(string action)
        {
            string record =
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {action}";

            _actionHistory.Push(record);
        }
    }
}
