using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Constants
{
    public static class UserStatusCode
    {
        public const string Preactive = "101";
        public const string Active = "102";
        public const string Inactive = "103";
        public const string Canceled = "104";
        public const string Frozen = "105";
        public const string Blocked = "106";
    }

    public static class NotificationStatusCode
    {
        public const string Created = "201";
        public const string Queued = "202";
        public const string Succeeded = "203";
        public const string Failed = "204";
    }
}
