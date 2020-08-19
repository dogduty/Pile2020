using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pile.db
{
    public class MetaCustomer
    {
        [Required, MinLength(2)]
        public string LastName;
        [Required]
        public string Status;
        [Required]
        public Nullable<System.DateTime> RouteStartDate;
        [Required]
        public string HowFoundId;
        [Required]
        public Nullable<int> PaymentMethodId;
        [Required]
        public Nullable<int> InvoiceMethodId;


    }
}