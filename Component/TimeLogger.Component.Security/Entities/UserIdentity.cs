using TimeLogger.Data.Entity;
using TimeLogger.Infrastructure.Utility.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Component.Security.Entities
{
    public class UserIdentity : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
        public TwoFactorTypes TwoFactorTypeId { get; set; }
        public int? CompanyId { get; set; }
        public int StatusId { get; set; }


        [ForeignKey("TwoFactorTypeId")]
        public virtual TwoFactorType TwoFactorType { get; set; }

        public virtual ICollection<PreviousPassword> PreviousPasswords { get; set; }

        public virtual ICollection<UserIdentityRole> UserRoles { get; set; }

        public virtual ICollection<UserIdentityLogin> Logins { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }

        public virtual ICollection<Addresses> Addresses { get; set; }
    }
}
