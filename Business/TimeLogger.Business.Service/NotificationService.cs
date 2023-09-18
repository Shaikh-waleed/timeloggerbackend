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
    public class NotificationService : BaseService<NotificationModel, Notification, int>, INotificationService
    {
        private readonly INotificationRepository notificationRepository;

        public NotificationService(IMapper mapper, INotificationRepository notificationRepository, IUnitOfWork unitOfWork) : base(mapper, notificationRepository, unitOfWork)
        {
            this.notificationRepository = notificationRepository;
        }
    }
}
