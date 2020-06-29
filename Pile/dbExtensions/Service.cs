using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.db
{
    public partial class Service
    {

        //TODO:Invoice stuff
        public EmployeeAmount GetEmployeeAmount(Employee employee, decimal custTotal, int qty)
        {
            decimal pay = 0;
            decimal pct = (decimal)employee.PayPerc;
            if (ExcludeEmpPay)
                return new EmployeeAmount(0, 0);

            //In the original order.  The latter must trump the former....
            pay = custTotal * (decimal)employee.PayPerc / 100;

            if (QtyFlatPayAmt > 0 && qty > 0) {
                pay = Invoice.GetWeeklyAmount(Freq, QtyFlatPayAmt, qty);
                pct = 0;
            }

            if (FlatEmpPayAmt > 0)
            {
                pay = Invoice.GetWeeklyAmount(Freq, FlatEmpPayAmt);
                pct = 0;
            }

            return new EmployeeAmount(Math.Round(pay, 2), pct);

        }

        //public static decimal GetWeeklyAmount(int freq, decimal price, decimal qty = 1)
        //{
        //    // Frequency Values
        //    // Weekly = 1
        //    // Every Other Week = 2
        //    // Monthly = 3
        //    // One Time = 4

        //    switch (freq)
        //    {
        //        case 1:
        //        case 2:
        //            return Math.Round(qty * price * 12  * freq / 52, 2);
        //    }

        //    // otherwise (would be Freq 3 or 4)
        //    return Math.Round(qty * price, 2);
        //}

        public string GetFreqString()
        {
            return Enum.GetName(typeof(Frequency), Freq).Replace('_', ' ');
        }
    }

    public class EmployeeAmount
    {
        public EmployeeAmount(decimal amount, decimal percent)
        {
            Amount = amount;
            Percent = percent;
        }

        public decimal Amount { get; set; }
        public decimal Percent { get; set; }
    }
}