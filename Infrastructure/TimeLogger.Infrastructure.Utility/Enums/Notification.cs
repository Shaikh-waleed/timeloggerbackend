using System;
using System.Collections.Generic;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Enums
{
    public enum NotificationTypes
    {
        Email = 1,
        Sms = 2,
        Site = 3,
        Push = 4
    }

    public enum NotificationTemplates
    {
        EmailUserRegisteration = 1,
        SmsUserRegisteration = 2,
        EmailForgotPassword = 3,
        SmsForgotPassword = 4,
        EmailSetPassword = 5,
        SmsSetPassword = 6,
        EmailChangePassword = 7,
        SmsChangePassword = 8,
        EmailTwoFactorToken = 9,
        SmsTwoFactorToken = 10,
        EmailUserStatusChange = 11,
        SmsUserStatusChange = 12
    }
}
