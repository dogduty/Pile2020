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
    
    public partial class RouteWeek
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> RouteMonth { get; set; }
        public Nullable<byte> RouteWeek1 { get; set; }
        public Nullable<System.DateTime> WeekStart { get; set; }
        public Nullable<System.DateTime> WeekEnd { get; set; }
    }
}