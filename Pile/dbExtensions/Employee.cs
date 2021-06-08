using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pile.db
{
    [MetadataType(typeof(MetaEmployee))]
    public partial class Employee
    {
        public virtual Models.ApplicationUser User { get; set; }

        public static Employee Create()
        {
            return new Employee { User = new Models.ApplicationUser() };
        }

        public decimal GetWeeklyAmount(decimal stopAmount, ServiceDetail sd)
        {
            decimal empAmount = 0;
            if (sd.Service.ExcludeEmpPay)
                return 0;
            if (PayPerc > 0)
                empAmount = stopAmount * PayPerc / 100;

            if (sd.Service.QtyFlatPayAmt > 0 && sd.Qty > 0)
            {
                var qtyAmount = Invoice.GetWeeklyAmount(sd.Service.Freq, sd.Service.QtyFlatPayAmt, sd.Qty);
                empAmount = qtyAmount == 0 ? empAmount : qtyAmount;
            }

            if (sd.Service.FlatEmpPayAmt > 0)
            {
                var flatAmount = Invoice.GetWeeklyAmount(sd.Service.Freq, sd.Service.FlatEmpPayAmt);
                empAmount = flatAmount == 0 ? empAmount : flatAmount;
            }

            return empAmount;
        }



    }

}