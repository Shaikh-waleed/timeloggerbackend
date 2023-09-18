using TimeLogger.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeLogger.Data.Database
{
    public interface IApplicationDbContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        DbSet<ApplicationUser> Users { get; set; }
        // DbSet<Permission> Permissions { get; set; }

        DbSet<Status> Statuses { get; set; }
        DbSet<StatusType> StatusTypes { get; set; }

        DbSet<Notification> Notifications { get; set; }
        DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        DbSet<NotificationType> NotificationTypes { get; set; }
    }
}
