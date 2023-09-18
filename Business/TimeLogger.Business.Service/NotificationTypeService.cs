using AutoMapper;
using TimeLogger.Business.IService;
using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using TimeLogger.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Business.Service
{
    public class NotificationTypeService : BaseService<NotificationTypeModel, NotificationType, int>, INotificationTypeService
    {
        private readonly INotificationTypeRepository notificationTypeRepository;

        public NotificationTypeService(IMapper mapper, INotificationTypeRepository notificationTypeRepository, IUnitOfWork unitOfWork) : base(mapper, notificationTypeRepository, unitOfWork)
        {
            this.notificationTypeRepository = notificationTypeRepository;
        }
    }
}
