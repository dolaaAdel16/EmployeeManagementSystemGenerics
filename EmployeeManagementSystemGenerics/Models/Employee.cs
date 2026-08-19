using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystemGenerics.Models
{
    internal class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public decimal Salary { get; set; } 
        public Employee (int id, string name, DateTime hireDate, int departmentId, decimal salary) 
        {
            Id = id;
            Name = name;
            HireDate = hireDate;
            DepartmentId = departmentId;
            Salary = salary;
        }

        public override string ToString()
        {
            return $"Id: {Id} | Name: {Name} | Department: {DepartmentId} | Salary: {Salary:N2}";
        }
    }
}
