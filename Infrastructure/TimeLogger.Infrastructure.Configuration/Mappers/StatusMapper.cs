using AutoMapper;
using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Configuration.Mappers
{
    public class StatusMapper : Profile
    {
        public StatusMapper()
        {
            CreateMap<Status, StatusModel>().ReverseMap();
            CreateMap<Status, StatusResponseModel>().ReverseMap();

            CreateMap<StatusType, StatusTypeModel>().ReverseMap();
        }
    }
}
