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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.ServiceHistories = new HashSet<ServiceHistory>();
        }
    
        public int Id { get; set; }
        public Nullable<int> CrewId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string TruckID { get; set; }
        public string CompanyPhone { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public decimal PayPerc { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool AcceptsTasks { get; set; }
    
        public virtual Crew Crew { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceHistory> ServiceHistories { get; set; }
    }
}