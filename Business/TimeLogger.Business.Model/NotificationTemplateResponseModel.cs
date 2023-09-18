using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Business.Model
{
    public class NotificationTemplateResponseModel
    {
        public NotificationTemplates Id { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }

        public NotificationTypeModel NotificationType { get; set; }
    }
}
