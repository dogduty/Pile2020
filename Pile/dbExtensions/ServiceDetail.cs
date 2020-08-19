using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.db

{
    public partial class ServiceDetail
    {

        //TODO:Invoice Stuff
        public decimal GetStopAmout()
        {
            decimal discounts = 0;
            decimal total = 0;

            using (var db = new pileEntities())
            {
                var service = db.Services.Single(x => x.ServiceId == ServiceId);

                discounts = Discount;
                total += Invoice.GetWeeklyAmount(service.Freq, Price);

                if (QtyPrice > 0)
                    total += Invoice.GetWeeklyAmount(service.Freq, QtyPrice, Qty - 1);

                if (Discount != 0)
                    total -= Invoice.GetWeeklyAmount(service.Freq, Discount);

                if (AdditAmount != 0)
                    total += Invoice.GetWeeklyAmount(service.Freq, AdditAmount);
            }

            return total;
        }

        public static void UpdateEstNums(int customerId)
        {
            using (var db = new pileEntities())
            {
                var crewDayList = db.ServiceDays.Where(x => x.CustomerId == customerId).Select(x => new { Crew = x.Crew, Day = x.Day }).ToList();

                foreach (var crewDay in crewDayList.Distinct())
                {
                    var crew = crewDay.Crew;
                    var day = crewDay.Day;
                    var orderedCustId = (from sd in db.ServiceDays
                                         join c in db.Customers on sd.CustomerId equals c.CustomerId
                                         where c.Status == "A" && sd.Day == day && sd.Crew == crew
                                         select new
                                         {
                                             EstNum = sd.EstNum,
                                             CustomerId = sd.CustomerId
                                         }).Distinct().OrderBy(x => x.EstNum).Select(x => x.CustomerId).ToList();


                    var serviceDetailsForCrewDay = db.ServiceDays.Where(x => orderedCustId.Contains(x.CustomerId) && x.Day == day && x.Crew == crew);

                    for (int i=0; i < orderedCustId.Count(); i++)
                    {
                        var custId = orderedCustId.ElementAt(i);
                        var serviceDaysToUpdate = serviceDetailsForCrewDay.Where(x => x.CustomerId == custId).ToList();
                        foreach (var detailToUpdate in serviceDaysToUpdate)
                            detailToUpdate.EstNum = i + 1;
                    }
                }

                db.SaveChanges();
            }
        }
    }
}