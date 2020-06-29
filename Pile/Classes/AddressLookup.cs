using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile
{
    public class AddressLookup
    {
        public string ErrorMessage { get; set; }
        public string LocationMessage { get; set; }
        public GoogleGeocoding Geocoding { get; set; }
        public string LatLng { get; set; }
        public bool HasError { get { return string.IsNullOrWhiteSpace(ErrorMessage); } }
    }
}