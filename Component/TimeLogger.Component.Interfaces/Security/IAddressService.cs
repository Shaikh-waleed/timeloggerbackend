using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLogger.Component.Models.Security;
using TimeLogger.Infrastructure.Models;

namespace TimeLogger.Component.Interfaces.Security
{
    public interface IAddressService
    {
        Task<ResponseModel<List<AddressModel>>> Get(string userId, int? companyId = null, int? pageNumber = null, int? pageSize = null);
        Task<ResponseModel<AddressModel>> Get(int id);
        Task<ResponseModel<AddressModel>> Add(AddressModel model);
        Task<ResponseModel<List<AddressModel>>> AddRange(List<AddressModel> models);
        Task<ResponseModel<AddressModel>> Update(AddressModel model);
        Task<ResponseModel> Delete(int id);
    }
}
