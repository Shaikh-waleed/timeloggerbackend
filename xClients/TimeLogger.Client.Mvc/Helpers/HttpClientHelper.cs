using TimeLogger.Client.Mvc.Extensions;
using TimeLogger.Client.Mvc.Models;
using TimeLogger.Client.Mvc.Options;
using TimeLogger.Client.Mvc.Utilities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TimeLogger.Client.Mvc.Helpers
{
    public class HttpClientHelper
    {
        private static HttpClient httpClient;
        private static readonly string apiUrl = string.Empty;

        static HttpClientHelper()
        {
            apiUrl = TimeLoggerOptions.ApiUrl;
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(apiUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task SetBearerToken(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async static Task<LoginResponseModel> TokenAsync(object data, HttpContentType contentType, string action, string address = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(address))
                    httpClient.BaseAddress = new Uri(address);

                dynamic content = null;
                if (data == null)
                    content = null;
                else if (contentType == HttpContentType.Json)
                    content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                else
                    content = new FormUrlEncodedContent(data?.ToKeyValue());

                HttpResponseMessage response = await httpClient.PostAsync(action, content);
                if (!response.IsSuccessStatusCode)
                    return new LoginResponseModel { Status = LoginStatus.Failed, Message = response.ReasonPhrase };

                var responseModel = await JsonSerializer.DeserializeAsync<LoginResponseModel>(await response.Content.ReadAsStringAsync());
                return responseModel;
            }
            catch (HttpRequestException ex)
            {
                return new LoginResponseModel { Message = ex.Message };
            }
        }

        public async static Task<ResponseModel<T>> GetAsync<T>(string action, string address = null, string token = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(address))
                    httpClient.BaseAddress = new Uri(address);
                if (!string.IsNullOrWhiteSpace(token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync(action);
                if (!response.IsSuccessStatusCode)
                    return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                return responseModel;
            }
            catch (Exception ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }

        public async static Task<ResponseModel<T>> PostAsync<T>(object data, HttpContentType contentType, string action, string address = null, string token = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(address))
                    httpClient.BaseAddress = new Uri(address);
                if (!string.IsNullOrWhiteSpace(token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                dynamic content = null;
                if (data == null)
                    content = null;
                else if (contentType == HttpContentType.Json)
                    content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                else
                    content = new FormUrlEncodedContent(data?.ToKeyValue());

                HttpResponseMessage response = await httpClient.PostAsync(action, content);
                if (!response.IsSuccessStatusCode)
                    return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                return responseModel;
            }
            catch (Exception ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }

        public async static Task<ResponseModel<T>> PutAsync<T>(object data, HttpContentType contentType, string action, string address = null, string token = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(address))
                    httpClient.BaseAddress = new Uri(address);
                if (!string.IsNullOrWhiteSpace(token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                dynamic content = null;
                if (data == null)
                    content = null;
                else if (contentType == HttpContentType.Json)
                    content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                else
                    content = new FormUrlEncodedContent(data?.ToKeyValue());

                HttpResponseMessage response = await httpClient.PutAsync(action, content);
                if (!response.IsSuccessStatusCode)
                    return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                return responseModel;
            }
            catch (Exception ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }

        public async static Task<ResponseModel<T>> DeleteAsync<T>(string action, string address = null, string token = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(address))
                    httpClient.BaseAddress = new Uri(address);
                if (!string.IsNullOrWhiteSpace(token))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.DeleteAsync(action);
                if (!response.IsSuccessStatusCode)
                    return new ResponseModel<T> { Success = false, Message = response.ReasonPhrase, StatusCode = (int)response.StatusCode };

                var responseModel = JsonConvert.DeserializeObject<ResponseModel<T>>(await response.Content.ReadAsStringAsync());
                return responseModel;
            }
            catch (Exception ex)
            {
                return new ResponseModel<T> { Success = false, Message = ex.Message };
            }
        }

        //public async static Task<ResponseModel<T>> PostDataWithUploadAsync<T>(object data, HttpPostedFileBase file, string action, string address = null)
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
