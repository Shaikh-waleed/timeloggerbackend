using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeLogger.Client.NetCore.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
        public T Data { get; set; }
    }
}