using AutoMapper;
using TimeLogger.Business.IService;
using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using TimeLogger.Data.IRepository;
using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Business.Service
{
    public class NotificationTemplateService : BaseService<NotificationTemplateModel, NotificationTemplate, int>, INotificationTemplateService
    {
        private readonly INotificationTemplateRepository _notificationTemplateRepository;

        public NotificationTemplateService(IMapper mapper, INotificationTemplateRepository notificationTemplateRepository, IUnitOfWork unitOfWork) : base(mapper, notificationTemplateRepository, unitOfWork)
        {
            _notificationTemplateRepository = notificationTemplateRepository;
        }

        public async Task<NotificationTemplateModel> GetNotificationTemplate(NotificationTemplates notificationTemplates, NotificationTypes notificationTypes)
        {
            var template = await _notificationTemplateRepository.FirstOrDefaultAsync(x => x.Id == notificationTemplates && x.NotificationTypeId == notificationTypes);
            return mapper.Map<NotificationTemplate, NotificationTemplateModel>(template); ;
        }
    }
}
