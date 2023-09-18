using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TimeLogger.Data.Database
{
    public static class DbMigrator
    {
        public static Task Migrate(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                //var componentOptions = scope.ServiceProvider.GetService<Microsoft.Extensions.Options.IOptionsSnapshot<Infrastructure.Models.ComponentOptions>>();
                //if (componentOptions.Value.Security.SecurityService == "AspnetIdentity")
                //    scope.ServiceProvider.GetRequiredService<SqlDbContextWithIdentity>().Database.Migrate();
                //else
                //    scope.ServiceProvider.GetRequiredService<SqlDbContext>().Database.Migrate();

                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                return Task.FromResult(0);
            }
        }
    }
}
