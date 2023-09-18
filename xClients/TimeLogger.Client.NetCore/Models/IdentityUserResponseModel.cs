using System.Collections.Generic;
using System;

namespace TimeLogger.Client.NetCore.Models
{
    public class IdentityUserResponseModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool HasPassword { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public int RecoveryCodesLeft { get; set; }

        public TwoFactorTypeModel TwoFactorType { get; set; }
        public CompanyModel Company { get; set; }
        public StatusModel Status { get; set; }
        public List<AddressModel> Addresses { get; set; }
        public List<string> Roles { get; set; }
        public IList<UserLoginInfoModel> Logins { get; set; }
        public IList<AuthenticationSchemeModel> OtherLogins { get; set; }
    }
}
