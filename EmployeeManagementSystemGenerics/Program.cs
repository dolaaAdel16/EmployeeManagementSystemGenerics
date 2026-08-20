using EmployeeManagementSystemGenerics.Events;
using EmployeeManagementSystemGenerics.Models;
using EmployeeManagementSystemGenerics.Services;
using System.Runtime.CompilerServices;

namespace EmployeeManagementSystemGenerics
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Company company = new Company();


            company.EmployeeOnboarded += OnEmployeeOnboarded;
            company.EmployeePromoted += OnEmployeePromoted;

        }

        // =====================================
        // EVENTS
        // =====================================
       
        private static void OnEmployeeOnboarded(object? sender, EmployeeEventArgs e)
        {
            Console.WriteLine($"[Event] Employee Onboarded : {e.Employee.Name}");
        }


        private static void OnEmployeePromoted(object? sender, EmployeeEventArgs e)
        {
            Console.WriteLine($"[Event] Employee Promoted : {e.Employee.Name}");
        }

        // =====================================
        // SEED DATA
        // =====================================

        private static void Seeddata(Company company)
        {
            company.AddDepartment(new Department(1, "IT"));
            company.AddDepartment(new Department(2, "HR"));
            company.AddDepartment(new Department(1, "Finance"));

            company.AddEmployee(new Employee(101, "Ahmed", new DateTime(2024, 1, 10), 1, 25000));
            company.AddEmployee(new Employee(101, "Sara", new DateTime(2024, 5, 20), 2, 18000));
            company.AddEmployee(new Employee(101, "Omar", new DateTime(2025, 2, 1), 1, 22000));

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
