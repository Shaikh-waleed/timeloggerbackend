using AutoMapper;
using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Configuration.Mappers
{
    public class NotificationMapper : Profile
    {
        public NotificationMapper()
        {
            CreateMap<Notification, NotificationModel>().ReverseMap();
            CreateMap<Notification, NotificationResponseModel>().ReverseMap();

            CreateMap<NotificationType, NotificationTypeModel>().ReverseMap();

            CreateMap<NotificationTemplate, NotificationTemplateModel>().ReverseMap();
            CreateMap<NotificationTemplate, NotificationTemplateResponseModel>().ReverseMap();
        }
    }
}
