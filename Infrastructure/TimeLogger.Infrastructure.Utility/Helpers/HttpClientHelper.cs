using TimeLogger.Infrastructure.Models;
using TimeLogger.Infrastructure.Utility.Enums;
using TimeLogger.Infrastructure.Utility.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    public static class HttpClientHelper
    {
        public async static Task<ResponseModel<T>> GetAsync<T>(string address, string token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var response = await httpClient.GetAsync(address);
                    if (!response.IsSuccessStatusCode)
                        return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                    var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }
        public async static Task<ResponseModel<T>> GetAsync<T>(string address, string action, string token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(address);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var response = await httpClient.GetAsync(action);
                    if (!response.IsSuccessStatusCode)
                        return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                    var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }

        public async static Task<ResponseModel<T>> PostAsync<T>(string address, string action, object data, HttpContentType contentType)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(address);
                    dynamic content = contentType == HttpContentType.Json
                                                        ? JsonConvert.SerializeObject(data)
                                                        : data?.ToKeyValue();

                    HttpResponseMessage response = await httpClient.PostAsync(action, 
                                                                              data == null 
                                                                                ? null 
                                                                                : contentType == HttpContentType.Json 
                                                                                    ? new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json) 
                                                                                    : new FormUrlEncodedContent(content));
                    if (!response.IsSuccessStatusCode)
                        return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                    var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                    return responseModel;
                }
            }
            catch (HttpRequestException ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }
        public async static Task<ResponseModel<T>> PostAsync<T>(string address, string action, object data, HttpContentType contentType, string token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(address);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    dynamic content = contentType == HttpContentType.Json
                                                        ? JsonConvert.SerializeObject(data)
                                                        : data?.ToKeyValue();

                    HttpResponseMessage response = await httpClient.PostAsync(action,
                                                                              data == null
                                                                                ? null
                                                                                : contentType == HttpContentType.Json
                                                                                    ? new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json)
                                                                                    : new FormUrlEncodedContent(content));
                    if (!response.IsSuccessStatusCode)
                        return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                    var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                    return responseModel;
                }
            }
            catch (HttpRequestException ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }

        public async static Task<ResponseModel<T>> PutAsync<T>(string address, string action, object data, HttpContentType contentType)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(address);
                    dynamic content = contentType == HttpContentType.Json
                                                        ? JsonConvert.SerializeObject(data)
                                                        : data?.ToKeyValue();

                    HttpResponseMessage response = await httpClient.PutAsync(action,
                                                                              data == null
                                                                                ? null
                                                                                : contentType == HttpContentType.Json
                                                                                    ? new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json)
                                                                                    : new FormUrlEncodedContent(content));
                    if (!response.IsSuccessStatusCode)
                        return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                    var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                    return responseModel;
                }
            }
            catch (HttpRequestException ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }
        public async static Task<ResponseModel<T>> PutAsync<T>(string address, string action, object data, HttpContentType contentType, string token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(address);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    dynamic content = contentType == HttpContentType.Json
                                                        ? JsonConvert.SerializeObject(data)
                                                        : data?.ToKeyValue();

                    HttpResponseMessage response = await httpClient.PutAsync(action,
                                                                              data == null
                                                                                ? null
                                                                                : contentType == HttpContentType.Json
                                                                                    ? new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json)
                                                                                    : new FormUrlEncodedContent(content));
                    if (!response.IsSuccessStatusCode)
                        return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                    var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                    return responseModel;
                }
            }
            catch (HttpRequestException ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }

        public async static Task<ResponseModel<T>> DeleteAsync<T>(string address, string action)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(address);

                    var response = await httpClient.DeleteAsync(action);
                    if (!response.IsSuccessStatusCode)
                        return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                    var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                    return responseModel;
                }
            }
            catch (HttpRequestException ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }
        public async static Task<ResponseModel<T>> DeleteAsync<T>(string address, string action, string token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(address);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var response = await httpClient.DeleteAsync(action);
                    if (!response.IsSuccessStatusCode)
                        return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                    var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                    return responseModel;
                }
            }
            catch (HttpRequestException ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }

        //public async static Task<ResponseModel<T>> PostDataWithUploadAsync<T>(Uri baseAddress, string action, object data, HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        if (file == null)
        //            return await PostAsync<T>(action, data);

        //        //using (HttpClient httpClient = new HttpClient())
        //        //{
        //        //    httpClient.BaseAddress = baseAddress;

        //        using (var content = new MultipartFormDataContent())
        //        {
        //            content.Add(new StreamContent(file.InputStream), "File", file.FileName.Replace(" ", ""));

        //            if (data != null)
        //                foreach (var property in data.GetType().GetProperties())
        //                {
        //                    object value = property.GetValue(data, null);
        //                    if (value != null)
        //                        content.Add(new StringContent(value.ToString()), property.Name);
        //                }

        //            //Create the request.
        //            var response = await httpClient.PostAsync(action, content);
        //            //Process the response.
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var responseModel = await JsonSerializer.DeserializeAsync<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
        //                return new ResponseModel<T> { Success = responseModel.Success, Message = responseModel.Message, Data = responseModel.Data };
        //            }
        //            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        //            {
        //                throw new Exception(await response.Content.ReadAsStringAsync());
        //            }
        //        }
        //        //}
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        //await EmailManager.SendEmail("Error", ex.ToString(), ConfigurationManager.AppSettings["SMTPSupportError"]);
        //    }

        //    return default(ResponseModel<T>);
        //}
    }
}
