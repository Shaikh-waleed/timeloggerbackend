using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TimeLogger.Data.Entity
{
    public class NotificationTemplate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public NotificationTemplates Id { get; set; }

        public NotificationTypes NotificationTypeId { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }

        [ForeignKey("NotificationTypeId")]
        public virtual NotificationType NotificationTypes { get; set; }
    }
}
