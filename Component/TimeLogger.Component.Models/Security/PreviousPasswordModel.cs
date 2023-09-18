using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Component.Models.Security
{
    public class PreviousPasswordModel
    {
        public string PasswordHash { get; set; }
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime CreateDateUtc { get; set; }
    }
}
