using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.Models
{
    public class CustomerStatus
    {
        public string DbStatus { get; set; }
        public string Status { get; set; }

        public static List<CustomerStatus> GetList()
        {
            return new List<CustomerStatus> {
                new CustomerStatus { DbStatus = "A", Status = "Active" },
                new CustomerStatus { DbStatus = "I", Status = "Inactive" }
            };
        }
    }

    public class CustomerType
    {
        public string DbType { get; set; }
        public string CustType { get; set; }

        public static List<CustomerType> GetList()
        {
            return new List<CustomerType> {
                new CustomerType { DbType = "R", CustType = "Residential" },
                new CustomerType { DbType = "C", CustType = "Commercial" }
            };
        }
    }
}