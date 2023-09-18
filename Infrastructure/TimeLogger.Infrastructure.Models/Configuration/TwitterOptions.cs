using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Models.Configuration
{
    public class TwitterOptions
    {
        public TwitterOptions()
        {
        }

        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
    }
}
