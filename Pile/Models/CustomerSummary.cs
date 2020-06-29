using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pile.db;

namespace Pile.Models
{
    public class CustomerSummary
    {
        public Customer Customer { get; set; }
        public ServiceSummary Services { get; set; }

    }
}