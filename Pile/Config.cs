using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Pile
{
    public static class Config
    {
        public static string getSetting(string name, string defaultValue = null)
        {
            return ConfigurationManager.AppSettings[name] ?? defaultValue;
        }
    }
}