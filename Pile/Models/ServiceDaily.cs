using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.Models
{
    public class ServiceDaily
    {
        public ServiceDaily()
        {
            this.ServiceList = new List<string>();
        }

        public int Day { get; set; }
        public int Crew { get; set; }
        public string CrewName { get; set; }
        public List<string> ServiceList { get; set; }
        public decimal Total { get; set; }
        public decimal EmpAmount { get; set; }
        public int NumDogs { get; set; }
        public DateTime LastServiceDate { get; set; }

        public decimal WeeklyTotal { get
            {
                return Math.Round(Total * 12 / 52, 2);
            }
        }

        public decimal CalcEmpTotal(bool? exclude, decimal? qtyFlatPayment, decimal? qty, decimal? flatEmpPayAmount, decimal payPercent, decimal? baseTotal)
        {
            if (exclude.GetValueOrDefault())
                return 0;

            if (qtyFlatPayment.GetValueOrDefault() != 0)
                return qtyFlatPayment.GetValueOrDefault() * qty.GetValueOrDefault();

            if (flatEmpPayAmount.GetValueOrDefault() != 0)
                return flatEmpPayAmount.GetValueOrDefault();

            return Math.Round(payPercent / 100 * baseTotal.GetValueOrDefault(), 2);
        }

    }
}