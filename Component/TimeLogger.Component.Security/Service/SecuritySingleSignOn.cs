using TimeLogger.Business.IService;
using TimeLogger.Component.Interfaces.Security;
using TimeLogger.Component.Models.Security;
using TimeLogger.Infrastructure.Interfaces;
using TimeLogger.Infrastructure.Models;
using TimeLogger.Infrastructure.Models.Configuration;
using TimeLogger.Infrastructure.Utility.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TimeLogger.Component.Security.Service
{
    public class SecuritySingleSignOn : ISecurityService
    {
        //from config
        private string authority = string.Empty;
        private string baseUri = string.Empty;
        private string clientId = string.Empty;
        private string clientSecret = string.Empty;
        private string scopes = string.Empty;
        private string adminUsername = string.Empty;
        private string adminPassword = string.Empty;

        private readonly SecurityOptions _securityOptions;
        private readonly TimeLoggerOptions _timeLoggerOptions;

        private readonly IUserService _userService;

        public SecuritySingleSignOn(
            IOptionsSnapshot<SecurityOptions> securityOptions,
            IOptionsSnapshot<TimeLoggerOptions> timeLoggerOptions,

            IUserService userService)
        {
            _securityOptions = securityOptions?.Value;
            _timeLoggerOptions = timeLoggerOptions?.Value;

            authority = _securityOptions.Authority;
            baseUri = authority + "api/";
            clientId = _securityOptions.ClientId;
            clientSecret = _securityOptions.ClientSecret;
            scopes = _securityOptions.Scopes;
            adminUsername = _securityOptions.AdminUsername;
            adminPassword = _securityOptions.AdminPassword;

            _userService = userService;
        }

        public async Task<ResponseModel<IdentityResult>> CreateUser(string firstName, string lastName, string userName, string email, string password)
        {
            //var content = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("UserName", userName),
            //    new KeyValuePair<string, string>("Email", email),
            //    new KeyValuePair<string, string>("Password", password),
            //    new KeyValuePair<string, string>("ClientID", clientId)
            //});

            //var response = await _httpClient.PostAsync<IdentityResult> (baseUri + "Account/Register", content);
            //return new ResponseModel<IdentityResult> { Data = response.Data };

            throw new NotImplementedException();
        }

        public async Task<LoginResponse> CreateExternalUser(string firstName, string lastName, string email, string picture, string loginProvider, string providerKey, string providerDisplayName)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<string>> GenerateEmailVerificationToken(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> ConfirmEmail(string userId, string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> ChangeUserStatus(string userId, string status)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IdentityUserModel>> UpdateUser(UpdateUserModel model, bool updateRelated)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse<UserAuthenticationInfoModel>> Login(string userName, string password, bool persistCookie = false, bool lockoutOnFailure = false)
        {
            //var content = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string,string>("UserName",userName),
            //    new KeyValuePair<string, string>("Password",password),
            //    new KeyValuePair<string, string>("ClientId",clientId),
            //    new KeyValuePair<string, string>("ClientSecret",clientSecret),
            //    new KeyValuePair<string, string>("Scopes",scopes)
            //});

            //var response = await _httpClient.PostAsync<LoginResponse<UserAuthenticationInfoModel>>(baseUri + "account/login", content);
            //return response.Data;

            throw new NotImplementedException();
        }

        public async Task<LoginResponse> ExternalLogin(string loginProvider, string providerKey, bool isPersistent = false, bool bypassTwoFactor = false)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse> TwoFactorLogin(string provider, string code, bool persistCookie = false, bool rememberMachine = false)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse> RecoveryCodeLogin(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<List<AuthenticationSchemeModel>>> GetLoginProviders()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<List<Microsoft.AspNetCore.Identity.UserLoginInfo>>> GetLogins(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<Microsoft.AspNetCore.Authentication.AuthenticationProperties>> GetLoginProperties(string provider, string redirectUrl, string userId = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> AddLogin(string userId, string loginProvider, string providerKey, string displayName)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> RemoveLogin(string userId, string provider, string providerKey)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<UserAuthenticationInfoModel>> GetAuthenticationDetail(string userName)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IdentityUserResponseModel>> GetUser(string key)
        {
            var response = await HttpClientHelper.GetAsync<IdentityUserResponseModel>(_timeLoggerOptions.IdentityServerApiUrl, "api/Account/Profile", SecurityOptions.AccessToken);
            if (!response.Success)
                return new ResponseModel<IdentityUserResponseModel> { Success = false, Message = "User not exists." };

            var user = await _userService.FirstOrDefaultAsync(x => x.Id.Equals(key) || x.Email.Equals(key));
            if (user == null)
                user = await _userService.Add(new Business.Model.UserModel
                {
                    Id = response.Data.Id,
                    FirstName = response.Data.FirstName,
                    LastName = response.Data.LastName,
                    Email = response.Data.Email,
                    Picture = response.Data.Picture,
                    CompanyId = response.Data.Company?.Id,
                    //StatusId = response.Data.Status?.Id
                });

            if (user == null)
                return new ResponseModel<IdentityUserResponseModel> { Success = false, Message = "User not exists." };

            return response;
        }

        public async Task<ResponseModel<IdentityUserResponseModel>> GetExternalUser(string loginProvider, string providerKey)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers(Expression<Func<IdentityUser, bool>> where)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers(Expression<Func<IdentityUser, bool>> where = null, Func<IQueryable<IdentityUser>, IOrderedQueryable<IdentityUser>> orderBy = null, params Expression<Func<IdentityUser, IdentityUser>>[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> BlockUser(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> AddUserClaim(string userId, string claimType, string claimValue)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IList<Claim>>> GetUserClaim(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> RemoveUserClaim(string userId, string claimType, string claimValue)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> CreateRole(string role)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> UpdateRole(string id, string role)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IdentityRoleModel>> GetRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<List<IdentityRoleModel>>> GetRoles()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> RemoveRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> AddUserRole(string userId, IEnumerable<string> roles)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IList<string>>> GetUserRoles(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> RemoveUserRole(string userId, string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> RemoveUserRoles(string userId, IEnumerable<string> roles)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<string>> GeneratePasswordResetToken(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> ValidatePasswordResetToken(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IdentityResult>> ResetPassword(string email, string code, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IdentityResult>> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> SetPassword(string userId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> GetPasswordFailuresSinceLastSuccess(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> GenerateChangeEmailToken(string userId, string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> ChangeEmail(string userId, string email, string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> GenerateChangePhoneNumberToken(string userId, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> ValidateChangePhoneNumberToken(string userId, string phoneNumber, string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> ChangePhoneNumber(string userId, string phoneNumber, string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> RemovePhoneNumber(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<AuthenticatorModel>> GetSharedKeyAndQrCodeUri(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IEnumerable<string>>> GenerateTwoFactorRecoveryCodes(string userId, int numberOfCodes = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> SendTwoFactorToken(string userName, string provider)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<string>> GenerateTwoFactorToken(string userName, string provider)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> VerifyTwoFactorToken(string userName, string provider, string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IEnumerable<string>>> EnableTwoFactorAuthentication(string userId, string tokenProvider, string code)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> ResetAuthenticator(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> DisableTwoFactorAuthentication(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel> Logout()
        {
            throw new NotImplementedException();
        }
    }
}