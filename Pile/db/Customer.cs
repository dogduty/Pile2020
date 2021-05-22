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
            this.EmailAddresses = new HashSet<EmailAddress>();
            this.Notes = new HashSet<Note>();
            this.Pauses = new HashSet<Pause>();
            this.Phones = new HashSet<Phone>();
            this.CustomerHistories = new HashSet<CustomerHistory>();
            this.ServiceDetails = new HashSet<ServiceDetail>();
            this.ServiceHistories = new HashSet<ServiceHistory>();
            this.ToDoes = new HashSet<ToDo>();
            this.Routes = new HashSet<Route>();
        }
    
        public int Id { get; set; }
        public string Flag { get; set; }
        public string LatePmt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseLastName { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Combo { get; set; }
        public Nullable<System.DateTime> FinalServiceDate { get; set; }
        public Nullable<int> QuitReasonId { get; set; }
        public string WhyQuitSpecify { get; set; }
        public Nullable<System.DateTime> RouteStartDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<int> HowFoundId { get; set; }
        public string HowFoundSpecify { get; set; }
        public Nullable<int> PaymentMethodId { get; set; }
        public Nullable<int> InvoiceMethodId { get; set; }
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
        public bool CreditCardOnFile { get; set; }
        public bool CustomerSetup { get; set; }
        public bool MeetScheduled { get; set; }
        public bool MeetPerformed { get; set; }
        public Nullable<System.DateTime> MeetSchDate { get; set; }
        public string MeetSchTime { get; set; }
        public string FMNotes { get; set; }
        public Nullable<System.DateTime> BulkEmailDate { get; set; }
        public Nullable<int> ContractRenewalTerm { get; set; }
        public Nullable<int> ContractCancellationTerm { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<System.DateTime> LastQbUpdate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Address> Addresses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmailAddress> EmailAddresses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Notes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pause> Pauses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phone> Phones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerHistory> CustomerHistories { get; set; }
        public virtual HowFound HowFound { get; set; }
        public virtual InvoiceMethod InvoiceMethod { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual QuitReason QuitReason { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceHistory> ServiceHistories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ToDo> ToDoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Route> Routes { get; set; }
    }
}
