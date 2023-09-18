using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class UserLoginInfoModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}
