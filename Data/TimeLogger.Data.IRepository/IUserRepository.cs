using TimeLogger.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Data.IRepository
{
    public interface IUserRepository : IBaseRepository<ApplicationUser, string>
    {
    }
}
