using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.DependencyResolution
{
    public static class Extensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            InfrastructureModule.Configure(services, configuration);
            ServiceModule.Configure(services);
            RepositoryModule.Configure(services, configuration);
            ComponentModule.Configure(services);
        }


        public static void RegisterApps(this IApplicationBuilder app)
        {
            InfrastructureModule.Configure(app);
            RepositoryModule.Configure(app);
            ServiceModule.Configure(app);
            ComponentModule.Configure(app);
        }
    }
}
