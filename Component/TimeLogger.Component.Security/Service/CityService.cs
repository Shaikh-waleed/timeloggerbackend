using AutoMapper;
using TimeLogger.Component.Interfaces.Security;
using TimeLogger.Component.Models.Security;
using TimeLogger.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Security.Service
{
    public class CityService : ICityService
    {
        private readonly IMapper _mapper;
        private readonly ISecurityDbContext _dbContext;

        public CityService(IMapper mapper, ISecurityDbContext dbContext) 
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<List<CityModel>>> Get(int? countryId = null, string search = null, int? pageNumber = null, int? pageSize = null)
        {
            var query = _dbContext.Cities.AsQueryable();

            if (countryId.HasValue)
                query = query.Where(x => x.CountryId.Equals(countryId.Value));

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower())
                                      || x.Code.Equals(search));

            var total = await query.CountAsync();
            var list = !pageNumber.HasValue
                                ? await query.ToListAsync()
                                : await query.Skip(pageSize.Value * (pageNumber.Value - 1)).Take(pageSize.Value).ToListAsync();

            var result = _mapper.Map<List<CityModel>>(list);
            return new ResponseModel<List<CityModel>> { Success = true, Message = "", Data = result, Total = total };
        }

        public async Task<ResponseModel<CityModel>> Get(int id)
        {
            var entity = await _dbContext.Cities.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (entity == null)
                return new ResponseModel<CityModel> { Success = false, Message = "No city record found." };

            var result = _mapper.Map<CityModel>(entity);
            return new ResponseModel<CityModel> { Success = true, Message = "", Data = result };
        }
    }
}
