using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TimeLogger.Data.Database;
using TimeLogger.Data.IRepository;
using TimeLogger.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using TimeLogger.Component.Security.Entities;
using TimeLogger.Infrastructure.Models.Configuration;

namespace TimeLogger.Infrastructure.DependencyResolution
{
    public static class RepositoryModule
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                       options => options.UseSqlServer(
                           configuration.GetConnectionString("MsSqlConnection"),
                           msSqlServerOptions => msSqlServerOptions.MigrationsAssembly("TimeLogger.Data.Database")
                       )
                   );

            var componentOptions = services.BuildServiceProvider().GetService<Microsoft.Extensions.Options.IOptionsSnapshot<ComponentOptions>>();
            if (componentOptions.Value.Security.SecurityService == "AspnetIdentity")
            {
                //services.AddDbContext<Component.Security.SecurityDbContext>(
                //    options => options.UseSqlServer(
                //        configuration.GetConnectionString("MsSqlConnection"),
                //        mySqlServerOptions => mySqlServerOptions.MigrationsAssembly("TimeLogger.Data.Database")
                //    )
                //);
                //services.AddTransient<Component.Security.ISecurityDbContext, Component.Security.SecurityDbContext>();

                services.AddIdentity<UserIdentity, RoleIdentity>(options =>
                {
                    options.User.RequireUniqueEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

                services.BuildServiceProvider().GetService<UserManager<UserIdentity>>();
                services.AddTransient<Component.Security.ISecurityDbContext, ApplicationDbContext>();
            }

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IUnitOfWork>(unitOfWork => new UnitOfWork(unitOfWork.GetService<IApplicationDbContext>()));

            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IStatusRepository, StatusRepository>();
            services.AddTransient<IStatusTypeRepository, StatusTypeRepository>();

            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddTransient<INotificationTypeRepository, NotificationTypeRepository>();
        }

        public static void Configure(IApplicationBuilder app)
        {
            DbMigrator.Migrate(app);
            DataSeeder.Seed(app);
        }
    }
}
