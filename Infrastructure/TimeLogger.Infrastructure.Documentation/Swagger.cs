using TimeLogger.Infrastructure.Utility.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Documentation
{
    public static class Swagger
    {
        public static void ConfigureService(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo 
                {
                    Version = "v1",
                    Title = "TimeLogger API",
                    Description = "TimeLogger Web API"
                });

                options.CustomSchemaIds(sch => sch.FullName);

                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerFilter>();

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                };
                options.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(securitySchema, new[] { "Bearer" });
                options.AddSecurityRequirement(securityRequirement);
            });
        }

        public static void ConfigureApp(IApplicationBuilder apps)
        {
            apps.UseSwagger();
            apps.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TimeLogger API V1");
            });
        }
    }
}
