using EmployeeManagementSystemGenerics.Common;
using EmployeeManagementSystemGenerics.Delegates;
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

        // =========================
        // EMPLOYEES and ONBOARDING
        // =========================

        public Result<Employee> AddEmployee(Employee employee)
        {
            if (employee.Id <= 0)
            {
                return Result<Employee>.Fail("Employee Id must be greate than zero.");
            }
            if(string.IsNullOrWhiteSpace(employee.Name))
            {
                return Result<Employee>.Fail("Employee name is required.");
            }
            if (employee.Salary < 0)
            {
                return Result<Employee>.Fail("Salary cannot be negative.");
            }
            if(! _departments.ContainsKey(employee.DepartmentId))
            {
                return Result<Employee>.Fail("Department doesn't exist.");
            }
            if(EmployeeIdExists(employee.Id))
            {
                return Result<Employee>.Fail($"Employee with Id {employee.Id} already exists.");
            }

            _onboardingQueue.Enqueue(employee);

            AddToHistory($"Employee added to onboarding queue: {employee.Name}");

            return Result<Employee>.Ok(employee, "Employee added to onboarding queue successfully");
        }

        public Result<Employee> ProcessNextOnboarding()
        {
            if (_onboardingQueue.Count == 0)
            {
                return Result<Employee>.Fail("Onboarding queue is empty");
            }

            Employee employee = _onboardingQueue.Dequeue();

            _employees.Add(employee);

            AddToHistory($"Employee onboarded: {employee.Name}");

            EmployeeOnboarded?.Invoke(this, new EmployeeEventArgs(employee));

            return Result<Employee>.Ok(employee, "Employee Onboarded successfully");
        }


        // ==========================
        // SEARCH
        // ==========================

        public Employee? FindEmployeeById(int id)
        {
            foreach (Employee employee in _employees)
            {
                if (employee.Id == id)
                {
                    return employee;
                }
            }

            return null;
        }

        public Employee? FindEmployeeByName(string name)
        {
            foreach(Employee employee in _employees)
            {
                if (employee.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return employee;
                }
            }

            return null;
        }


        // ==========================
        // DEPARTMENT EMPLOYEES
        // ==========================

        public Result<List<Employee>> GetEmployeesByDepartment(int departmentId)
        {
            if (! _departments.ContainsKey(departmentId))
            {
                return Result<List<Employee>>.Fail("Department doesn't exist");
            }

            List<Employee> result = new List<Employee>();   

            foreach (Employee employee in _employees)
            {
                if (employee.DepartmentId == departmentId)
                {
                    result.Add(employee);
                }
            }

            return Result<List<Employee>>.Ok(result, $"Found {result.Count} employee(s)");
        }


        // ==========================
        // DELEGATE FILTER
        // ==========================

        public List<Employee> FilterEmployees(EmployeeFilter filter)
        {
            List<Employee> result = new List<Employee>();

            foreach (Employee employee in _employees)
            {
                if (filter(employee))
                {
                    result.Add(employee);
                }
            }

            return result;
        }

        // ==========================
        // PROMOTION
        // ==========================

        public Result<Manager> PromoteEmployeeToManager(int employeeId)
        {
            for (int i = 0; i < _employees.Count; i++)
            {
                Employee employee = _employees[i];

                if(employee.Id == employeeId)
                {
                    if (employee is Manager)
                    {
                        return Result<Manager>.Fail("Employee is already a manager");
                    }

                    Manager manager = new Manager(
                        employee.Id,
                        employee.Name,
                        employee.HireDate,
                        employee.DepartmentId,
                        employee.Salary);

                    _employees[i] = manager;

                    AddToHistory($"Employee promoted: {manager.Name}");

                    EmployeePromoted?.Invoke(this, new EmployeeEventArgs(manager));

                    return Result<Manager>.Ok(manager, "Employee promoted successfully."); 
                }
            }

            return Result<Manager>.Fail("Employee not found");
        }

        // ==========================
        // SKILLS
        // ==========================

        public Result<string> RegisterSkillForEmployee(int employeeId, string skill)
        {
            Employee? employee = FindEmployeeById(employeeId);  

            if (employee == null)
            {
                return Result<string>.Fail("Employee not found");
            }

            if (string.IsNullOrWhiteSpace(skill))
            {
                return Result<string>.Fail("Skill cannot be empty");
            }

            bool added = _uniqueSkills.Add(skill);

            if (!added)
            {
                return Result<string>.Fail($"Skill '{skill}' already exists.");
            }

            AddToHistory($"{employee.Name} registered skill: {skill}");

            return Result<string>.Ok(skill, "Skill registered successfully");
        }

        // ==========================
        // AVERAGE SALARY
        // ==========================

        public decimal CalculateAverageSalary()
        {
            if (_employees.Count == 0)
            {
                return 0;
            }

            dedcimal totalSalary = 0;

            foreach (Employee employee in _employees)
            {
                totalSalary += employee.Salary;
            }

            return totalSalary / _employees.Count;
        }

        // ==========================
        // DEPARTMENT REPORT
        // ==========================

        public Dictionary<int, int> GetEmployeeCountPerDepartment()
        {
            Dictonary<int, int> report = new Dictionary<int, int>();

            foreach (KeyValuePair<int, Department> department in _departments)
            {
                reports.Add(department.Key, 0);
            }

            foreach (Employee employee in _employees)
            {
                if (report.ContainsKey(employee.DepartmentId))
                {
                    report[employee.DepartmentId]++;
                }
            }
            return report;
        }


        // ==========================
        // ACTION HISTORY
        // ==========================

        public List<string> GetActionHistoryNewestFirst()
        {
            List<string> history = new List<string>();

            foreach (string action in _actionHistory)
            {
                history.Add(action);
            }
            return history;
        }

        // ==========================
        // ACTIVE EMPLOYEES
        // ==========================

        public List<Employee> GetAllEmployees()
        {
            List<Employee> result = new List<Employee>();

            foreach (Employee employee in _employees)
            {
                result.Add(employee);
            }
            return result;
        }


        private bool EmployeeIdExists(int id)
        {
            foreach (Employee employee in _employees)
            {
                if (employee.Id == id)
                {
                    return true;
                }
            }


            foreach (Employee employee in _onboardingQueue)
            {
                if (employee.Id == id)
                {
                    return true;
                }
            }

            return false;
        }


        private void AddToHistory(string action)
        {
            string record =
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {action}";

            _actionHistory.Push(record);
        }
    }
}
