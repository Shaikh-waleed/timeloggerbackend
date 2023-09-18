using TimeLogger.Client.NetCore.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeLogger.Client.NetCore.Helpers
{
    public static class Configurations
    {
        /// <summary>
        /// Add all default TimeLogger services. Either you can add all TimeLogger services one by one or just call this method.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection InitializeConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TimeLoggerOptions>(configuration.GetSection("TimeLogger"));
            services.Configure<SecurityOptions>(configuration.GetSection("Security"));
            return services;
        }
    }
}
