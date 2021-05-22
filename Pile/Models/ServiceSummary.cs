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

        public static ServiceSummary GetServiceSummary(db.pileEntities db, int id)
        {

            var svcs = (from d in db.ServiceDetails
                        join s in db.Services on d.ServiceId equals s.Id
                        //join e in db.Employees on d.ServiceDay.Crew equals e.Crew
                        where d.CustomerId == id //&& e.Status != "I"
                        orderby d.ServiceDay.Day, d.ServiceDay.Crew, s.DisplayOrder
                        select new
                        {
                            ServiceName = s.Description,
                            Day = d.ServiceDay.Day,
                            Crew = d.ServiceDay.Crew,
                            CrewName = d.ServiceDay.Crew.Id + " - " + d.ServiceDay.Crew.Employees.FirstOrDefault().FirstName, // e.Crew.Value.ToString().Trim() + " - " + e.FirstName,
                            Price = d.Price,
                            Discount = d.Discount,
                            QtyPrice = d.QtyPrice,
                            Qty = d.Qty,
                            AddnlAmount = d.AdditAmount,
                            PayPercent = d.ServiceDay.Crew.Employees.FirstOrDefault().PayPerc, // e.PayPerc,
                            NumDogs = d.ServiceDay.NumDogs,
                            InvoiceAmt = d.InvoiceAmount,
                            EmpPayAdj = d.EmpPayAdj,
                            LastService = d.LastServiceDate,
                            ExcludeEmpPay = s.ExcludeEmpPay,
                            QtyFlatPayAmount = s.QtyFlatPayAmt,
                            FlatEmpPayAmount = s.FlatEmpPayAmt
                        }).ToList();

            int lastDay = -1;
            int lastCrew = -1;

            var serviceSummary = new ServiceSummary();
            ServiceDaily service = null;

            foreach (var detail in svcs)
            {
                if (lastDay != detail.Day || lastCrew != detail.Crew.Id)
                {
                    if (service != null)
                        serviceSummary.DailyServices.Add(service);
                    service = new ServiceDaily();
                }
                lastDay = detail.Day;
                lastCrew = detail.Crew.Id;
                service.Crew = detail.Crew.Id;
                service.CrewName = detail.CrewName;
                service.Day = detail.Day;
                service.LastServiceDate = detail.LastService.GetValueOrDefault();
                service.NumDogs = detail.NumDogs;
                service.ServiceList.Add(detail.ServiceName);

                var qty = detail.Qty - 1;
                var itemTotal = detail.InvoiceAmt != 0
                    ? detail.InvoiceAmt
                    : detail.Price + (detail.QtyPrice * qty) + detail.AddnlAmount - detail.Discount;

                service.Total += itemTotal;

                service.EmpAmount += service.CalcEmpTotal(detail.ExcludeEmpPay, detail.QtyFlatPayAmount, detail.Qty, detail.FlatEmpPayAmount, detail.PayPercent, itemTotal);
            }

            if (svcs.Count() > 0)
                serviceSummary.DailyServices.Add(service);

            return serviceSummary;
        }


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