using AutoMapper;
using TimeLogger.Business.IService;
using TimeLogger.Business.Model;
using TimeLogger.Data.Entity;
using TimeLogger.Data.IRepository;
using TimeLogger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TimeLogger.Business.Service
{
    public class UserService : BaseService<UserModel, ApplicationUser, string>, IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository userRepository;

        public UserService(IMapper mapper, IUserRepository userRepository, IUnitOfWork unitOfWork) : base(mapper, userRepository, unitOfWork)
        {
            _mapper = mapper;
            this.userRepository = userRepository;
        }

        public async Task<ResponseModel<List<UserResponseModel>>> GetUsers(string search = null, int? pageNumber = null, int? pageSize = null)
        {
            var query = userRepository.Get();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.FirstName.ToLower().Contains(search.ToLower())
                                      || x.LastName.ToLower().Contains(search.ToLower())
                                      || x.Picture.ToLower().Contains(search.ToLower()));

            var total = await query.CountAsync();
            var list = !pageNumber.HasValue
                                ? await query.ToListAsync()
                                : await query.Skip(pageSize.Value * (pageNumber.Value - 1)).Take(pageSize.Value).ToListAsync();

            var result = _mapper.Map<List<UserResponseModel>>(list);
            return new ResponseModel<List<UserResponseModel>> { Success = true, Message = "", Data = result, Total = total };
        }
    }
}
