using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Pile.db;

namespace Pile.Models
{
    public class CustomerCreate
    {
        [Key]
        public int Id { get; set; }
        public Customer Customer{ get; set; }
        public EmailAddress MainEmail { get; set; }
        public List<EmailAddress> AddnlEmails { get; set; }
        public Address SiteAddress { get; set; }
        public Address MailAddress { get; set; }
        public List<Phone> Phones { get; set; }
        public List<Note> Notes { get; set; }


    }
}