//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pile.db
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailServer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EmailServer()
        {
            this.EmailDomains = new HashSet<EmailDomain>();
        }
    
        public int Id { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailDomain> EmailDomains { get; set; }
    }
}
