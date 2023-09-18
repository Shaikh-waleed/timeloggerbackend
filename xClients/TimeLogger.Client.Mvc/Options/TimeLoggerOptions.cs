using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TimeLogger.Client.Mvc.Options
{
    public static class TimeLoggerOptions
    {
        public static string ApiUrl { get; set; }

        static TimeLoggerOptions()
        {
            ApiUrl = ConfigurationManager.AppSettings["ApiUrl"];
        }
    }
}