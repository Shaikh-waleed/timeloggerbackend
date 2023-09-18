using AutoMapper;
using TimeLogger.Infrastructure.Configuration;
using TimeLogger.Infrastructure.Documentation;
using TimeLogger.Infrastructure.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.DependencyResolution
{
    public static class InfrastructureModule
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            Configurations.InitializeConfigurations(services, configuration);
            Documentations.RegisterServices(services);
            Utilities.RegisterServices(services);

            //services.AddSingleton<ILogging>(NLoggerUtil.GetLoggingService("nlog.config"));
        }

        public static void Configure(IApplicationBuilder app)
        {
            Documentations.RegisterApps(app);
            Utilities.RegisterApps(app);
        }
    }
}
