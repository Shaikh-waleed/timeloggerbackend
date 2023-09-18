using TimeLogger.Data.Database;
using TimeLogger.Data.Entity;
using TimeLogger.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Data.Repository
{
    public class StatusRepository : BaseRepository<Status, int>, IStatusRepository
    {
        public StatusRepository(IApplicationDbContext context) : base(context)
        {
        }
    }
}
