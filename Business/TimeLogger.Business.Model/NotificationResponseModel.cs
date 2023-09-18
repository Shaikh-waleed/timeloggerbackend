using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Business.Model
{
    public class NotificationResponseModel
    {
        public int Id { get; set; }
        public string Recipient { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Attachment { get; set; }
        public int Attempts { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string Result { get; set; }

        public NotificationTypeModel NotificationType { get; set; }
        public StatusModel Status { get; set; }
    }
}
