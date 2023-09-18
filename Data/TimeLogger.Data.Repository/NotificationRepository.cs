using TimeLogger.Data.Database;
using TimeLogger.Data.Entity;
using TimeLogger.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Data.Repository
{
    public class NotificationRepository : BaseRepository<Notification, int>, INotificationRepository
    {
        public NotificationRepository(IApplicationDbContext context) : base(context)
        {
        }
    }
}
