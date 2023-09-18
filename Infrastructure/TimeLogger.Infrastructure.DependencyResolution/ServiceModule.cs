using TimeLogger.Business.IService;
using TimeLogger.Business.Service;
using TimeLogger.Component.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.DependencyResolution
{
    public static class ServiceModule
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IStatusTypeService, StatusTypeService>();

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddScoped<INotificationTypeService, NotificationTypeService>();
        }

        public static void Configure(IApplicationBuilder app)
        {
        }
    }
}
