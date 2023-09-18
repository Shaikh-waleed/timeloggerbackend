using TimeLogger.Component.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Infrastructure.Models;

namespace TimeLogger.Component.Interfaces.Security
{
    public interface ICityService
    {
        Task<ResponseModel<List<CityModel>>> Get(int? countryId = null, string search = null, int? pageNumber = null, int? pageSize = null);
        Task<ResponseModel<CityModel>> Get(int id);
    }
}
