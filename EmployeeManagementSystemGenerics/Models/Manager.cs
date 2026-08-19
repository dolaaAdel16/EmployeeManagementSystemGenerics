using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystemGenerics.Models
{
    public class Manager : Employee
    {
        public List<Employee> TeamMembers { get; set; }

        public Manager (int id,
                        string name,
                        DateTime hireDate,
                        int departmentId,
                        decimal salary) : base(id, name, hireDate, departmentId, salary)
        {
            TeamMembers = new List<Employee>();
        }
    }                   
}
