using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pile
{
    public class Flag
    {
        public static List<Flag> GetFlags()
        {
            return new List<Flag>
            {
                new Flag { Id = "", Meaning = "" },
                new Flag { Id = "L", Meaning = "Late" },
                new Flag { Id = "C", Meaning = "Call Office" },
                new Flag { Id = "F", Meaning = "Final Notice" }
            };
        }

        [Key]
        public string Id { get; set; }
        public string Meaning { get; set; }
    }

}