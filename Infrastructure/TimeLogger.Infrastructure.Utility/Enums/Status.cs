using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Enums
{
    public enum StatusTypes
    {
        UserStatus = 1,
        NotificationStatus = 2
    }

    public enum UserStatus
    {
        Preactive,
        Active,
        Inactive,
        Canceled,
        Frozen,
        Blocked
    }

    public enum NotificationStatus
    {
        Created,
        Queued,
        Succeeded,
        Failed
    }
}
