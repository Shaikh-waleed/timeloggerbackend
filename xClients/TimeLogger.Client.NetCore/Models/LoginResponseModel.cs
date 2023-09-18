using TimeLogger.Client.NetCore.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeLogger.Client.NetCore.Models
{
    public class LoginResponseModel
    {
        public LoginStatus Status { get; set; }
        public string Message { get; set; }
        public UserAuthenticationInfoModel Data { get; set; }
    }

    public class LoginResponseModel<T>
    {
        public LoginStatus Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
