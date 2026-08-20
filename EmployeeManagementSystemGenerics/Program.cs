using EmployeeManagementSystemGenerics.Common;
using EmployeeManagementSystemGenerics.Events;
using EmployeeManagementSystemGenerics.Models;
using EmployeeManagementSystemGenerics.Services;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace EmployeeManagementSystemGenerics
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Company company = new Company();


            SeedData(company);
            // Subscribe to Events
            company.EmployeeOnboarded += OnEmployeeOnboarded;

            company.EmployeePromoted += OnEmployeePromoted;




            bool running = true;


            do
            {
                ShowMenu();

                int choice =
                    ReadInt("Choose an option: ");

                Console.WriteLine();


                switch (choice)
                {
                    case 1:
                        AddDepartment(company);
                        break;

                    case 2:
                        AddEmployee(company);
                        break;

                    case 3:
                        ProcessOnboarding(company);
                        break;

                    case 4:
                        PromoteEmployee(company);
                        break;

                    case 5:
                        RegisterSkill(company);
                        break;

                    case 6:
                        SearchEmployee(company);
                        break;

                    case 7:
                        ShowDepartmentEmployees(company);
                        break;

                    case 8:
                        FilterEmployees(company);
                        break;

                    case 9:
                        ShowAverageSalary(company);
                        break;

                    case 10:
                        ShowDepartmentReport(company);
                        break;

                    case 11:
                        ShowActionHistory(company);
                        break;

                    case 12:
                        ShowUniqueSkills(company);
                        break;

                    case 13:
                        ShowAllEmployees(company);
                        break;

                    case 0:
                        running = false;
                        break;

                    default:
                        Console.WriteLine(
                            "Invalid menu option."
                        );
                        break;
                }


                Console.WriteLine();

            } while (running);


            // Unsubscribe from Events
            company.EmployeeOnboarded -= OnEmployeeOnboarded;

            company.EmployeePromoted -= OnEmployeePromoted;


            Console.WriteLine("Program closed.");
        }

        // =====================================
        // ADD DEPARTMENT
        // =====================================

        private static void AddDepartment(
            Company company)
        {
            int id =
                ReadInt("Department Id: ");

            string name =
                ReadString("Department Name: ");


            Department department =
                new Department(id, name);


            Result<Department> result =
                company.AddDepartment(department);


            PrintResult(result);
        }


        // =====================================
        // ADD EMPLOYEE
        // =====================================

        private static void AddEmployee(
            Company company)
        {
            Console.WriteLine(
                "Available Departments:"
            );

            List<Department> departments =
                company.GetAllDepartments();


            foreach (
                Department department
                in departments)
            {
                Console.WriteLine(department);
            }


            Console.WriteLine();


            int id =
                ReadInt("Employee Id: ");

            string name =
                ReadString("Employee Name: ");

            DateTime hireDate =
                ReadDate(
                    "Hire Date (yyyy-MM-dd): "
                );

            int departmentId =
                ReadInt("Department Id: ");

            decimal salary =
                ReadDecimal("Salary: ");


            Employee employee =
                new Employee(
                    id,
                    name,
                    hireDate,
                    departmentId,
                    salary
                );


            Result<Employee> result =
                company.AddEmployee(employee);


            PrintResult(result);
        }


        // =====================================
        // ONBOARDING
        // =====================================

        private static void ProcessOnboarding(
            Company company)
        {
            Result<Employee> result =
                company.ProcessNextOnboarding();


            PrintResult(result);
        }


        // =====================================
        // PROMOTION
        // =====================================

        private static void PromoteEmployee(
            Company company)
        {
            int id =
                ReadInt("Employee Id: ");


            Result<Manager> result =
                company.PromoteEmployeeToManager(id);


            PrintResult(result);
        }


        // =====================================
        // SKILL
        // =====================================

        private static void RegisterSkill(
            Company company)
        {
            int employeeId =
                ReadInt("Employee Id: ");

            string skill =
                ReadString("Skill: ");


            Result<string> result =
                company.RegisterSkillForEmployee(
                    employeeId,
                    skill
                );


            PrintResult(result);
        }


        // =====================================
        // SEARCH
        // =====================================

        private static void SearchEmployee(
            Company company)
        {
            Console.WriteLine(
                "1. Search by Id"
            );

            Console.WriteLine(
                "2. Search by Name"
            );


            int choice =
                ReadInt("Choose search type: ");


            Employee? employee = null;


            if (choice == 1)
            {
                int id =
                    ReadInt("Employee Id: ");

                employee =
                    company.FindEmployeeById(id);
            }

            else if (choice == 2)
            {
                string name =
                    ReadString("Employee Name: ");

                employee =
                    company.FindEmployeeByName(name);
            }

            else
            {
                Console.WriteLine(
                    "Invalid option."
                );

                return;
            }


            if (employee == null)
            {
                Console.WriteLine(
                    "Employee not found."
                );
            }

            else
            {
                PrintEmployee(employee);
            }
        }


        // =====================================
        // DEPARTMENT EMPLOYEES
        // =====================================

        private static void ShowDepartmentEmployees(
            Company company)
        {
            int departmentId =
                ReadInt("Department Id: ");


            Result<List<Employee>> result =
                company.GetEmployeesByDepartment(
                    departmentId
                );


            PrintResult(result);


            if (
                result.Success &&
                result.Data != null)
            {
                PrintEmployees(result.Data);
            }
        }


        // =====================================
        // DELEGATE + LAMBDA FILTERING
        // =====================================

        private static void FilterEmployees(
            Company company)
        {
            Console.WriteLine(
                "1. Managers only"
            );

            Console.WriteLine(
                "2. Salary above amount"
            );

            Console.WriteLine(
                "3. By Department"
            );


            int option =
                ReadInt("Choose filter: ");


            List<Employee> result;


            if (option == 1)
            {
                result =
                    company.FilterEmployees(
                        employee =>
                            employee is Manager
                    );
            }

            else if (option == 2)
            {
                decimal salary =
                    ReadDecimal(
                        "Minimum salary: "
                    );


                result =
                    company.FilterEmployees(
                        employee =>
                            employee.Salary >= salary
                    );
            }

            else if (option == 3)
            {
                int departmentId =
                    ReadInt(
                        "Department Id: "
                    );


                result =
                    company.FilterEmployees(
                        employee =>
                            employee.DepartmentId
                            == departmentId
                    );
            }

            else
            {
                Console.WriteLine(
                    "Invalid filter option."
                );

                return;
            }


            PrintEmployees(result);
        }


        // =====================================
        // AVERAGE SALARY
        // =====================================

        private static void ShowAverageSalary(
            Company company)
        {
            decimal average =
                company.CalculateAverageSalary();


            Console.WriteLine(
                $"Average Salary: {average:N2}"
            );
        }


        // =====================================
        // DEPARTMENT REPORT
        // =====================================

        private static void ShowDepartmentReport(
            Company company)
        {
            Dictionary<int, int> report =
                company.GetEmployeeCountPerDepartment();


            Console.WriteLine(
                "Employee Count Per Department"
            );


            foreach (
                KeyValuePair<int, int> item
                in report)
            {
                Department? department =
                    company.GetDepartmentById(
                        item.Key
                    );


                if (department != null)
                {
                    Console.WriteLine(
                        $"{department.Name}: {item.Value}"
                    );
                }
            }
        }


        // =====================================
        // ACTION HISTORY
        // =====================================

        private static void ShowActionHistory(
            Company company)
        {
            List<string> history =
                company.GetActionHistoryNewestFirst();


            if (history.Count == 0)
            {
                Console.WriteLine(
                    "No actions yet."
                );

                return;
            }


            foreach (string action in history)
            {
                Console.WriteLine(action);
            }
        }


        // =====================================
        // UNIQUE SKILLS
        // =====================================

        private static void ShowUniqueSkills(
            Company company)
        {
            List<string> skills =
                company.GetUniqueSkills();


            if (skills.Count == 0)
            {
                Console.WriteLine(
                    "No skills registered."
                );

                return;
            }


            foreach (string skill in skills)
            {
                Console.WriteLine(skill);
            }
        }


        // =====================================
        // ALL EMPLOYEES
        // =====================================

        private static void ShowAllEmployees(
            Company company)
        {
            List<Employee> employees =
                company.GetAllEmployees();


            PrintEmployees(employees);
        }


        // =====================================
        // PRINT HELPERS
        // =====================================

        private static void PrintEmployee(
            Employee employee)
        {
            string role =
                employee is Manager
                ? "Manager"
                : "Employee";


            Console.WriteLine(
                $"{role} | {employee}"
            );
        }


        private static void PrintEmployees(
            List<Employee> employees)
        {
            if (employees.Count == 0)
            {
                Console.WriteLine(
                    "No employees found."
                );

                return;
            }


            foreach (
                Employee employee
                in employees)
            {
                PrintEmployee(employee);
            }
        }


        private static void PrintResult<T>(
            Result<T> result)
        {
            if (result.Success)
            {
                Console.WriteLine(
                    $"SUCCESS: {result.Message}"
                );
            }

            else
            {
                Console.WriteLine(
                    $"ERROR: {result.Message}"
                );
            }
        }


        // =====================================
        // EVENTS
        // =====================================

        private static void OnEmployeeOnboarded(
            object? sender,
            EmployeeEventArgs e)
        {
            Console.WriteLine(
                $"[EVENT] Employee Onboarded: {e.Employee.Name}"
            );
        }


        private static void OnEmployeePromoted(
            object? sender,
            EmployeeEventArgs e)
        {
            Console.WriteLine(
                $"[EVENT] Employee Promoted: {e.Employee.Name}"
            );
        }



        // =====================================
        // MENU
        // =====================================

        private static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine(
                "=========================================="
            );

            Console.WriteLine(
                "      EMPLOYEE MANAGEMENT SYSTEM"
            );

            Console.WriteLine(
                "=========================================="
            );

            Console.WriteLine("1. Add Department");

            Console.WriteLine(
                "2. Add Employee To Onboarding Queue"
            );

            Console.WriteLine(
                "3. Process Next Onboarding"
            );

            Console.WriteLine(
                "4. Promote Employee To Manager"
            );

            Console.WriteLine(
                "5. Register Skill"
            );

            Console.WriteLine(
                "6. Search Employee"
            );

            Console.WriteLine(
                "7. Show Department Employees"
            );

            Console.WriteLine(
                "8. Filter Employees"
            );

            Console.WriteLine(
                "9. Average Salary"
            );

            Console.WriteLine(
                "10. Department Report"
            );

            Console.WriteLine(
                "11. Action History"
            );

            Console.WriteLine(
                "12. Unique Skills"
            );

            Console.WriteLine(
                "13. Show All Employees"
            );

            Console.WriteLine(
                "0. Exit"
            );

            Console.WriteLine(
                "=========================================="
            );
        }


        // =====================================
        // INPUT VALIDATION
        // =====================================

        private static int ReadInt(string message)
        {
            while(true)
            {
                Console.Write(message);

                string? input = Console.ReadLine();

                int value;

                if (int.TryParse(input, out value))
                {
                    return value;
                }

                Console.WriteLine("Invalid number. Try again.");
            }
        }

        private static decimal ReadDecimal(string message)
        {
            while (true)
            {
                Console.Write(message);

                string? input = Console.ReadLine();

                decimal value;

                if (decimal.TryParse(input, out value))
                {
                    return value;
                }
                Console.WriteLine("Invalid decimal number.");
            }
        }

        private static string ReadString(string message)
        {
            while (true)
            {
                Console.Write(message);

                string? value = Console.ReadLine();

                if (! string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }

                Console.WriteLine("Value cannot be empty.");
            } 
        }

        private static DateTime ReadDate(string message)
        {
            while (true)
            {
                Console.Write(message);

                string? input = Console.ReadLine();

                DateTime date;

                bool valid = DateTime.TryParseExact(input, "yyyy-MM-dd", 
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date); 

                if (valid)
                {
                    return date;
                }

                Console.WriteLine("Invalid date. Use  yyyy-MM-dd.");
            }
        }

        // =====================================
        // SEED DATA
        // =====================================

        private static void SeedData(Company company)
        {
            company.AddDepartment(new Department(1, "IT"));
            company.AddDepartment(new Department(2, "HR"));
            company.AddDepartment(new Department(3, "Finance"));

            company.AddEmployee(new Employee(101, "Ahmed", new DateTime(2024, 1, 10), 1, 25000));
            company.AddEmployee(new Employee(101, "Sara", new DateTime(2024, 5, 20), 2, 18000));
            company.AddEmployee(new Employee(101, "Omar", new DateTime(2025, 2, 1), 3, 22000));

            company.ProcessNextOnboarding();
            company.ProcessNextOnboarding();
            company.ProcessNextOnboarding();

            company.PromoteEmployeeToManager(101);

            company.RegisterSkillForEmployee(101, "Leadership");
            company.RegisterSkillForEmployee(102, "Communication");
            company.RegisterSkillForEmployee(103, "C#");
        }
    }
}
