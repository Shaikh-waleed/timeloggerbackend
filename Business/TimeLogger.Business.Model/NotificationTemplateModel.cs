using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Business.Model
{
    public class NotificationTemplateModel
    {
        public NotificationTemplates Id { get; set; }
        public NotificationTypes NotificationTypeId { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
    }
}
