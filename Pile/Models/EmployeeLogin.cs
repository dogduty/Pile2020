using Pile.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.Models
{
    public class EmployeeLogin
    {
        public static EmployeeLogin Create()
        {
            var emp = new EmployeeLogin
            {
                Employee = new Employee(),
                ApplicationUser = new ApplicationUser()
            };

            return emp;
        }

        public bool Active { get; set; }
        public Employee Employee { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}