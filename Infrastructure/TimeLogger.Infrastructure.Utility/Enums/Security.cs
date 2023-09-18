using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Enums
{
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

    public enum UserRoles
    {
        SuperAdmin,
        Admin,
        User,
        Customer,
        Merchant
    }

    public enum UserStatusType
    {
        Preactive,
        Active,
        Inactive,
        Cancel,
        Freez,
        Block
    }

    public enum CustomClaimTypes
    {
        User,
        FirstName,
        LastName,
        Roles,
        ValidationCallTime,
        SecurityStamp
    }

    public enum ResponseType
    {
        Success,
        Error
    }
}
