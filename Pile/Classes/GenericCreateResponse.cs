using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile
{
    public class GenericCreateResponse
    {
        public GenericCreateResponse(string message = "See Location Header for location of new object")
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}