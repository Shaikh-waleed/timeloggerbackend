using System.Collections.Generic;
using System;

namespace TimeLogger.Client.NetCore.Models
{
    public class UserAuthenticationInfoModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool HasPassword { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public int RecoveryCodesLeft { get; set; }
        public TwoFactorTypeModel TwoFactorType { get; set; }
        public IList<UserLoginInfoModel> Logins { get; set; }
        public IList<AuthenticationSchemeModel> OtherLogins { get; set; }
        public bool BrowserRemembered { get; set; }

        public string AccessToken { get; set; }
    }
}
