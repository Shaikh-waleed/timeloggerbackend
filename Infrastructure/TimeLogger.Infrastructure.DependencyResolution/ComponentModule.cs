using TimeLogger.Component.Communication;
using TimeLogger.Component.Interfaces;
using TimeLogger.Component.Security;
using TimeLogger.Infrastructure.Documentation;
using TimeLogger.Infrastructure.Models;
using TimeLogger.Infrastructure.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.DependencyResolution
{
    public static class ComponentModule
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Securities.RegisterServices(services);
            Communications.RegisterServices(services);

            //services.AddSingleton<ILogging>(NLoggerUtil.GetLoggingService("nlog.config"));
        }

        public static void Configure(IApplicationBuilder app)
        {
            Securities.RegisterApps(app);
        }
    }
}
