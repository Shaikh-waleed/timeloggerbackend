using TimeLogger.Data.Database;
using TimeLogger.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IApplicationDbContext dbContext;

        public UnitOfWork(IApplicationDbContext context)
        {
            dbContext = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
