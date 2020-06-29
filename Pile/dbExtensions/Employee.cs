using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.db
{
    public partial class Employee
    {
        public virtual Models.ApplicationUser User { get; set; }
    }
}