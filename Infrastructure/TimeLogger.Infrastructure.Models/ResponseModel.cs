using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Models
{
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
        public object Data { get; set; }

        public ResponseModel()
        {

        }

        public ResponseModel(bool success, object data = null, string message = "", int total = 0)
        {
            this.Success = success;
            this.Data = data;
            this.Message = message;
            this.Total = total;
        }

        public static ResponseModel Create(bool success, object data = null, string message = "", int total = 0)
        {
            return new ResponseModel() { Success = success, Data = data, Message = message, Total = total };
        }

        public static ResponseModel Failed(string message, object data = null, int total = 0)
        {
            return new ResponseModel(false, data, message, total);
        }

        public static ResponseModel Succeed(object data = null, int total = 0, string message = "")
        {
            return new ResponseModel(true, data, message, total);
        }
    }

    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
        public T Data { get; set; }
    }
}