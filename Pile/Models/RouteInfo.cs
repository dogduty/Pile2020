using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.Models
{
    public class RouteInfo
    {
        public int Id { get; set; }
        public bool Selected { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public int CustomerId { get; set; }
        public string Flag { get; set; }
        public string LatePayment { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string GateCode { get; set; }
        public string Code { get; set; }
        public string Notes { get; set; }
        public int Day { get; set; }
        public int CrewId { get; set; }
        public string Services { get; set; }
        public bool Visible { get; set; }
    }
}