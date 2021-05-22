using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pile.db
{
    public class MetaEmployee
    {
        [Required, MinLength(2)]
        public string FirstName;
        [Required, MinLength(2)]
        public string LastName;

    }
}

