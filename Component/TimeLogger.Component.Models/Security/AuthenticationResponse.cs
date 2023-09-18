using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Models.Security
{
    public class AuthenticationResponse
    {
        public ResponseType ResponseType { get; set; }

        public string Data;

        public AuthenticationResponse()
        {

        }
        public AuthenticationResponse(ResponseType responseType, string data)
        {
            ResponseType = responseType;
            Data = data;
        }
        public static AuthenticationResponse Create(ResponseType responseType, string data)
        {
            return new AuthenticationResponse(responseType, data);
        }

        public static AuthenticationResponse Error(string data)
        {
            return new AuthenticationResponse(ResponseType.Error, data);
        }

        public static AuthenticationResponse Success(string data)
        {
            return new AuthenticationResponse(ResponseType.Success, data);
        }
    }
}
