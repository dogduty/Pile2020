using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.Models
{
    public class ServiceSummary
    {
        public ServiceSummary()
        {
            this.DailyServices = new List<ServiceDaily>();
        }

        public List<ServiceDaily> DailyServices { get; set; }

        public decimal GrandWeekly
        {
            get
            {
                return this.DailyServices.Select(x => x.WeeklyTotal).Sum();
            }
        }

        public decimal GrandMonthly
        {
            get
            {
                return this.DailyServices.Select(x => x.Total).Sum();
            }
        }

        public decimal GrandEmpPay
        {
            get
            {
                return this.DailyServices.Select(x => x.EmpAmount).Sum();
            }
        }
    }
}