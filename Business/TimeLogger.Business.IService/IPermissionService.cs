using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Business.IService
{
    public interface IPermissionService : IBaseService<PermissionModel, Permission, int>
    {
    }
}
