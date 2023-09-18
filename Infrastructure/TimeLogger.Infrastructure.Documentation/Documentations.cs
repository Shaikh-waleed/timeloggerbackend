using TimeLogger.Infrastructure.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Documentation
{
    public static class Documentations
    {
        private static IOptionsSnapshot<InfrastructureOptions> infrastructureOptions = null;

        public static void RegisterServices(IServiceCollection services)
        {
            infrastructureOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<InfrastructureOptions>>();
            switch (infrastructureOptions.Value.Documentation)
            {
                case "Swagger":
                    Swagger.ConfigureService(services);
                    break;
                default:
                    break;
            }
        }

        public static void RegisterApps(IApplicationBuilder app)
        {
            switch (infrastructureOptions.Value.Documentation)
            {
                case "Swagger":
                    Swagger.ConfigureApp(app);
                    break;
                default:
                    break;
            }
        }
    }
}
