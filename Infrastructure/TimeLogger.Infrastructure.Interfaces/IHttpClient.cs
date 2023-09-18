using TimeLogger.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Interfaces
{
    public interface IHttpClient
    {
        Task SetBearerToken(string token);
        //Task<LoginResponseModel> TokenAsync(string action, object data);
        Task<ResponseModel<T>> GetAsync<T>(string action);
        Task<ResponseModel<T>> PostAsync<T>(string action);
        Task<ResponseModel<T>> PostAsync<T>(string action, object data);
        Task<ResponseModel<T>> PutAsync<T>(string action, object data);
        Task<ResponseModel<T>> DeleteAsync<T>(string action);
    }
}
