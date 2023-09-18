using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Models.Configuration
{
    public class ComponentOptions
    {
        public ComponentOptions()
        {
        }
        public Security Security { get; set; }
        //public string Communication { get; set; }
        public CommunicationOptions Communication { get; set; }
    }
}
