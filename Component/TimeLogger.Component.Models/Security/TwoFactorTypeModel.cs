using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class TwoFactorTypeModel
    {
        public TwoFactorTypes Id { get; set; }
        public string Name { get; set; }
    }
}
