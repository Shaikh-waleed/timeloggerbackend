using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeLogger.Client.Mvc.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
    }
}