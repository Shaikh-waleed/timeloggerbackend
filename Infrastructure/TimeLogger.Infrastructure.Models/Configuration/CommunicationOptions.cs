using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Models.Configuration
{
    public class CommunicationOptions
    {
        public CommunicationOptions()
        {
        }

        public string EmailService { get; set; }
        public string SmsService { get; set; }
    }
}
