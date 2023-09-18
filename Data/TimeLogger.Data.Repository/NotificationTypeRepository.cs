using TimeLogger.Data.Database;
using TimeLogger.Data.Entity;
using TimeLogger.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Data.Repository
{
    public class NotificationTypeRepository : BaseRepository<NotificationType, int>, INotificationTypeRepository
    {
        public NotificationTypeRepository(IApplicationDbContext context) : base(context)
        {
        }
    }
}
