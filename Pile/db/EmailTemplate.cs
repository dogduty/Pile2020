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
    
    public partial class EmailTemplate
    {
        public int Id { get; set; }
        public int EmailTypeId { get; set; }
        public string Title { get; set; }
        public int EmailAccountId { get; set; }
        public string BodyTemplate { get; set; }
    
        public virtual EmailAccount EmailAccount { get; set; }
        public virtual EmailType EmailType { get; set; }
    }
}
