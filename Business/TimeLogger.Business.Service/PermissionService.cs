using AutoMapper;
using TimeLogger.Business.IService;
using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using TimeLogger.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Business.Service
{
    public class PermissionService : BaseService<PermissionModel, Permission, int>, IPermissionService
    {
        private readonly IPermissionRepository permissionRepository;

        public PermissionService(IMapper mapper, IPermissionRepository permissionRepository, IUnitOfWork unitOfWork) : base(mapper, permissionRepository, unitOfWork)
        {
            this.permissionRepository = permissionRepository;
        }
    }
}
