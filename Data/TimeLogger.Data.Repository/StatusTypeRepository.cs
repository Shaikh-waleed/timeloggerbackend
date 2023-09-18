using TimeLogger.Data.Database;
using TimeLogger.Data.Entity;
using TimeLogger.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Data.Repository
{
    public class StatusTypeRepository : BaseRepository<StatusType, int>, IStatusTypeRepository
    {
        public StatusTypeRepository(IApplicationDbContext context) : base(context)
        {
        }
    }
}
