using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Models.Configuration
{
    public class FacebookOptions
    {
        public FacebookOptions()
        {
        }

        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
