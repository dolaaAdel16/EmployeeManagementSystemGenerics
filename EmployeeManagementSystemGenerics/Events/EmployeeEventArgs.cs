using EmployeeManagementSystemGenerics.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystemGenerics.Events
{
    public class EmployeeEventArgs : EventArgs
    {
        public Employee Employee {  get; set; }

        public EmployeeEventArgs (Employee employee)
        {
            Employee = employee;
        }
    }
}
