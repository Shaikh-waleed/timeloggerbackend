using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeLogger.Client.Mvc.Utilities.Enums
{
    public enum HttpContentType
    {
        Json,
        FormUrlEncoded
    }

    public enum TwoFactorTypes
    {
        None = 1,
        Email = 2,
        Phone = 3,
        Authenticator = 4
    }

    public enum LoginStatus
    {
        Locked = 0,
        AccountLocked,
        InvalidCredential,
        Succeded,
        TimeoutLocked,
        Failed,
        RequiresTwoFactor
    }
    public enum CustomClaimTypes
    {
        User,
        FirstName,
        LastName,
        ValidationCallTime,
        SecurityStamp,
        Roles
    }
}