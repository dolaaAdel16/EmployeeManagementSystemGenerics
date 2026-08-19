using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagementSystemGenerics.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Department (int id, string name)
        {
            Id = id;
            Name = name;    
        }

        public override string ToString()
        {
            return $"Id : {Id} | Department: {Name}";
        }
    }
}
