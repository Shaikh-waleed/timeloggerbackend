using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Data.Entity
{
    public class NotificationType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public NotificationTypes Id { get; set; }
        public string Name { get; set; }
    }
}
