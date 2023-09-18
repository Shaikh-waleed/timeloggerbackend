using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class IdentityUserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public TwoFactorTypes TwoFactorTypeId { get; set; }
        public int? CompanyId { get; set; }
        public int StatusId { get; set; }
    }
}
