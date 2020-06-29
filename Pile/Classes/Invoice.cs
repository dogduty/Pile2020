using Pile.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile
{
    public class Invoice
    {

        public int EligibleStops { get; private set; }
        public int ActualStops { get; private set; }
        public decimal InvoiceAmount { get; private set; }
        public decimal CreditAmount { get; set; }
        public decimal StopAmount { get; set; }
        public decimal MonthlyAmount { get; set; }

        //public void SetStops2(List<vInvoiceFeed> serviceInstances, DateTime startDate, DateTime endDate)
        //{
        //    ///the view vInvoiceFeed returns stop information for every week of the month that is eligible for the customer
        //    ///without regard for stop frequency.  A monthly customer will still have 4 or 5 recrods, etc.
        //    ///However, it does respect StartDate and EndDate as well as Pause & Restart Dates.
            
        //    EligibleStops = serviceInstances.Count();
        //    ActualStops = serviceInstances.Count(x => !x.Skip.Value);
        //    switch (serviceInstances[0].Freq)
        //    {
        //        case 2:  //every other week.
        //            if (ActualStops > 2)
        //                EligibleStops = 2;
        //                ActualStops = 2;
        //            break;
        //        case 3:
        //            if (ActualStops > 1)
        //                EligibleStops = 1;
        //                ActualStops = 1;
        //            break;
        //    }

        //    if (ActualStops > serviceInstances[0].CountDown)
        //        ActualStops = serviceInstances[0].CountDown;

        //}

        //public void SetInvoiceAmount2(vInvoiceFeed invoiceDatum)
        //{
        //    StopAmount = invoiceDatum.GetStopAmout();
        //    MonthlyAmount = invoiceDatum.GetMonthlyAmount(); //Invoice.GetWeeklyAmount(serviceDetail.Freq, serviceDetail.Price, serviceDetail.Qty) * 4;
        //    InvoiceAmount = MonthlyAmount;

        //    if (ActualStops > 0 && ActualStops >= EligibleStops)
        //    {
        //        return;  //No need for goofy calcs.
        //    }

        //    // original, strange logic.  Original comment as well:
        //    // relief valve to keep one time scoops from becoming $3 services.
        //    // using 5 because 4 actually works in companys favor
        //    int missedStops = EligibleStops = ActualStops;
        //    if (missedStops > 1 && EligibleStops != 4)
        //    {
        //        InvoiceAmount = Math.Round(ActualStops * StopAmount, 2);
        //        CreditAmount = Math.Round(MonthlyAmount - InvoiceAmount, 2);
        //    }
        //    else
        //    {
        //        CreditAmount = Math.Round(missedStops * StopAmount, 2);
        //        InvoiceAmount = Math.Round(MonthlyAmount - CreditAmount, 2);
        //    }
        //}

        public static decimal GetWeeklyAmount(int freq, decimal price, decimal qty = 1)
        {
            // Frequency Values
            // Weekly = 1
            // Every Other Week = 2
            // Monthly = 3
            // One Time = 4

            switch (freq)
            {
                case 1:
                case 2:
                    return Math.Round(qty * price * 12 * freq / 52, 2);
            }

            // otherwise (would be Freq 3 or 4)
            return Math.Round(qty * price, 2);
        }




        //public void SetStops(ddServiceDetail serviceDetail, DateTime beginningRouteWeek, DateTime startDate, DateTime endDate)
        //{
        //    //Start in Next Weeks UnCreated Routes (comment from original garbage code)
        //    var nextRouteDate = beginningRouteWeek.AddDays(serviceDetail.Day + 7);
        //    var lastEligibleDate = serviceDetail.LastRouteDate;

        //    //TODO:  Remove next line, reenable one below
        //    DateTime? lastRouteDate = new DateTime(2020, 2, 29);
        //    //var lastRouteDate = serviceDetail.LastRouteDate;

        //    while (nextRouteDate < endDate)
        //    {
        //        if ((serviceDetail.ddCustomer.FinalServiceDate.HasValue && serviceDetail.ddCustomer.FinalServiceDate.Value < startDate) ||
        //            serviceDetail.ddCustomer.RouteStartDate >= endDate || !serviceDetail.LastRouteDate.HasValue)
        //            break;

        //        //calculate eligible stops
        //        var lastEligibleDays = (int)(nextRouteDate - lastEligibleDate.Value).TotalDays;

        //        if ((serviceDetail.ddService.Freq == 1 && lastEligibleDays >= 7) ||
        //            (serviceDetail.ddService.Freq == 2 && lastEligibleDays >= 14) ||
        //            (serviceDetail.ddService.Freq == 3 && lastEligibleDays >= 28))
        //        {
        //            lastEligibleDate = nextRouteDate;
        //            if (nextRouteDate >= startDate)
        //            {
        //                EligibleStops++;
        //            }
        //        }

        //        if (ddRoute.RouteQualifies(nextRouteDate, serviceDetail.ddService, serviceDetail, serviceDetail.ddCustomer, nextRouteDate, lastRouteDate.Value))
        //        {
        //            if (serviceDetail.CountDown < 999)
        //                serviceDetail.CountDown--;

        //            lastRouteDate = nextRouteDate;
        //            if (nextRouteDate >= startDate)
        //                ActualStops++;
        //        }
        //        nextRouteDate = nextRouteDate.AddDays(7);
        //    } //while (nextRouteDate < endDate)
        //} 

        //public void SetInvoiceAmount(ddServiceDetail serviceDetail)
        //{
        //    StopAmount = serviceDetail.GetStopAmout();
        //    MonthlyAmount = serviceDetail.ddService.GetWeeklyAmount(serviceDetail.Price, serviceDetail.Qty);
        //    InvoiceAmount = MonthlyAmount;

        //    if (ActualStops > 0 && ActualStops >= EligibleStops)
        //    {
        //        return;  //No need for goofy calcs.
        //    }

        //    // original, strange logic.  Original comment as well:
        //    // relief valve to keep one time scoops from becoming $3 services.
        //    // using 5 because 4 actually works in companys favor
        //    int missedStops = EligibleStops = ActualStops;
        //    if (missedStops > 1 && EligibleStops != 4)
        //    {
        //        InvoiceAmount = Math.Round(ActualStops * StopAmount, 2);
        //        CreditAmount = Math.Round(MonthlyAmount - InvoiceAmount, 2);
        //    }
        //    else
        //    {
        //        CreditAmount = Math.Round(missedStops * StopAmount, 2);
        //        InvoiceAmount = Math.Round(MonthlyAmount - CreditAmount, 2);
        //    }
        //}

    }
}