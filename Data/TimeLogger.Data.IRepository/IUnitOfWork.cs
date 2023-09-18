using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Data.IRepository
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
