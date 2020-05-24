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
    
    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.Addresses = new HashSet<Address>();
            this.Notes = new HashSet<Note>();
            this.Pauses = new HashSet<Pause>();
            this.Phones = new HashSet<Phone>();
        }
    
        public int CustomerId { get; set; }
        public string Flag { get; set; }
        public string LatePmt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseLastName { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Combo { get; set; }
        public Nullable<System.DateTime> FinalServiceDate { get; set; }
        public Nullable<int> WhyQuit { get; set; }
        public string WhyQuitSpecify { get; set; }
        public Nullable<System.DateTime> RouteStartDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> HowFound { get; set; }
        public string HowFoundSpecify { get; set; }
        public string GPS { get; set; }
        public Nullable<int> PaymentMethod { get; set; }
        public Nullable<int> InvoiceMethod { get; set; }
        public Nullable<bool> TurnOffServiceEmails { get; set; }
        public string QBId { get; set; }
        public string QbSyncToken { get; set; }
        public Nullable<decimal> QBBalance { get; set; }
        public Nullable<decimal> QBTotalBalance { get; set; }
        public string QBInvoiceNumber { get; set; }
        public Nullable<decimal> QBSubTotal { get; set; }
        public Nullable<System.DateTime> QBLastInvoiceDate { get; set; }
        public Nullable<System.DateTime> DBStartDate { get; set; }
        public Nullable<System.DateTime> DBEndDate { get; set; }
        public string EmailStatus { get; set; }
        public Nullable<System.DateTime> LateLastSent { get; set; }
        public Nullable<System.DateTime> CallOfficeLastSent { get; set; }
        public Nullable<System.DateTime> FinalNoticeLastSent { get; set; }
        public Nullable<bool> CreditCardOnFile { get; set; }
        public Nullable<bool> CustomerSetup { get; set; }
        public Nullable<bool> MeetScheduled { get; set; }
        public Nullable<bool> MeetPerformed { get; set; }
        public Nullable<System.DateTime> MeetSchDate { get; set; }
        public string MeetSchTime { get; set; }
        public string FMNotes { get; set; }
        public string Email2 { get; set; }
        public Nullable<bool> TurnOffServiceEmail2 { get; set; }
        public Nullable<System.DateTime> BulkEmailDate { get; set; }
        public Nullable<int> ContractRenewalTerm { get; set; }
        public Nullable<int> ContractCancellationTerm { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<System.DateTime> LastQbUpdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Address> Addresses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Notes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pause> Pauses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phone> Phones { get; set; }
    }
}