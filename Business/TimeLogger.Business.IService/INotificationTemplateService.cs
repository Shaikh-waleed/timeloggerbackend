using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Business.IService
{
    public interface INotificationTemplateService : IBaseService<NotificationTemplateModel, NotificationTemplate, int>
    {
        Task<NotificationTemplateModel> GetNotificationTemplate(NotificationTemplates notificationTemplates, NotificationTypes notificationTypes);
    }
}
