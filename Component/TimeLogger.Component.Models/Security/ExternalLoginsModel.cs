using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class ExternalLoginsModel
    {
        public IList<UserLoginInfoModel> CurrentLogins { get; set; }

        public IList<ExternalAuthenticationProvider> OtherLogins { get; set; }
    }
}
