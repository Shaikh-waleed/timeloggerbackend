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
    public interface ICompanyService
    {
        Task<ResponseModel<List<CompanyModel>>> Get(string search = null, int? pageNumber = null, int? pageSize = null);
        Task<ResponseModel<CompanyModel>> Get(int id);
        Task<ResponseModel<CompanyModel>> Add(string userId, CompanyModel model);
        Task<ResponseModel<CompanyModel>> Update(CompanyModel model);
        Task<ResponseModel<CompanyModel>> Update(string userId, int companyId);
        Task<ResponseModel> Delete(int id);
        Task<ResponseModel> Delete(string userId, int companyId);
    }
}
