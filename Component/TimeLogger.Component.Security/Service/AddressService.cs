using AutoMapper;
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
    public class AddressService : IAddressService
    {
        private readonly IMapper _mapper;
        private readonly ISecurityDbContext _dbContext;

        public AddressService(IMapper mapper, ISecurityDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<List<AddressModel>>> Get(string userId, int? companyId = null, int? pageNumber = null, int? pageSize = null)
        {
            var query = _dbContext.Addresses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(x=> x.UserId.Equals(userId));

            if (companyId.HasValue)
                query = query.Where(x => x.CompanyId.Equals(companyId));

            var total = await query.CountAsync();
            var list = !pageNumber.HasValue
                                ? await query.ToListAsync()
                                : await query.Skip(pageSize.Value * (pageNumber.Value - 1)).Take(pageSize.Value).ToListAsync();

            var result = _mapper.Map<List<AddressModel>>(list);
            return new ResponseModel<List<AddressModel>> { Success = true, Message = "", Data = result, Total = total };
        }

        public async Task<ResponseModel<AddressModel>> Get(int id)
        {
            var entity = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (entity == null)
                return new ResponseModel<AddressModel> { Success = false, Message = "No address record found." };

            var result = _mapper.Map<AddressModel>(entity);
            return new ResponseModel<AddressModel> { Success = true, Message = "", Data = result };
        }

        public async Task<ResponseModel<AddressModel>> Add(AddressModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId) && !model.CompanyId.HasValue)
                return new ResponseModel<AddressModel> { Success = false, Message = "User id or company id is required." };

            var entity = await _dbContext.Addresses.AddAsync(_mapper.Map<Addresses>(model));
            if (entity == null)
                return new ResponseModel<AddressModel> { Success = false, Message = "Something went wrong in adding address." };

            var result = _mapper.Map<AddressModel>(entity);
            return new ResponseModel<AddressModel> { Success = true, Message = "Added successfully.", Data = result };
        }

        public async Task<ResponseModel<List<AddressModel>>> AddRange(List<AddressModel> models)
        {
            if (models.Any(x => x.UserId == null && ! x.CompanyId.HasValue))
                return new ResponseModel<List<AddressModel>> { Success = false, Message = "User id or company id is required." };

            await _dbContext.Addresses.AddRangeAsync(_mapper.Map<List<Addresses>>(models));
            return new ResponseModel<List<AddressModel>> { Success = true, Message = "Added successfully.", Data = models };
        }

        public async Task<ResponseModel<AddressModel>> Update(AddressModel model)
        {
            if (model.Id <= 0)
                return new ResponseModel<AddressModel> { Success = false, Message = "Invalid address id." };

            if (string.IsNullOrWhiteSpace(model.UserId) && !model.CompanyId.HasValue)
                return new ResponseModel<AddressModel> { Success = false, Message = "User id or company id is required." };

            var entity = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
            if (entity == null)
                return new ResponseModel<AddressModel> { Success = false, Message = "No address record found." };

            entity.UserId = model.UserId;
            entity.CompanyId = model.CompanyId;
            entity.Description = model.Description;
            entity.Address = model.Address;
            entity.ZipCode = model.ZipCode;
            entity.CountryId = model.CountryId;
            entity.CityId = model.CountryId;
            entity.IsDefault = model.IsDefault;

            _dbContext.Addresses.Update(entity);
            var result = _mapper.Map<AddressModel>(entity);
            return new ResponseModel<AddressModel> { Success = true, Message = "Updated successfully.", Data = result };
        }

        public async Task<ResponseModel> Delete(int id)
        {
            var entity = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (entity == null)
                return new ResponseModel { Success = false, Message = "No address record found." };

            _dbContext.Addresses.Remove(entity);
            return new ResponseModel { Success = true, Message = "Deleted successfully." };
        }
    }
}
