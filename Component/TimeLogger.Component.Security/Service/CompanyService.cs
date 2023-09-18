using AutoMapper;
using IdentityModel;
using TimeLogger.Component.Interfaces.Security;
using TimeLogger.Component.Models.Security;
using TimeLogger.Component.Security.Entities;
using TimeLogger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Security.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly IMapper _mapper;
        private readonly ISecurityDbContext _dbContext;
        private readonly ISecurityService _securityService;

        public CompanyService(IMapper mapper, ISecurityDbContext dbContext, ISecurityService securityService)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _securityService = securityService;
        }

        public async Task<ResponseModel<List<CompanyModel>>> Get(string search = null, int? pageNumber = null, int? pageSize = null)
        {
            var query = _dbContext.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower())
                                      || x.Website.ToLower().Contains(search.ToLower()));

            var total = await query.CountAsync();
            var list = !pageNumber.HasValue
                                ? await query.ToListAsync()
                                : await query.Skip(pageSize.Value * (pageNumber.Value - 1)).Take(pageSize.Value).ToListAsync();

            var result = _mapper.Map<List<CompanyModel>>(list);
            return new ResponseModel<List<CompanyModel>> { Success = true, Message = "", Data = result, Total = total };
        }

        public async Task<ResponseModel<CompanyModel>> Get(int id)
        {
            var entity = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (entity == null)
                return new ResponseModel<CompanyModel> { Success = false, Message = "No company record found." };

            var result = _mapper.Map<CompanyModel>(entity);
            return new ResponseModel<CompanyModel> { Success = true, Message = "", Data = result };
        }

        public async Task<ResponseModel<CompanyModel>> Add(string userId, CompanyModel model)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new ResponseModel<CompanyModel> { Success = false, Message = "User id is required." };

            if (model == null)
                return new ResponseModel<CompanyModel> { Success = false, Message = "Company information is required." };

            var entity = await _dbContext.Companies.AddAsync(_mapper.Map<Company>(model));
            if (entity == null)
                return new ResponseModel<CompanyModel> { Success = false, Message = "Something went wrong in adding company." };

            var userUpdateResult = await _securityService.UpdateUser(new UpdateUserModel
            {
                Id = userId,
                CompanyId = model.Id
            }, true);
            if (!userUpdateResult.Success)
                return new ResponseModel<CompanyModel> { Success = false, Message = userUpdateResult.Message };

            var result = _mapper.Map<CompanyModel>(entity);
            return new ResponseModel<CompanyModel> { Success = true, Message = "User company added successfully.", Data = result };
        }

        public async Task<ResponseModel<CompanyModel>> Update(CompanyModel model)
        {
            if (model.Id <= 0)
                return new ResponseModel<CompanyModel> { Success = false, Message = "Invalid company id." };

            var entity = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
            if (entity == null)
                return new ResponseModel<CompanyModel> { Success = false, Message = "No company record found." };

            entity.Name = model.Name;
            entity.Phone = model.Phone;
            entity.Website = model.Website;

            _dbContext.Companies.Update(entity);
            var result = _mapper.Map<CompanyModel>(entity);
            return new ResponseModel<CompanyModel> { Success = true, Message = "Updated successfully.", Data = result };
        }

        public async Task<ResponseModel<CompanyModel>> Update(string userId, int companyId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new ResponseModel<CompanyModel> { Success = false, Message = "User id is required." };

            if (companyId <= 0)
                return new ResponseModel<CompanyModel> { Success = false, Message = "Invalid company id." };

            var response = await Get(companyId);
            if (!response.Success)
                return response;

            var userUpdateResult = await _securityService.UpdateUser(new UpdateUserModel
            {
                Id = userId,
                CompanyId = response.Data.Id
            }, true);
            if (!userUpdateResult.Success)
                return new ResponseModel<CompanyModel> { Success = false, Message = userUpdateResult.Message };

            return new ResponseModel<CompanyModel> { Success = true, Message = "User company updated successfully.", Data = response.Data };
        }

        public async Task<ResponseModel> Delete(int id)
        {
            var entity = await _dbContext.Companies
                                         .Include(x => x.Users)
                                         .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (entity == null)
                return new ResponseModel { Success = false, Message = "No company record found." };

            if (entity.Users.Any())
                return new ResponseModel { Success = false, Message = "This company is associated with users, hance connot be deleted." };

            _dbContext.Companies.Remove(entity);
            return new ResponseModel { Success = true, Message = "Deleted successfully." };
        }

        public async Task<ResponseModel> Delete(string userId, int companyId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new ResponseModel { Success = false, Message = "User id is required." };

            if (companyId <= 0)
                return new ResponseModel { Success = false, Message = "Invalid company id." };

            var userResponse = await _securityService.GetUser(userId);
            if (userResponse == null)
                return new ResponseModel { Success = false, Message = "Invalid user." };

            if (userResponse.Data.Company != null && !userResponse.Data.Company.Id.Equals(companyId))
                return new ResponseModel { Success = false, Message = "User is not in this company." };

            var userUpdateResult = await _securityService.UpdateUser(new UpdateUserModel
            {
                Id = userId,
                CompanyId = null
            }, true);
            if (!userUpdateResult.Success)
                return new ResponseModel { Success = false, Message = userUpdateResult.Message };

            return new ResponseModel { Success = true, Message = "User company deleted successfully." };
        }
    }
}
