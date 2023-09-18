using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TimeLogger.Component.Security.Entities
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }

        public virtual ICollection<UserIdentity> Users { get; set; }
        public virtual ICollection<Addresses> Addresses { get; set; }
    }
}
