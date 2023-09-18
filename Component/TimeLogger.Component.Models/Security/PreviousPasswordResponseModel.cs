using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class PreviousPasswordResponseModel
    {
        public string PasswordHash { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime CreateDateUtc { get; set; }

        public IdentityUserModel User { get; set; }
    }
}
