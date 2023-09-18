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
    public class StatusService : BaseService<StatusModel, Status, int>, IStatusService
    {
        private readonly IStatusRepository statusRepository;

        public StatusService(IMapper mapper, IStatusRepository statusRepository, IUnitOfWork unitOfWork) : base(mapper, statusRepository, unitOfWork)
        {
            this.statusRepository = statusRepository;
        }
    }
}
