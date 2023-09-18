using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class LoginResponse
    {
        public LoginStatus Status { get; set; }
        public string Message { get; set; }
        public Object Data { get; set; }
    }

    public class LoginResponse<T>
    {
        public LoginStatus Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
