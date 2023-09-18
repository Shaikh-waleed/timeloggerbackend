using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using TimeLogger.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Business.IService
{
    public interface IUserService : IBaseService<UserModel, ApplicationUser, string>
    {
        Task<ResponseModel<List<UserResponseModel>>> GetUsers(string search = null, int? pageNumber = null, int? pageSize = null);
    }
}
