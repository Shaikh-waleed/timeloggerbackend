using TimeLogger.Client.Mvc.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeLogger.Client.Mvc.Models
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