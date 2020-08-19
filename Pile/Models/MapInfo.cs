using Pile.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.Models
{
    public class MapInfo
    {
        public MapInfo() { }

        //public MapInfo(Customer customer, ServiceDay serviceDay, List<Service> ServicesList)
        //{
        //    CustomerName = $"{customer.FirstName} {customer.LastName ?? ""}";
        //    Address = customer.Addresses.Single(x => x.AddressType == "Site").Address1;
        //    Crew = serviceDay.Crew;
        //    Day = serviceDay.Day;
        //    Stop = serviceDay.EstNum;

        //    var serviceIds = serviceDay.ServiceDetails.Select(x => x.ServiceId);
        //    Services = string.Join(", ", ServicesList.Where(x => serviceIds.Contains(x.ServiceId)).Select(x => x.Description));
        //}

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
        public int CrewId { get; set; }
        public int Day { get; set; }
        public decimal Stop { get; set; }
        public string Services { get; set; }
        public IEnumerable<int> ServiceIds { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
    }
}