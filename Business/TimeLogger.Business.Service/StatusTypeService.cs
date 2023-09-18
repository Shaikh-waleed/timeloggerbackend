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
    public class StatusTypeService : BaseService<StatusTypeModel, StatusType, int>, IStatusTypeService
    {
        private readonly IStatusTypeRepository statusTypeRepository;

        public StatusTypeService(IMapper mapper, IStatusTypeRepository statusTypeRepository, IUnitOfWork unitOfWork) : base(mapper, statusTypeRepository, unitOfWork)
        {
            this.statusTypeRepository = statusTypeRepository;
        }
    }
}
