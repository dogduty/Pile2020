using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pile.db
{
    public class MetaCustomer
    {
        [Required, MinLength(2)]
        public string LastName;
        [Required]
        public string Status;
        [Required]
        public Nullable<System.DateTime> RouteStartDate;

        //------before major database work

        //[Required, EmailAddress]
        //public string Email;
        //[EmailAddress]
        //public string Email2;
        //[MinLength(12), MaxLength(12)]
        //public string Home;
        //[MinLength(12), MaxLength(12)]
        //public string Mobile;
        //[Required]
        //[MinLength(2), MaxLength(2)]
        //public string State;
        //[MaxLength(2)]
        //public string MailState;
        //[Required]
        //public string Address;

        //----------end before major database work

        //public int CustomerId;
        //[Required]
        //public Nullable<int> Day;
        //public Nullable<int> Crew;
        //public string Flag;
        //public string Notes3;
        //public string LatePmt;
        //public string EmpPay;
        //public string FirstName;
        //public string LastName;
        //public string SpouseFirstName;
        //public string SpouseLastName;
        //public string MapsCo;
        //public string Home;
        //public string Mobile;
        //public Nullable<int> NumDogs;
        //public string Code;
        //public Nullable<int> FB;
        //public Nullable<int> Take;
        //public Nullable<int> Odor;
        //public Nullable<int> Xtra;
        //public Nullable<int> Zap;
        //public string Notes1;
        //public string Notes2;
        //public string City;
        //public string Zip;
        //public string PickUpDay;
        //public string Territory;
        //public string Route;
        //public Nullable<int> EstNum;
        //public string EstPickUpTime;
        //public string EstTravelTime;
        //public Nullable<decimal> WeeklyRate;
        //public Nullable<decimal> MonthlyRate;
        //[Required]
        //public Nullable<int> Count;
        //public Nullable<decimal> FullPay;
        //public Nullable<decimal> AdjPay;
        //public Nullable<decimal> FBRate;
        //public Nullable<decimal> TakeRate;
        //public Nullable<decimal> OdorRate;
        //[Required]
        //public string Type;
        //public string MailAddress;
        //public string MailCity;
        //public string MailZip;
        //public string MailState;
        //public string State;
        //public Nullable<decimal> XtraRate;
        //public Nullable<decimal> Total;
        //public string RC;
        //public string DUP;
        //public string Warning;
        //public string Runner;
        //public string Complaints;
        //[Required]
        //public string Source;
        //public string WorkPhone;
        //public string OtherPhone;
        //public string OtherContact;
        //public string Pmts;
        //public string Biscuits;
        //public string Invoice;
        //public string Services;
        //public string Discount;
        //public string Freq;
        //public string QualityChecked;
        //public string Combo;
        //public Nullable<System.DateTime> FinalServiceDate;
        //public Nullable<int> WhyQuit;
        //public string WhyQuitSpecify;
        //public Nullable<System.DateTime> StartDate;
        //public Nullable<int> HowFound;
        //public string HowFoundSpecify;
        //public Nullable<System.DateTime> PauseDate;
        //public Nullable<System.DateTime> ReStartDate;
        //public string GPS;
        //public string NewGPS;
        //public string GPSState;
        //public string GPSZip;
        //public Nullable<int> PaymentMethod;
        //public Nullable<int> InvoiceMethod;
        //public bool TurnOffServiceEmails;
        //public string QBId;
        //public string QbSyncToken;
        //public Nullable<decimal> QBBalance;
        //public Nullable<decimal> QBTotalBalance;
        //public string TempQBId;
        //public string TempQBScore;
        //public string QBStatus;
        //public string QBSeverity;
        //public string QBMessage;
        //public string QBInvoiceNumber;
        //public Nullable<decimal> QBSubTotal;
        //public Nullable<System.DateTime> QBLastInvoiceDate;
        //public Nullable<System.DateTime> DBStartDate;
        //public Nullable<System.DateTime> DBEndDate;
        //public string EmailStatus;
        //public Nullable<System.DateTime> LateLastSent;
        //public Nullable<System.DateTime> CallOfficeLastSent;
        //public Nullable<System.DateTime> FinalNoticeLastSent;
        //public bool CreditCardOnFile;
        //public bool CustomerSetup;
        //public bool MeetScheduled;
        //public bool MeetPerformed;
        //public Nullable<System.DateTime> MeetSchDate;
        //public string MeetSchTime;
        //public string FMNotes;
        //public string NewHomePhone;
        //public string NewMobilePhone;
        //public string NewWorkPhone;
        //public string NewOtherPhone;
        //public bool TurnOffServiceEmail2;
        //public Nullable<System.DateTime> BulkEmailDate;
        //public Nullable<int> ContractRenewalTerm;
        //public Nullable<int> ContractCancellationTerm;
        //public Nullable<System.DateTime> LastUpdate;
        //public Nullable<System.DateTime> LastQbUpdate;

    }
}