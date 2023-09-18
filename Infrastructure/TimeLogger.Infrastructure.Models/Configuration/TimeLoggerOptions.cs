using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Models.Configuration
{
    public class TimeLoggerOptions
    {
        public TimeLoggerOptions()
        {
        }
        public string ApiUrl { get; set; }
        public string IdentityServerUrl { get; set; }
        public string IdentityServerApiUrl { get; set; }
        public string WebUrl { get; set; }
        public List<string> WebUrls { get; set; }
        public string ApiName { get; set; }
    }
}
