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
    public class CountryService : ICountryService
    {
        private readonly IMapper _mapper;
        private readonly ISecurityDbContext _dbContext;

        public CountryService(IMapper mapper, ISecurityDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<List<CountryModel>>> Get(string search = null, int? pageNumber = null, int? pageSize = null)
        {
            var query = _dbContext.Countries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower())
                                      || x.Code.Equals(search));

            var total = await query.CountAsync();
            var list = !pageNumber.HasValue
                                ? await query.ToListAsync()
                                : await query.Skip(pageSize.Value * (pageNumber.Value - 1)).Take(pageSize.Value).ToListAsync();

            var result = _mapper.Map<List<CountryModel>>(list);
            return new ResponseModel<List<CountryModel>> { Success = true, Message = "", Data = result, Total = total };
        }

        public async Task<ResponseModel<CountryModel>> Get(int id)
        {
            var entity = await _dbContext.Countries.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (entity == null)
                return new ResponseModel<CountryModel> { Success = false, Message = "No country record found." };

            var result = _mapper.Map<CountryModel>(entity);
            return new ResponseModel<CountryModel> { Success = true, Message = "", Data = result };
        }
    }
}
