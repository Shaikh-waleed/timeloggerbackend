using AutoMapper;
using TimeLogger.Infrastructure.Interfaces;
using TimeLogger.Infrastructure.Utility.Filters;
using TimeLogger.Infrastructure.Utility.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Utility
{
    public static class Utilities
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Todo: we have to change to Interfaces in some of following:

            //services.AddTransient<IHttpClient, HttpClientHelper>();

            services.AddScoped<ValidateModelState>();
        }

        public static void RegisterApps(IApplicationBuilder app)
        {
            app.Use(next =>
                async context =>
                {
                    if (HttpMethods.IsPatch(context.Request.Method))
                    {
                        context.Request.EnableBuffering();

                        // Leave the body open so the next middleware can read it.
                        var reader = new System.IO.StreamReader(context.Request.Body, System.Text.Encoding.UTF8, leaveOpen: true);
                        using (reader)
                        {
                            var body = await reader.ReadToEndAsync();

                            // Adding the body to the request context.
                            context.Items.Add($"Body{HttpMethods.Patch}", body);

                            // Reset the request body stream position so the next middleware can read it
                            context.Request.Body.Position = 0;
                        }
                    }
                    await next(context);
                    return;
                });
        }
    }
}
