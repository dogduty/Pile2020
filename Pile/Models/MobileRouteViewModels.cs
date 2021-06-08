using Pile.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.Models
{
    public class MobileRouteStopViewModel
    {
        public DateTime Date { get; set; }
        public Decimal EstNum { get; set; }
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int Dogs { get; set; }
        public string Code { get; set; }
        public List<Note> Notes { get; set; }
        public List<Phone> Phones { get; set; }
        public string Combo { get; set; }
        public DateTime RouteStartDate { get; set; }
        public bool Completed { get; set; }
        public List<MobileRouteStopDetailViewModel> Details { get; set; }
        public List<YardConditionsViewModel> Conditions { get; set; }
        public bool Expand { get; internal set; }
    }

    public class MobileRouteStopDetailViewModel
    {
        public int ServiceDetailId { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }

    public class YardConditionsViewModel
    {
        public bool Selected { get; set; }
        public bool ShowCheck { get; set; }
        public string Problem { get; set; }
    }
}