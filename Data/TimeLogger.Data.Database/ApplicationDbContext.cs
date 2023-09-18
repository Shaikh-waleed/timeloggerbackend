using TimeLogger.Component.Security;
using TimeLogger.Component.Security.Entities;
using TimeLogger.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TimeLogger.Data.Database
{
    public class ApplicationDbContext : SecurityDbContext, IApplicationDbContext, ISecurityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public virtual DbSet<ApplicationUser> Users { get; set; }
        //public virtual DbSet<Permission> Permissions { get; set; }

        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<StatusType> StatusTypes { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
    }
}
