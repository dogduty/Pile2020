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
    
    public partial class Pause
    {
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public System.DateTime PauseDate { get; set; }
        public System.DateTime RestartDate { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
