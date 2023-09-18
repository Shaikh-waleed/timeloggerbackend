using TimeLogger.Component.Models.Security;
using TimeLogger.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Component.Interfaces.Security
{
    public interface ISecurityService
    {
        Task<ResponseModel<IdentityResult>> CreateUser(string firstName, string lastName, string userName, string email, string password);

        Task<LoginResponse> CreateExternalUser(string firstName, string lastName, string email, string picture, string loginProvider, string providerKey, string providerDisplayName);

        Task<ResponseModel<string>> GenerateEmailVerificationToken(string email);

        Task<ResponseModel> ConfirmEmail(string userId, string code);

        Task<ResponseModel> ChangeUserStatus(string userId, string status);

        Task<ResponseModel<IdentityUserModel>> UpdateUser(UpdateUserModel model, bool updateRelated);

        Task<LoginResponse<UserAuthenticationInfoModel>> Login(string userName, string password, bool persistCookie = false, bool lockoutOnFailure = false);

        Task<LoginResponse> ExternalLogin(string loginProvider, string providerKey, bool isPersistent = false, bool bypassTwoFactor = false);

        Task<LoginResponse> TwoFactorLogin(string provider, string code, bool persistCookie = false, bool rememberMachine = false);

        Task<LoginResponse> RecoveryCodeLogin(string code);

        Task<ResponseModel<List<AuthenticationSchemeModel>>> GetLoginProviders();

        Task<ResponseModel<List<Microsoft.AspNetCore.Identity.UserLoginInfo>>> GetLogins(string userId);

        Task<ResponseModel<Microsoft.AspNetCore.Authentication.AuthenticationProperties>> GetLoginProperties(string provider, string redirectUrl, string userId = null);

        Task<ResponseModel> AddLogin(string userId, string loginProvider, string providerKey, string displayName);

        Task<ResponseModel> RemoveLogin(string userId, string provider, string providerKey);

        Task<ResponseModel<UserAuthenticationInfoModel>> GetAuthenticationDetail(string userName);

        Task<ResponseModel<IdentityUserResponseModel>> GetUser(string key);

        Task<ResponseModel<IdentityUserResponseModel>> GetExternalUser(string loginProvider, string providerKey);

        Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers();

        Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers(Expression<Func<IdentityUser, bool>> where);

        Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers(Expression<Func<IdentityUser, bool>> where = null, Func<IQueryable<IdentityUser>, IOrderedQueryable<IdentityUser>> orderBy = null, params Expression<Func<IdentityUser, IdentityUser>>[] includeProperties);

        Task<ResponseModel> BlockUser(string userId);

        Task<ResponseModel> AddUserClaim(string userId, string claimType, string claimValue);

        Task<ResponseModel<IList<Claim>>> GetUserClaim(string userId);

        Task<ResponseModel> RemoveUserClaim(string userId, string claimType, string claimValue);

        Task<ResponseModel> CreateRole(string role);

        Task<ResponseModel> UpdateRole(string id, string role);

        Task<ResponseModel<IdentityRoleModel>> GetRole(string roleName);

        Task<ResponseModel<List<IdentityRoleModel>>> GetRoles();

        Task<ResponseModel> RemoveRole(string roleName);

        Task<ResponseModel> AddUserRole(string userId, IEnumerable<string> roles);

        Task<ResponseModel<IList<string>>> GetUserRoles(string userId);

        Task<ResponseModel> RemoveUserRole(string userId, string roleName);

        Task<ResponseModel> RemoveUserRoles(string userId, IEnumerable<string> roles);

        Task<ResponseModel> ForgotPassword(string email);

        Task<ResponseModel<string>> GeneratePasswordResetToken(string email);
        Task<ResponseModel> ValidatePasswordResetToken(string userId, string token);

        Task<ResponseModel<IdentityResult>> ResetPassword(string email, string code, string password);

        Task<ResponseModel<IdentityResult>> ChangePassword(string userName, string currentPassword, string newPassword);

        Task<ResponseModel> SetPassword(string userId, string newPassword);

        Task<ResponseModel> GetPasswordFailuresSinceLastSuccess(string email);

        Task<ResponseModel> GenerateChangeEmailToken(string userId, string email);

        Task<ResponseModel> ChangeEmail(string userId, string email, string code);

        Task<ResponseModel> GenerateChangePhoneNumberToken(string userId, string phoneNumber);

        Task<ResponseModel> ValidateChangePhoneNumberToken(string userId, string phoneNumber, string code);

        Task<ResponseModel> ChangePhoneNumber(string userId, string phoneNumber, string code);

        Task<ResponseModel> RemovePhoneNumber(string userId);

        Task<ResponseModel<AuthenticatorModel>> GetSharedKeyAndQrCodeUri(string userId);

        Task<ResponseModel<IEnumerable<string>>> GenerateTwoFactorRecoveryCodes(string userId, int numberOfCodes = 0);

        Task<ResponseModel> SendTwoFactorToken(string userName, string provider);

        Task<ResponseModel<string>> GenerateTwoFactorToken(string userName, string provider);

        Task<ResponseModel> VerifyTwoFactorToken(string userName, string provider, string code);

        Task<ResponseModel<IEnumerable<string>>> EnableTwoFactorAuthentication(string userId, string tokenProvider, string code);

        Task<ResponseModel> ResetAuthenticator(string userId);

        Task<ResponseModel> DisableTwoFactorAuthentication(string userId);

        Task<ResponseModel> Logout();
    }
}
