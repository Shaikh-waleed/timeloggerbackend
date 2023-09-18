using AutoMapper;
using IdentityModel;
using TimeLogger.Business.IService;
using TimeLogger.Business.Model;
using TimeLogger.Component.Interfaces.Communication;
using TimeLogger.Component.Interfaces.Security;
using TimeLogger.Component.Models.Security;
using TimeLogger.Component.Security.Entities;
using TimeLogger.Data.Entity;
using TimeLogger.Infrastructure.Models;
using TimeLogger.Infrastructure.Models.Configuration;
using TimeLogger.Infrastructure.Utility;
using TimeLogger.Infrastructure.Utility.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace TimeLogger.Component.Security.Service
{
    public class SecurityAspnetIdentity : ISecurityService
    {
        private readonly SecurityOptions _securityOptions;
        private readonly TimeLoggerOptions _timeLoggerOptions;

        protected readonly UserManager<UserIdentity> _userManager;
        protected readonly RoleManager<RoleIdentity> _roleManager;
        protected readonly SignInManager<UserIdentity> _signInManager;

        private readonly IStatusService _statusService;
        private readonly INotificationTemplateService _notificationTemplateService;
        private readonly ICommunicationService _communicationService;

        private readonly IMapper _mapper;
        protected readonly ISecurityDbContext _dbContext;

        private readonly UrlEncoder _urlEncoder;

        private string clientId = string.Empty;
        private string clientSecret = string.Empty;
        private string authenticatorUriFormat = string.Empty;
        private int numberOfRecoveryCodes;
        private string scopes = string.Empty;
        private string apiUrl = string.Empty;

        public SecurityAspnetIdentity(
            IOptionsSnapshot<SecurityOptions> securityOptions,
            IOptionsSnapshot<TimeLoggerOptions> timeLoggerOptions,

            UserManager<UserIdentity> userManager,
            RoleManager<RoleIdentity> roleManager,
            SignInManager<UserIdentity> signInManager,

            IStatusService statusService,
            INotificationTemplateService notificationTemplateService,
            ICommunicationService communicationService,

            IMapper mapper,
            ISecurityDbContext dbContext,

            UrlEncoder urlEncoder)
        {
            _securityOptions = securityOptions.Value;
            _timeLoggerOptions = timeLoggerOptions.Value;

            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _statusService = statusService;
            _notificationTemplateService = notificationTemplateService;
            _communicationService = communicationService;

            _mapper = mapper;
            _dbContext = dbContext;

            _urlEncoder = urlEncoder;
            clientId = _securityOptions.ClientId;
            clientSecret = _securityOptions.ClientSecret;
            authenticatorUriFormat = _securityOptions.AuthenticatorUriFormat;
            numberOfRecoveryCodes = this._securityOptions.NumberOfRecoveryCodes;
            scopes = _securityOptions.Scopes;
            apiUrl = _timeLoggerOptions.ApiUrl;
        }

        public async Task<ResponseModel<IdentityResult>> CreateUser(string firstName, string lastName, string userName, string email, string password)
        {
            var preactiveStats = await _statusService.FirstOrDefaultAsync(s => s.Name.Equals(UserStatusType.Preactive.ToString()));
            if (preactiveStats == null)
                throw new Exception($"{UserStatusType.Preactive.ToString()} is not found in the system.");

            var user = new UserIdentity
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                EmailConfirmed = !_securityOptions.RequireConfirmedAccount,
                TwoFactorTypeId = TwoFactorTypes.None,
                StatusId = preactiveStats.Id
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var applicationUser = new ApplicationUser
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
                await _dbContext.Users.AddAsync(applicationUser);

                await AddPreviousPassword(user, password);
                await _userManager.AddToRoleAsync(user, UserRoles.User.ToString());

                if (!string.IsNullOrWhiteSpace(firstName))
                    await AddUserClaim(user.Id, JwtClaimTypes.GivenName.ToString(), firstName);
                if (!string.IsNullOrWhiteSpace(lastName))
                    await AddUserClaim(user.Id, JwtClaimTypes.FamilyName.ToString(), lastName);
                await AddUserClaim(user.Id, JwtClaimTypes.Name, user.UserName);
                await AddUserClaim(user.Id, JwtClaimTypes.Email, user.Email);
                await AddUserClaim(user.Id, JwtClaimTypes.Role, UserRoles.User.ToString());

                if (_securityOptions.RequireConfirmedAccount)
                {
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var link = _timeLoggerOptions.WebUrl + "Account/ConfirmEmail?userId=" + user.Id + "&code=" + HttpUtility.UrlEncode(emailConfirmationToken);
                    var template = await _notificationTemplateService.GetNotificationTemplate(NotificationTemplates.EmailUserRegisteration, NotificationTypes.Email);
                    var name = !string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(user.LastName)
                                    ? $"{user.FirstName} {user.LastName}"
                                    : !string.IsNullOrWhiteSpace(user.FirstName)
                                            ? user.FirstName
                                            : !string.IsNullOrWhiteSpace(user.LastName)
                                                ? user.LastName
                                                : !string.IsNullOrWhiteSpace(userName)
                                                    ? userName
                                                    : email;

                    var emailMessage = template.MessageBody.Replace("#Name", name)
                                                           .Replace("#Link", $"{link}");

                    var sent = await _communicationService.SendEmail(template.Subject, emailMessage, user.Email);
                    if (sent)
                        return new ResponseModel<IdentityResult> { Success = true, Message = "Account created successfully. A confirmation link has been sent to your specified email , click the link to confirm your email and proceed to login." };

                    await _userManager.DeleteAsync(user);
                    _dbContext.Users.Remove(applicationUser);
                    return new ResponseModel<IdentityResult> { Success = false, Message = "Confirmation link cannot be sent, plz try again latter" };
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new ResponseModel<IdentityResult> { Success = true, Message = "Registeration successfull." };

            }
            var errorMessaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new ResponseModel<IdentityResult> { Success = false, Message = errorMessaeg, Data = result };
        }

        public async Task<LoginResponse> CreateExternalUser(string firstName, string lastName, string email, string picture, string loginProvider, string providerKey, string providerDisplayName)
        {
            var preactiveStats = await _statusService.FirstOrDefaultAsync(s => s.Name.Equals(UserStatusType.Preactive.ToString()));
            if (preactiveStats == null)
                throw new Exception($"{UserStatusType.Preactive.ToString()} is not found in the system.");

            var user = new UserIdentity
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = email,
                Email = email,
                Picture = picture,
                TwoFactorTypeId = TwoFactorTypes.None,
                StatusId = preactiveStats.Id
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var applicationUser = new ApplicationUser
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Picture = picture
                };
                await _dbContext.Users.AddAsync(applicationUser);

                await _userManager.AddToRoleAsync(user, UserRoles.User.ToString());

                if (!string.IsNullOrWhiteSpace(firstName))
                    await AddUserClaim(user.Id, JwtClaimTypes.GivenName.ToString(), firstName);
                if (!string.IsNullOrWhiteSpace(lastName))
                    await AddUserClaim(user.Id, JwtClaimTypes.FamilyName.ToString(), lastName);
                if (!string.IsNullOrWhiteSpace(picture))
                    await AddUserClaim(user.Id, JwtClaimTypes.Picture.ToString(), picture);
                await AddUserClaim(user.Id, JwtClaimTypes.Name, user.UserName);
                await AddUserClaim(user.Id, JwtClaimTypes.Email, user.Email);
                await AddUserClaim(user.Id, JwtClaimTypes.Role, UserRoles.User.ToString());

                var loginInfo = await AddLogin(user.Id, loginProvider, providerKey, providerDisplayName);
                if (!loginInfo.Success)
                {
                    result = await _userManager.DeleteAsync(user);
                    _dbContext.Users.Remove(applicationUser);
                    return new LoginResponse { Status = LoginStatus.Failed, Message = loginInfo.Message };
                }
                var login = await ExternalLogin(loginProvider, providerKey);
                return login;
            }
            var errorMessaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new LoginResponse { Status = LoginStatus.Failed, Message = errorMessaeg };
        }

        public async Task<ResponseModel<string>> GenerateEmailVerificationToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ResponseModel<string> { Success = false, Message = "No user exists with the specified email address." };

            var varficationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (string.IsNullOrWhiteSpace(varficationCode))
                return new ResponseModel<string> { Success = false, Message = "Email varification could not be generated." };

            return new ResponseModel<string> { Success = true, Data = varficationCode };
        }

        public async Task<ResponseModel> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "No user exists with the specified email address." };

            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (emailConfirmed)
                return ResponseModel.Failed(message: "Email has already been confirmed.");

            var response = await _userManager.ConfirmEmailAsync(user, code);
            if (response.Succeeded)
            {
                var statusResult = await ChangeUserStatus(userId, UserStatusType.Active.ToString());
                if (!statusResult.Success)
                    return statusResult;

                return new ResponseModel { Success = true, Message = "Email confirmed successfully." };
            }

            var message = response.Errors.FirstOrDefault() != null
                ? response.Errors.FirstOrDefault().Description
                : "Email confirmation failed.";
            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel> ChangeUserStatus(string userId, string status)
        {
            status = status == UserStatusType.Preactive.ToString()
                        ? UserStatus.Preactive.ToString()
                        : status == UserStatusType.Active.ToString()
                            ? UserStatus.Active.ToString()
                            : status == UserStatusType.Inactive.ToString()
                                ? UserStatus.Inactive.ToString()
                                : status == UserStatusType.Cancel.ToString()
                                    ? UserStatus.Canceled.ToString()
                                    : status == UserStatusType.Freez.ToString()
                                        ? UserStatus.Frozen.ToString()
                                        : status == UserStatusType.Block.ToString()
                                            ? UserStatus.Blocked.ToString()
                                            : string.Empty;

            var stats = await _statusService.FirstOrDefaultAsync(s => s.Name.Equals(status));
            if (stats == null)
                throw new Exception($"{nameof(status)} is not found in the system.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not found with specified Id." };

            if (status.Equals(UserStatusType.Block.ToString()))
            {
                var lockoutResult = await _userManager.SetLockoutEnabledAsync(user, true);
                if (!lockoutResult.Succeeded)
                {
                    var message = lockoutResult.Errors.FirstOrDefault() != null
                                       ? lockoutResult.Errors.FirstOrDefault().Description
                                       : "User could not be block.";
                    return new ResponseModel { Success = false, Message = message };
                }
            }

            user.StatusId = stats.Id;
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                var message = userUpdateResult.Errors.FirstOrDefault() != null
                                   ? userUpdateResult.Errors.FirstOrDefault().Description
                                   : "Faild to update user detail.";
                return new ResponseModel { Success = false, Message = message };
            }
            return new ResponseModel { Success = true, Message = $"User has been successfully {status.ToLower()}." };
        }

        public async Task<ResponseModel<IdentityUserModel>> UpdateUser(UpdateUserModel model, bool updateRelated)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return new ResponseModel<IdentityUserModel> { Success = false, Message = "No user exists." };

            var applicationUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
            if (applicationUser == null)
                return new ResponseModel<IdentityUserModel> { Success = false, Message = "No user exists." };

            if (!string.IsNullOrWhiteSpace(model.FirstName) && !user.FirstName.Equals(model.FirstName))
            {
                await RemoveUserClaim(user.Id, JwtClaimTypes.GivenName.ToString(), user.FirstName);
                user.FirstName = model.FirstName;
                applicationUser.FirstName = model.FirstName;
                await AddUserClaim(user.Id, JwtClaimTypes.GivenName.ToString(), model.FirstName);
            }
            if (!string.IsNullOrWhiteSpace(model.LastName) && !user.LastName.Equals(model.LastName))
            {
                await RemoveUserClaim(user.Id, JwtClaimTypes.GivenName.ToString(), user.FirstName);
                user.LastName = model.LastName;
                applicationUser.LastName = model.LastName;
                await AddUserClaim(user.Id, JwtClaimTypes.FamilyName.ToString(), model.LastName);
            }
            if (!string.IsNullOrWhiteSpace(model.Picture) && !user.LastName.Equals(model.Picture))
            {
                await RemoveUserClaim(user.Id, JwtClaimTypes.Picture.ToString(), user.Picture);
                user.Picture = model.Picture;
                applicationUser.Picture = model.Picture;
                await AddUserClaim(user.Id, JwtClaimTypes.Picture.ToString(), model.Picture);
            }
            if (updateRelated)
            {
                if (!user.CompanyId.Equals(model.CompanyId))
                    if (model.CompanyId.HasValue && model.CompanyId <= 0)
                        return new ResponseModel<IdentityUserModel> { Success = false, Message = "Invalid company id." };
                    else
                    {
                        user.CompanyId = model.CompanyId;
                        applicationUser.CompanyId = model.CompanyId;
                    }
            }

            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                var message = userUpdateResult.Errors.FirstOrDefault() != null
                                   ? userUpdateResult.Errors.FirstOrDefault().Description
                                   : "Faild to update user detail.";
                return new ResponseModel<IdentityUserModel> { Success = false, Message = message };
            }
            _dbContext.Users.Update(applicationUser);

            return new ResponseModel<IdentityUserModel> { Success = true, Message = "User info has been successfully updated.", Data = _mapper.Map<IdentityUserModel>(user) };
        }

        private async Task<string> Token(UserIdentity user)
        {
            List<Claim> claims = new List<Claim>();
            claims.AddRange(new List<Claim>
            {
                new Claim(JwtClaimTypes.Id, user.Id),
                new Claim(ClaimTypes.Sid, user.Id), // For old mvc applicaton to validate AntiForgeryToken in views.
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
            });

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                claims.Add(new Claim(JwtClaimTypes.GivenName.ToString(), user.FirstName));
            if (!string.IsNullOrWhiteSpace(user.LastName))
                claims.Add(new Claim(JwtClaimTypes.FamilyName.ToString(), user.LastName));
            if (!string.IsNullOrWhiteSpace(user.Picture))
                claims.Add(new Claim(JwtClaimTypes.Picture.ToString(), user.Picture));

            var roles = (await _userManager.GetRolesAsync(user));

            foreach (var role in roles)
                claims.Add(new Claim(JwtClaimTypes.Role, role.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Core.Secret@TimeLogger"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                            issuer: apiUrl,
                            audience: apiUrl,
                            claims: claims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: credentials);
            var accessToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }

        public async Task<LoginResponse<UserAuthenticationInfoModel>> Login(string userName, string password, bool persistCookie = false, bool lockoutOnFailure = false)
        {
            var user = await _userManager.Users
                                         .Include(x => x.TwoFactorType)
                                         .Include(x => x.Logins)
                                         .FirstOrDefaultAsync(x => x.UserName.Equals(userName) || x.Email.Equals(userName));
            if (user == null)
                return new LoginResponse<UserAuthenticationInfoModel> { Status = LoginStatus.InvalidCredential, Message = "User not exists." };

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return new LoginResponse<UserAuthenticationInfoModel> { Status = LoginStatus.Failed, Message = "Invalid user name or password." };
            }

            if (!user.EmailConfirmed)
                return new LoginResponse<UserAuthenticationInfoModel> { Status = LoginStatus.Failed, Message = "Email has not yet been confirmed , please confirm your email and login again." };

            var result = await _signInManager.PasswordSignInAsync(userName, password, persistCookie, lockoutOnFailure: lockoutOnFailure);
            if (result.Succeeded)
            {
                var accessToken = await Token(user);
                return new LoginResponse<UserAuthenticationInfoModel> { Status = LoginStatus.Succeded, Data = new UserAuthenticationInfoModel { AccessToken = accessToken } };
            }
            else if (result.RequiresTwoFactor)
            {
                var otherLoginProvders = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                                                          .Where(x => user.Logins.All(ul => x.Name != ul.LoginProvider))
                                                          .ToList();

                var authenticationInfoModel = _mapper.Map<UserAuthenticationInfoModel>(user);
                authenticationInfoModel.RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);
                authenticationInfoModel.OtherLogins = _mapper.Map<List<AuthenticationSchemeModel>>(otherLoginProvders);

                var twoFactorTokenResult = await SendTwoFactorToken(user.UserName, user.TwoFactorType.Name);
                if (!twoFactorTokenResult.Success)
                    return new LoginResponse<UserAuthenticationInfoModel> { Status = LoginStatus.Failed, Message = twoFactorTokenResult.Message };

                return new LoginResponse<UserAuthenticationInfoModel> { Status = LoginStatus.RequiresTwoFactor, Message = "Requires two factor varification.", Data = authenticationInfoModel };
            }
            else if (result.IsLockedOut)
            {
                return new LoginResponse<UserAuthenticationInfoModel> { Status = LoginStatus.AccountLocked, Message = "Your account has been Locked out." };
            }
            else
            {
                return new LoginResponse<UserAuthenticationInfoModel> { Status = LoginStatus.Failed, Message = "Invalid login attempt." };
            }
        }

        public async Task<LoginResponse> ExternalLogin(string loginProvider, string providerKey, bool isPersistent = false, bool bypassTwoFactor = false)
        {
            var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
            if (user == null)
                return new LoginResponse { Status = LoginStatus.Failed, Message = "Invalid user name or password." };

            var result = await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent, bypassTwoFactor);
            if (result.Succeeded)
            {
                var accessToken = await Token(user);
                return new LoginResponse { Status = LoginStatus.Succeded, Data = new UserAuthenticationInfoModel { AccessToken = accessToken } };
            }
            else if (result.RequiresTwoFactor)
            {
                var authenticationResult = await GetAuthenticationDetail(user.UserName);
                if (authenticationResult.Success)
                {
                    var twoFactorTokenResult = await SendTwoFactorToken(user.UserName, authenticationResult.Data.TwoFactorType.Name);
                    if (!twoFactorTokenResult.Success)
                        return new LoginResponse { Status = LoginStatus.Failed, Message = twoFactorTokenResult.Message };

                    return new LoginResponse { Status = LoginStatus.RequiresTwoFactor, Message = "Requires two factor varification.", Data = authenticationResult.Data };
                }
                return new LoginResponse { Status = LoginStatus.Failed, Message = authenticationResult.Message };
            }
            else
            {
                return new LoginResponse { Status = LoginStatus.Failed, Message = result.IsLockedOut ? "Locked Out" : "Invalid login attempt." };
            }
        }

        public async Task<LoginResponse> TwoFactorLogin(string provider, string code, bool persistCookie = false, bool rememberMachine = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return new LoginResponse { Status = LoginStatus.Failed, Message = "Invalid email address, not such user exists." };

            var result = await _signInManager.TwoFactorSignInAsync(provider, code, persistCookie, rememberMachine);
            if (result.Succeeded)
            {
                var accessToken = await Token(user);
                return new LoginResponse { Status = LoginStatus.Succeded, Data = new UserAuthenticationInfoModel { AccessToken = accessToken } };
            }
            return result.RequiresTwoFactor
                ? new LoginResponse { Status = LoginStatus.RequiresTwoFactor, Message = "Requires two factor varification." }
                : new LoginResponse { Status = LoginStatus.Failed, Message = result.IsLockedOut ? "Locked Out" : "Invalid authenticator code." };
        }

        public async Task<LoginResponse> RecoveryCodeLogin(string code)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return new LoginResponse { Status = LoginStatus.Failed, Message = "User not exist." };

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(code);
            if (result.Succeeded)
            {
                var accessToken = await Token(user);
                return new LoginResponse { Status = LoginStatus.Succeded, Data = new UserAuthenticationInfoModel { AccessToken = accessToken } };
            }
            return result.RequiresTwoFactor
                ? new LoginResponse { Status = LoginStatus.RequiresTwoFactor, Message = "Requires two factor varification." }
                : new LoginResponse { Status = LoginStatus.Failed, Message = result.IsLockedOut ? "Locked Out" : "Invalid recovery code." };
        }

        public async Task<ResponseModel<List<AuthenticationSchemeModel>>> GetLoginProviders()
        {
            var externalAuthenticationProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
            if (externalAuthenticationProviders == null)
                return new ResponseModel<List<AuthenticationSchemeModel>> { Success = false, Message = "No login provider is available." };

            var authenticationSchemes = externalAuthenticationProviders.ToList();
            var authenticationSchemeModels = _mapper.Map<List<AuthenticationSchemeModel>>(authenticationSchemes);
            return new ResponseModel<List<AuthenticationSchemeModel>> { Success = true, Data = authenticationSchemeModels };
        }

        public async Task<ResponseModel<List<UserLoginInfo>>> GetLogins(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel<List<UserLoginInfo>> { Success = false, Message = "User not exists." };

            var userLogins = await _userManager.GetLoginsAsync(user);
            return new ResponseModel<List<UserLoginInfo>> { Success = true, Data = userLogins.ToList() };
        }

        public async Task<ResponseModel<Microsoft.AspNetCore.Authentication.AuthenticationProperties>> GetLoginProperties(string provider, string redirectUrl, string userId = null)
        {
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new ResponseModel<Microsoft.AspNetCore.Authentication.AuthenticationProperties> { Success = false, Message = "User not exists." };
            }
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
            if (properties == null)
                return new ResponseModel<Microsoft.AspNetCore.Authentication.AuthenticationProperties> { Success = false, Message = "External authentication not found in the system." };

            return new ResponseModel<Microsoft.AspNetCore.Authentication.AuthenticationProperties> { Success = true, Data = properties };
        }

        public async Task<ResponseModel> AddLogin(string userId, string loginProvider, string providerKey, string displayName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var loginInfo = new ExternalLoginInfo(null, loginProvider, providerKey, displayName);

            var result = await _userManager.AddLoginAsync(user, loginInfo);
            if (!result.Succeeded)
                return new ResponseModel { Success = false, Message = result.Errors.FirstOrDefault() != null ? result.Errors.FirstOrDefault().Description : "The external login could not be added, please try again latter." };

            return new ResponseModel { Success = true, Message = "The external login is added." };
        }

        public async Task<ResponseModel> RemoveLogin(string userId, string provider, string providerKey)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.RemoveLoginAsync(user, provider, providerKey);
            if (!result.Succeeded)
                return new ResponseModel { Success = false, Message = result.Errors.FirstOrDefault() != null ? result.Errors.FirstOrDefault().Description : "The external login could not be removed, please try again latter." };

            return new ResponseModel { Success = true, Message = "The external login is removed." };
        }

        public async Task<ResponseModel<UserAuthenticationInfoModel>> GetAuthenticationDetail(string userName)
        {
            var user = await _userManager.Users
                                         .Include(x => x.TwoFactorType)
                                         .Include(x => x.Logins)
                                         .FirstOrDefaultAsync(x => x.UserName.Equals(userName) || x.Email.Equals(userName));
            if (user == null)
                return new ResponseModel<UserAuthenticationInfoModel> { Success = false, Message = "User not exists." };


            var otherLoginProvders = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                                                          .Where(x => user.Logins.All(ul => x.Name != ul.LoginProvider))
                                                          .ToList();

            var authenticationInfoModel = _mapper.Map<UserAuthenticationInfoModel>(user);
            authenticationInfoModel.OtherLogins = _mapper.Map<List<AuthenticationSchemeModel>>(otherLoginProvders);
            authenticationInfoModel.RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);
            //authenticationInfoModel.BrowserRemembered = await _userManager.TwoFactorBrowserRememberedAsync(user) Todo

            return new ResponseModel<UserAuthenticationInfoModel> { Success = true, Data = authenticationInfoModel };
        }

        public async Task<ResponseModel<IdentityUserResponseModel>> GetUser(string key)
        {
            var user = await _userManager.Users
                                         .Include(x => x.TwoFactorType)
                                         .Include(x => x.Company)
                                         .Include(x => x.Status)
                                         .Include(x => x.Addresses)
                                         .Include(x => x.Logins)
                                         .Include(x => x.UserRoles)
                                         .ThenInclude(x => x.Role)
                                         .FirstOrDefaultAsync(x => x.Id.Equals(key) || x.UserName.Equals(key) || x.Email.Equals(key));
            if (user == null)
                return new ResponseModel<IdentityUserResponseModel> { Success = false, Message = "User not exists." };

            var otherLoginProvders = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                                                          .Where(x => user.Logins.All(ul => x.Name != ul.LoginProvider))
                                                          .ToList();

            var userResponseModel = _mapper.Map<IdentityUserResponseModel>(user);
            userResponseModel.RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);
            userResponseModel.OtherLogins = _mapper.Map<List<AuthenticationSchemeModel>>(otherLoginProvders);

            return new ResponseModel<IdentityUserResponseModel> { Success = true, Data = userResponseModel };
        }

        public async Task<ResponseModel<IdentityUserResponseModel>> GetExternalUser(string loginProvider, string providerKey)
        {
            var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
            if (user == null)
                return new ResponseModel<IdentityUserResponseModel> { Success = false, Message = "User not exists." };

            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            var userResponseModel = _mapper.Map<IdentityUserResponseModel>(user);
            userResponseModel.Roles = roles;

            return new ResponseModel<IdentityUserResponseModel> { Success = true, Data = userResponseModel };
        }

        public async Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers()
        {
            var users = await _userManager.Users
                                          .Include(x => x.TwoFactorType)
                                          .Include(x => x.Company)
                                          .Include(x => x.Status)
                                          .Include(x => x.Addresses)
                                          .Include(x => x.Logins)
                                          .Include(x => x.UserRoles)
                                          .ThenInclude(x => x.Role)
                                          .ToListAsync();
            if (users == null)
                return new ResponseModel<List<IdentityUserResponseModel>> { Success = false, Message = "No users exist." };

            var usersResponseModel = _mapper.Map<List<IdentityUserResponseModel>>(users);
            return new ResponseModel<List<IdentityUserResponseModel>> { Success = true, Data = usersResponseModel };
        }

        public async Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers(Expression<Func<IdentityUser, bool>> where)
        {
            var users = await _userManager.Users
                                          .Include(x => x.TwoFactorType)
                                          .Include(x => x.Company)
                                          .Include(x => x.Status)
                                          .Include(x => x.Addresses)
                                          .Include(x => x.Logins)
                                          .Include(x => x.UserRoles)
                                          .ThenInclude(x => x.Role)
                                          .Where(where)
                                          .ToListAsync();
            if (users == null)
                return new ResponseModel<List<IdentityUserResponseModel>> { Success = false, Message = "No users exist." };

            var usersResponseModel = _mapper.Map<List<IdentityUserResponseModel>>(users);
            return new ResponseModel<List<IdentityUserResponseModel>> { Success = true, Data = usersResponseModel };
        }

        public async Task<ResponseModel<List<IdentityUserResponseModel>>> GetUsers(Expression<Func<IdentityUser, bool>> where = null, Func<IQueryable<IdentityUser>, IOrderedQueryable<IdentityUser>> orderBy = null, params Expression<Func<IdentityUser, IdentityUser>>[] includeProperties)
        {
            IQueryable<IdentityUser> query = _userManager.Users
                                                         .Include(x => x.TwoFactorType)
                                                         .Include(x => x.Company)
                                                         .Include(x => x.Status)
                                                         .Include(x => x.Addresses)
                                                         .Include(x => x.Logins)
                                                         .Include(x => x.UserRoles)
                                                         .ThenInclude(x => x.Role);

            if (Check.NotNull(where))
                query = query.Where(where);
            if (Check.NotNull(includeProperties))
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            if (Check.NotNull(orderBy))
                query = orderBy(query);

            var users = await query.ToListAsync();
            if (users == null)
                return new ResponseModel<List<IdentityUserResponseModel>> { Success = false, Message = "No users exist." };

            var usersResponseModel = _mapper.Map<List<IdentityUserResponseModel>>(users);

            return new ResponseModel<List<IdentityUserResponseModel>> { Success = true, Data = usersResponseModel };
        }

        public async Task<ResponseModel> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not found with specified Id." };

            var lockoutResult = await _userManager.SetLockoutEnabledAsync(user, true);
            if (!lockoutResult.Succeeded)
            {
                var message = lockoutResult.Errors.FirstOrDefault() != null
                                   ? lockoutResult.Errors.FirstOrDefault().Description
                                   : "User could not be block.";
                return new ResponseModel { Success = false, Message = message };
            }
            return new ResponseModel { Success = false, Message = "User has been successfully blocked." };
        }

        public async Task<ResponseModel> AddUserClaim(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not found with specified Id." };

            var userClaims = await _userManager.GetClaimsAsync(user);
            if (userClaims.Any())
            {
                var givenClaim = userClaims.FirstOrDefault(x => x.Type.ToLower() == claimType.ToLower());
                if (givenClaim != null)
                    return new ResponseModel { Success = false, Message = "The specified claim already assigned to user, try different value." };
            }
            var result = await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim(claimType, claimValue));
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = "Claim added." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : "Failed to add claim.";

            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel<IList<System.Security.Claims.Claim>>> GetUserClaim(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel<IList<System.Security.Claims.Claim>> { Success = false, Message = "User not found with specified Id." };

            var userClaims = await _userManager.GetClaimsAsync(user);
            return new ResponseModel<IList<System.Security.Claims.Claim>> { Success = false, Data = userClaims };
        }

        public async Task<ResponseModel> RemoveUserClaim(string userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not found with specified Id." };

            var userClaims = await _userManager.GetClaimsAsync(user);
            if (userClaims.Any())
            {
                var givenClaim = userClaims.FirstOrDefault(x => x.Type.ToLower() == claimType.ToLower() && x.Value.ToLower() == claimValue.ToLower());
                if (givenClaim == null)
                    return new ResponseModel { Success = false, Message = "User doesn't have the specified claim." };
            }

            var result = await _userManager.RemoveClaimAsync(user, new System.Security.Claims.Claim(claimType, claimValue));
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = "Claim removed successfully." };

            return new ResponseModel
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() != null
                                            ? result.Errors.FirstOrDefault().Description
                                            : "Failed to remove claim."
            };
        }

        public async Task<ResponseModel> CreateRole(string role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (roleExist)
                return new ResponseModel { Success = false, Message = $"{role} role already exists." };

            var identityRole = new RoleIdentity
            {
                Name = role
            };
            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = $"{role} role successfully added." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to add {role} role.";

            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel> UpdateRole(string id, string role)
        {
            var existingRole = await _roleManager.FindByIdAsync(id);
            if (existingRole == null)
                return new ResponseModel { Success = false, Message = $"Role not found with specified Id." };

            existingRole.Name = role;
            var result = await _roleManager.UpdateAsync(existingRole);
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = $"Role successfully updated." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to update role.";

            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel<IdentityRoleModel>> GetRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return new ResponseModel<IdentityRoleModel> { Success = false, Message = $"{role} role not found." };

            var roleModel = _mapper.Map<IdentityRoleModel>(role);
            return new ResponseModel<IdentityRoleModel> { Success = true, Data = roleModel };
        }

        public async Task<ResponseModel<List<IdentityRoleModel>>> GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            var rolesModel = _mapper.Map<List<IdentityRoleModel>>(roles);
            return new ResponseModel<List<IdentityRoleModel>> { Success = true, Data = rolesModel };
        }

        public async Task<ResponseModel> RemoveRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return new ResponseModel { Success = false, Message = $"{role} role not found." };

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = $"{role} role removed successfully." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to remove {role} role.";

            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel> AddUserRole(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not found with specified Id." };

            string notFoundRoles = string.Empty;
            //var notExistingRoles = _roleManager.Roles.ToList().Select(role => { role.Name = role.Name.ToLower(); return role.Name; }).Where(roleName => !roles.Contains(roleName));
            var notExistingRoles = roles.ToList()
                                        .Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                        .Where(roleName => !_roleManager.Roles
                                                                        .ToList()
                                                                        .Select(role => { role.Name = role.Name.ToLower(); return role.Name; })
                                                                        .Contains(roleName));
            foreach (var roleName in notExistingRoles)
            {
                notFoundRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(notFoundRoles))
                return new ResponseModel
                {
                    Success = false,
                    Message = $"{notFoundRoles.Remove(notFoundRoles.LastIndexOf(','))} {(notExistingRoles.Count() > 1 ? "roles" : "role")} not found in the system."
                };

            var alreadyFoundUserRoles = string.Empty;
            var userRoles = await _userManager.GetRolesAsync(user);
            var alreadyExistingUserRoles = userRoles.Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                                    .Where(roleName => roles.Contains(roleName));
            foreach (var roleName in alreadyExistingUserRoles)
            {
                alreadyFoundUserRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(alreadyFoundUserRoles))
                return new ResponseModel
                {
                    Success = false,
                    Message = $"User is already in {alreadyFoundUserRoles.Remove(alreadyFoundUserRoles.LastIndexOf(','))} {(alreadyFoundUserRoles.Count() > 1 ? "roles" : "role")}."
                };

            var result = await _userManager.AddToRolesAsync(user, roles);
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = $"{(roles.Count() > 1 ? "Roles" : "Role")} successfully added for user." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to add {(roles.Count() > 1 ? "roles" : "role")} for user.";

            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel<IList<string>>> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel<IList<string>> { Success = false, Message = "User not found with specified Id." };

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles == null || !userRoles.Any())
                return new ResponseModel<IList<string>> { Success = false, Message = "User do not have any role." };

            return new ResponseModel<IList<string>> { Success = true, Data = userRoles };
        }

        public async Task<ResponseModel> RemoveUserRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not found with specified Id." };

            var role = await _roleManager.FindByIdAsync(roleName);
            if (role == null)
                return new ResponseModel { Success = false, Message = $"{roleName} role not found in the system." };

            var userInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!userInRole)
                return new ResponseModel { Success = false, Message = $"User do not have {roleName} role." };

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = $"{roleName} role removed successfully from user." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : $"Failed to remove {roleName} role form user.";

            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel> RemoveUserRoles(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not found with specified Id." };

            string notFoundRoles = string.Empty;
            //var notExistingRoles = _roleManager.Roles.ToList().Select(role => { role.Name = role.Name.ToLower(); return role.Name; }).Where(roleName => !roles.Contains(roleName));
            var notExistingRoles = roles
                                    .ToList()
                                    .Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                    .Where(roleName => !_roleManager.Roles
                                                                    .ToList()
                                                                    .Select(role => { role.Name = role.Name.ToLower(); return role.Name; })
                                                                     .Contains(roleName));
            foreach (var roleName in notExistingRoles)
            {
                notFoundRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(notFoundRoles))
                return new ResponseModel
                {
                    Success = false,
                    Message = $"{notFoundRoles.Remove(notFoundRoles.LastIndexOf(','))} {(notExistingRoles.Count() > 1 ? "roles" : "role")} not found in the system."
                };

            var notFoundUserRoles = string.Empty;
            var userRoles = await _userManager.GetRolesAsync(user);
            var notExistingUserRoles = roles
                                        .Select(roleName => { roleName = roleName.ToLower(); return roleName; })
                                        .Where(roleName => !userRoles.Select(roleNam => { roleNam = roleNam.ToLower(); return roleNam; })
                                                                     .Contains(roleName));
            foreach (var roleName in notExistingUserRoles)
            {
                notFoundUserRoles += roleName + ",";
            }
            if (!string.IsNullOrWhiteSpace(notFoundUserRoles))
                return new ResponseModel
                {
                    Success = false,
                    Message = $"User is not in {notFoundUserRoles.Remove(notFoundUserRoles.LastIndexOf(','))} {(notFoundUserRoles.Count() > 1 ? "roles" : "role")}."
                };

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = "Roles removed successfully from user." };

            var message = result.Errors.FirstOrDefault() != null
                ? result.Errors.FirstOrDefault().Description
                : "Failed to remove roles form user.";

            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                user = await _userManager.FindByNameAsync(email);

            if (user == null)
                return new ResponseModel { Success = false, Message = "No user exists with the specified email." };

            var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = _timeLoggerOptions.WebUrl + "Account/ResetPassword?code=" + HttpUtility.UrlEncode(resetCode);
            var template = await _notificationTemplateService.GetNotificationTemplate(NotificationTemplates.EmailForgotPassword, NotificationTypes.Email);
            var emailMessage = template.MessageBody.Replace("#Name", $"{user.FirstName} {user.LastName}")
                                                   .Replace("#Link", $"{link}");

            var sent = await _communicationService.SendEmail(template.Subject, emailMessage, user.Email);
            if (!sent)
                return ResponseModel.Failed("Confirmation link cannot be sent, plz try again latter");

            return new ResponseModel { Success = true, Message = "Your password reset code has been sent to your specified email address, follow the link to reset your password." };
        }

        public async Task<ResponseModel<string>> GeneratePasswordResetToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ResponseModel<string> { Success = false, Message = "No user exists with the specified email." };

            var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new ResponseModel<string> { Success = true, Data = resetCode };
        }

        public async Task<ResponseModel> ValidatePasswordResetToken(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (string.IsNullOrWhiteSpace(token))
                return new ResponseModel { Success = false, Message = "Please provide a valid token to validate." };

            var result = await _userManager.VerifyUserTokenAsync(user, "Default", "ResetPassword", token);
            return result
                    ? new ResponseModel { Success = true, Message = "Token is valid." }
                    : new ResponseModel { Success = false, Message = "Token is in-valid." };
        }

        public async Task<ResponseModel<IdentityResult>> ResetPassword(string email, string code, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ResponseModel<IdentityResult> { Success = false, Message = "Invalid user." };

            var previousPasswordValidation = await ValidatePreviousPassword(user, password);
            if (!previousPasswordValidation.Success)
                return previousPasswordValidation;

            var result = await _userManager.ResetPasswordAsync(user, code, password);
            if (result.Succeeded)
            {
                await AddPreviousPassword(user, password);
                return new ResponseModel<IdentityResult> { Success = true, Message = "Password was reset successfully." };
            }

            return new ResponseModel<IdentityResult>
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() != null
                            ? result.Errors.FirstOrDefault().Description
                            : "Password reset failed.",
                Data = result
            };
        }

        public async Task<ResponseModel<IdentityResult>> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            if (currentPassword == newPassword)
                return new ResponseModel<IdentityResult> { Success = false, Message = "New password must not be same as cureent password." };

            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new ResponseModel<IdentityResult> { Success = false, Message = "No user exists with the specified email/username." };
            }

            var previousPasswordValidation = await ValidatePreviousPassword(user, newPassword);
            if (!previousPasswordValidation.Success)
                return previousPasswordValidation;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await AddPreviousPassword(user, newPassword);
                return new ResponseModel<IdentityResult> { Success = true, Message = "Password changed successfully." };
            }

            string messaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new ResponseModel<IdentityResult> { Success = false, Message = messaeg ?? "Failed to change password.", Data = result };
        }

        public async Task<ResponseModel> SetPassword(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "No user exists." };

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
                return new ResponseModel { Success = false, Message = "You already have a password. You can only change your password." };

            var result = await _userManager.AddPasswordAsync(user, newPassword);
            if (result.Succeeded)
            {
                var statusResult = await ChangeUserStatus(user.Id, UserStatusType.Preactive.ToString());
                if (!statusResult.Success)
                    return statusResult;

                user.TwoFactorTypeId = TwoFactorTypes.None;
                var userUpdateResult = await _userManager.UpdateAsync(user);
                await AddPreviousPassword(user, newPassword);
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = _timeLoggerOptions.WebUrl + "Account/ConfirmEmail?userId=" + user.Id + "&code=" + HttpUtility.UrlEncode(emailConfirmationToken);
                var template = await _notificationTemplateService.GetNotificationTemplate(NotificationTemplates.EmailSetPassword, NotificationTypes.Email);
                var emailMessage = template.MessageBody.Replace("#Name", $"{user.FirstName} {user.LastName}")
                                                       .Replace("#Link", $"{link}");

                var sent = await _communicationService.SendEmail(template.Subject, emailMessage, user.Email);
                if (!sent)
                    return ResponseModel.Failed("Confirmation link cannot be sent, plz try again latter");

                return new ResponseModel { Success = true, Message = $"Password has been set successfully. But to confirm your email address, a confirmation link has been sent to {user.Email}, please verify your email." };
            }
            string messaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new ResponseModel { Success = false, Message = messaeg ?? "Failed to set password." };
        }

        public async Task<ResponseModel> GetPasswordFailuresSinceLastSuccess(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User doesn't exist with the specified email." };

            var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
            return new ResponseModel
            {
                Success = true,
                Message = "Total failed access count:" + failedAttempts.ToString()
            };
        }

        public async Task<ResponseModel> GenerateChangeEmailToken(string userId, string email)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, email);
            var link = _timeLoggerOptions.WebUrl + "Manage/ChangeEmailAddress?userId=" + user.Id + "&email=" + email + "&code=" + HttpUtility.UrlEncode(token);
            var template = await _notificationTemplateService.GetNotificationTemplate(NotificationTemplates.EmailChangePassword, NotificationTypes.Email);
            var emailMessage = template.MessageBody.Replace("#Name", $"{user.FirstName} {user.LastName}")
                                                   .Replace("#Link", $"{link}");

            var sent = await _communicationService.SendEmail(template.Subject, emailMessage, email);
            if (!sent)
                return ResponseModel.Failed("Confirmation link cannot be sent, plz try again latter");

            return new ResponseModel { Success = true, Message = $"A confirmation link has been sent to {email}, please verify your email to change it." };
        }

        public async Task<ResponseModel> ChangeEmail(string userId, string email, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = "Your email has been changed successfully." };

            var message = result.Errors.FirstOrDefault() != null
               ? result.Errors.FirstOrDefault().Description
               : "Faild to change email.";
            return new ResponseModel { Success = false, Message = message };
        }

        public async Task<ResponseModel> GenerateChangePhoneNumberToken(string userId, string phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            if (!await _communicationService.SendSms(code, user.PhoneNumber)) // Todo: Phone notification is not done yet.
                return new ResponseModel { Success = false, Message = "Sms could not be sent." };

            return new ResponseModel { Success = true, Message = $"Sms has been sent to {phoneNumber}." };
        }

        public async Task<ResponseModel> ValidateChangePhoneNumberToken(string userId, string phoneNumber, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.VerifyChangePhoneNumberTokenAsync(user, phoneNumber, code);
            if (!result)
                return new ResponseModel { Success = false, Message = "Code is not correct." };

            return new ResponseModel { Success = true, Message = "Phone number verified successfully." };
        }

        public async Task<ResponseModel> ChangePhoneNumber(string userId, string phoneNumber, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, code);
            if (result.Succeeded)
                return new ResponseModel { Success = true, Message = "Your phone number has been changed successfully." };

            string messaeg = result.Errors.Aggregate("", (current, error) => current + (error.Description + "\n")).TrimEnd('\n');
            return new ResponseModel { Success = false, Message = messaeg };
        }

        public async Task<ResponseModel> RemovePhoneNumber(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.SetPhoneNumberAsync(user, null);
            if (!result.Succeeded)
                return new ResponseModel { Success = false, Message = "Phone number could not be deleted." };

            return new ResponseModel { Success = true, Message = "Your phone number has been deleted successfully." };
        }

        public async Task<ResponseModel<AuthenticatorModel>> GetSharedKeyAndQrCodeUri(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel<AuthenticatorModel> { Success = false, Message = "User not exists." };

            var model = new AuthenticatorModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            if (string.IsNullOrWhiteSpace(model.SharedKey))
                return new ResponseModel<AuthenticatorModel> { Success = false, Message = "Shared key could not be generated." };

            return new ResponseModel<AuthenticatorModel> { Success = true, Data = model };
        }

        public async Task<ResponseModel<IEnumerable<string>>> GenerateTwoFactorRecoveryCodes(string userId, int numberOfCodes = 0)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel<IEnumerable<string>> { Success = false, Message = "User not exists." };

            if (!user.TwoFactorEnabled)
                return new ResponseModel<IEnumerable<string>> { Success = false, Message = "Cannot generate recovery codes for user, as they do not have two factor authentication enabled." };

            if (numberOfCodes <= 0)
                numberOfCodes = numberOfRecoveryCodes;

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, numberOfCodes);

            if (!recoveryCodes.Any())
                return new ResponseModel<IEnumerable<string>> { Success = false, Message = "Recovery codes could not be generated." };

            return new ResponseModel<IEnumerable<string>> { Success = true, Message = "Recovery codes generated successfully.", Data = recoveryCodes };
        }

        public async Task<ResponseModel> SendTwoFactorToken(string userName, string provider)
        {
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new ResponseModel { Success = false, Message = "User not exists." };
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, provider);

            if (user.TwoFactorEnabled && user.TwoFactorTypeId == TwoFactorTypes.Email)
            {
                var template = await _notificationTemplateService.GetNotificationTemplate(NotificationTemplates.EmailTwoFactorToken, NotificationTypes.Email);
                var emailMessage = template.MessageBody.Replace("#Name", $"{user.FirstName} {user.LastName}")
                                                       .Replace("#Token", $"{token}");

                var emailSent = await _communicationService.SendEmail(template.Subject, emailMessage, user.Email);
                if (!emailSent)
                    return ResponseModel.Failed("Code cannot be sent, plz try again latter");

                return new ResponseModel { Success = true, Message = $"A code has been sent to {user.Email}, please verify the code." };
            }
            else if (user.TwoFactorEnabled && user.TwoFactorTypeId == TwoFactorTypes.Phone)
            {
                var smsSent = await _communicationService.SendSms(token, user.PhoneNumber);
                if (!smsSent)
                    return ResponseModel.Failed("Code cannot be sent, plz try again latter");

                return new ResponseModel { Success = true, Message = $"A code has been sent to {user.PhoneNumber}, please verify the code." };
            }

            return new ResponseModel { Success = true, Message = $"Two factor authentication is not enable yet, please enable it first." };
        }

        public async Task<ResponseModel<string>> GenerateTwoFactorToken(string userName, string provider)
        {
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new ResponseModel<string> { Success = false, Message = "User not exists." };
            }

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, provider);
            return new ResponseModel<string> { Success = true, Data = token, Message = $"Two factor code has been created successfully." };
        }

        public async Task<ResponseModel> VerifyTwoFactorToken(string userName, string provider, string code)
        {
            var user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                    return new ResponseModel { Success = false, Message = "User not exists." };
            }

            var isEmailTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, provider, code);

            if (!isEmailTokenValid)
                return new ResponseModel { Success = false, Message = "Invalid token." };

            return new ResponseModel { Success = true, Message = "Token is valid." };
        }

        public async Task<ResponseModel<IEnumerable<string>>> EnableTwoFactorAuthentication(string userId, string tokenProvider, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel<IEnumerable<string>> { Success = false, Message = "User not exists." };

            var tokenVerificationResult = await VerifyTwoFactorToken(user.UserName, tokenProvider, code);
            if (!tokenVerificationResult.Success)
                return new ResponseModel<IEnumerable<string>> { Success = false, Message = tokenVerificationResult.Message };

            var twoFactorResult = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (!twoFactorResult.Succeeded)
            {
                var message = twoFactorResult.Errors.FirstOrDefault() != null
                                   ? twoFactorResult.Errors.FirstOrDefault().Description
                                   : "Faild to enable two factor authentication.";
                return new ResponseModel<IEnumerable<string>> { Success = false, Message = message };
            }

            var twoFactorType = await _dbContext.TwoFactorTypes.FirstOrDefaultAsync(t => t.Name.Equals(tokenProvider));
            if (twoFactorType == null)
                throw new Exception($"{nameof(twoFactorType)} is not found in the system.");

            user.TwoFactorTypeId = twoFactorType.Id;
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                var message = userUpdateResult.Errors.FirstOrDefault() != null
                                   ? userUpdateResult.Errors.FirstOrDefault().Description
                                   : "Faild to enable two factor authentication.";
                return new ResponseModel<IEnumerable<string>> { Success = false, Message = message };
            }

            var recoveryCodesResult = await GenerateTwoFactorRecoveryCodes(userId, numberOfRecoveryCodes);
            if (!recoveryCodesResult.Success)
                return new ResponseModel<IEnumerable<string>> { Success = false, Message = recoveryCodesResult.Message };

            return new ResponseModel<IEnumerable<string>> { Success = true, Message = "Two factor authentication enabled successfully.", Data = recoveryCodesResult.Data };
        }

        public async Task<ResponseModel> ResetAuthenticator(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var disableAuthenticatorResult = await DisableTwoFactorAuthentication(userId);
            if (!disableAuthenticatorResult.Success)
                return new ResponseModel { Success = false, Message = disableAuthenticatorResult.Message };

            var resetAuthenticatorResult = await _userManager.ResetAuthenticatorKeyAsync(user);
            if (!resetAuthenticatorResult.Succeeded)
            {
                var message = resetAuthenticatorResult.Errors.FirstOrDefault() != null
                                   ? resetAuthenticatorResult.Errors.FirstOrDefault().Description
                                   : "Faild to rest authenticator.";
                return new ResponseModel { Success = false, Message = message };
            }
            return new ResponseModel { Success = true, Message = "Authenticator has been reset successfully." };
        }

        public async Task<ResponseModel> DisableTwoFactorAuthentication(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseModel { Success = false, Message = "User not exists." };

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                var message = result.Errors.FirstOrDefault() != null
                                                       ? result.Errors.FirstOrDefault().Description
                                                       : "Faild to disable two factor authentication.";
                return new ResponseModel { Success = false, Message = message };
            }

            user.TwoFactorTypeId = TwoFactorTypes.None;
            var userUpdateResult = await _userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                var message = userUpdateResult.Errors.FirstOrDefault() != null
                                   ? userUpdateResult.Errors.FirstOrDefault().Description
                                   : "Faild to disable two factor authentication.";
                return new ResponseModel { Success = false, Message = message };
            }
            return new ResponseModel { Success = true, Message = "Two factor authentication disabled successfully." };
        }

        public async Task<ResponseModel> Logout()
        {
            await _signInManager.SignOutAsync();
            return new ResponseModel { Success = true, Message = "Signed out successfully." };
        }


        #region Private Methods



        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
                result.Append(unformattedKey.Substring(currentPosition));

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                authenticatorUriFormat,
                _urlEncoder.Encode("TimeLogger"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(UserIdentity user, AuthenticatorModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        private async Task AddPreviousPassword(UserIdentity user, string newPassword)
        {
            var previousPassword = new PreviousPassword
            {
                UserId = user.Id,
                PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword),
                CreateDate = DateTime.Now
            };
            if (!await _dbContext.PreviousPasswords.AnyAsync(x => x.UserId.Equals(user.Id) && x.PasswordHash.Equals(previousPassword.PasswordHash)))
            {
                await _dbContext.PreviousPasswords.AddAsync(previousPassword);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task<ResponseModel<IdentityResult>> ValidatePreviousPassword(UserIdentity user, string newPassword)
        {
            var previousPasswords = await _dbContext.PreviousPasswords
                                                                .Where(x => x.UserId.Equals(user.Id))
                                                                .OrderByDescending(x => x.CreateDate)
                                                                .Take(_securityOptions.PreviousPasswordValidationLimit)
                                                                .ToListAsync();
            if (!previousPasswords.Any())
                return new ResponseModel<IdentityResult> { Success = true };

            var isPreviousPassword = previousPasswords.Any(x => _userManager.PasswordHasher.VerifyHashedPassword(user, x.PasswordHash, newPassword) != PasswordVerificationResult.Failed);
            return isPreviousPassword
                    ? new ResponseModel<IdentityResult> { Success = false, Message = "You cannot use your previous passwords." }
                    : new ResponseModel<IdentityResult> { Success = true };
        }

        #endregion Private Methods
    }
}