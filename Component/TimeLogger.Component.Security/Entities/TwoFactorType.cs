using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Component.Security.Entities
{
    public class TwoFactorType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public TwoFactorTypes Id { get; set; }
        public string Name { get; set; }
    }
}
