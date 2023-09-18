using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Models.Configuration
{
    public class GoogleOptions
    {
        public GoogleOptions()
        {
        }

        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
