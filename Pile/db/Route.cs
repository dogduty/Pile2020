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
    
    public partial class Route
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public int CustomerID { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> WeeklyRate { get; set; }
        public decimal EmpPerc { get; set; }
        public Nullable<decimal> EmpAmount { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<System.DateTime> LastServiceDate { get; set; }
        public Nullable<System.DateTime> StartServiceDate { get; set; }
        public Nullable<int> ServiceDetailId { get; set; }
        public Nullable<decimal> EstNum { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
