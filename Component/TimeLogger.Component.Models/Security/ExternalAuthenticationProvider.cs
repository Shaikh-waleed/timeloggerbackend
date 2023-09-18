using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class ExternalAuthenticationProvider
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string HandlerType { get; set; }
    }
}
