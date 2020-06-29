using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.db
{
    public partial class EmailAccount
    {
        public string EmailAddress
        {
            get
            {
                if (this.EmailDomain == null)
                {
                    using (var db = new pileEntities())
                    {
                        this.EmailDomain = db.EmailDomains.Single(x => x.Id == this.Domain);
                    }
                }
                return string.Format("{0}@{1}", this.LocalPart, this.EmailDomain.Domain);
            }
        }
    }
}