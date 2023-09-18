using AutoMapper;
using TimeLogger.Infrastructure.Models.Configuration;
using TimeLogger.Infrastructure.Utility.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Configuration
{
    public static class Configurations
    {
        /// <summary>
        /// Add all default TimeLogger services. Either you can add all TimeLogger services one by one or just call this method.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection InitializeConfigurations(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TimeLoggerOptions>(configuration.GetSection("TimeLogger"));
            services.Configure<ComponentOptions>(configuration.GetSection("Component"));
            services.Configure<InfrastructureOptions>(configuration.GetSection("Infrastructure"));
            services.Configure<SecurityOptions>(configuration.GetSection("Security"));
            services.Configure<GoogleOptions>(configuration.GetSection("Google"));
            services.Configure<OutlookOptions>(configuration.GetSection("Outlook"));
            services.Configure<FacebookOptions>(configuration.GetSection("Facebook"));
            services.Configure<TwitterOptions>(configuration.GetSection("Twitter"));

            AppServicesHelper.Configuration = configuration;

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //var config = ModelMapper.ConfigurationProvider();
            //IMapper mapper = config.CreateMapper();
            //services.AddTransient<IMapper, ModelMapper>();
            //services.AddSingleton(mapper);

            return services;
        }
    }
}
