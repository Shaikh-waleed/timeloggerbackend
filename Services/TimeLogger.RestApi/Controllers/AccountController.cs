
using TimeLogger.Business.IService;
using TimeLogger.Business.Model;
using TimeLogger.Component.Interfaces.Security;
using TimeLogger.Component.Models.Security;
using TimeLogger.Infrastructure.Models;
using TimeLogger.Infrastructure.Models.Configuration;
using TimeLogger.Infrastructure.Utility.Enums;
using TimeLogger.Infrastructure.Utility.Extensions;
using TimeLogger.Infrastructure.Utility.Filters;
using TimeLogger.Infrastructure.Utility.Helpers;
using TimeLogger.RestApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace TimeLogger.RestApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SecurityOptions securityOptions;
        public AccountController(
                ISecurityService securityService,
                IHttpContextAccessor httpContextAccessor,
                IOptionsSnapshot<SecurityOptions> securityOptions
            )
        {
            _securityService = securityService;
            this.httpContextAccessor = httpContextAccessor;
            this.securityOptions = securityOptions.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult test()
        {
            return null;
        }

        //
        // POST: /Account/Create
        [AllowAnonymous]
        [HttpPost("Create")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel<IdentityResult>))]
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            try
            {
                var result = await _securityService.CreateUser(model.FirstName, model.LastName, model.UserName, model.Email, model.Password);
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Create(success: false, message: "There was an error processing your request, please try again."));

            }
        }

        ////
        //// POST: /Account/CreateMerchant
        //[AllowAnonymous]
        //[HttpPost("CreateMerchant")]
        //[ServiceFilter(typeof(ValidateModelState))]
        //[Produces("application/json", Type = typeof(ResponseModel))]
        //public async Task<IActionResult> RegisterMerchant(RegisterMerchantModel model)
        //{
        //    try
        //    {
        //        var result = await _securityService.CreateMerchant(model);
        //        if (!result.Success)
        //            return new BadRequestObjectResult(result);

        //        // Todo:
        //        //var userInfo = JToken.Parse(result.Data);
        //        //var userId = userInfo["data"]["userId"].ToString();
        //        //var emailConfirmationToken = userInfo["data"]["emailConfirmationToken"].ToString();

        //        return new ObjectResult(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BadRequestObjectResult(ResponseModel.Create(success: false, message: "There was an error processing your request, please try again."));

        //    }
        //}

        //
        // POST: /Account/CreateExternal
        [AllowAnonymous]
        [HttpPost("CreateExternal")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(LoginResponse))]
        public async Task<IActionResult> RegisterExternal(RegisterExternalModel model)
        {
            try
            {
                var result = await _securityService.CreateExternalUser(model.FirstName, model.LastName, model.Email, model.Picture, model.Provider, model.ProviderKey, model.ProviderDisplayName);
                if (result.Status == LoginStatus.Failed)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new LoginResponse { Message = "There was an error processing your request, please try again." });
            }
        }

        //
        // POST: /Account/ConfirmEmail
        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel model)
        {
            try
            {
                var result = await _securityService.ConfirmEmail(model.UserId, model.Code);
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(LoginResponse<UserAuthenticationInfoModel>))]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var result = await _securityService.Login(model.UserName, model.Password, model.RememberMe);
                if (result.Status == LoginStatus.Failed)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new LoginResponse { Message = "There was an error processing your request, please try again." });
            }
        }

        //
        // POST: /Account/ExternalLogin
        [AllowAnonymous]
        [HttpPost("ExternalLogin")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(LoginResponse))]
        public async Task<IActionResult> ExternalLogin(ExternalLoginModel model)
        {
            try
            {
                var result = await _securityService.ExternalLogin(model.LoginProvider, model.ProviderKey, model.IsPersistent, model.BypassTwoFactor);
                if (result.Status == LoginStatus.Failed)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new LoginResponse { Message = "There was an error processing your request, please try again." });
            }
        }

        //
        // POST: /Account/TwoFactorLogin
        [AllowAnonymous]
        [HttpPost("TwoFactorLogin")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(LoginResponse))]
        public async Task<IActionResult> TwoFactorLogin(VerifyCodeModel model)
        {
            try
            {
                var result = await _securityService.TwoFactorLogin(model.Provider, model.Code, model.RememberBrowser, model.RememberMachine);
                if (result.Status == LoginStatus.Failed)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new LoginResponse { Message = "There was an error processing your request, please try again." });
            }
        }

        //
        // POST: /Account/RecoveryCodeLogin
        [AllowAnonymous]
        [HttpPost("RecoveryCodeLogin")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(LoginResponse))]
        public async Task<IActionResult> RecoveryCodeLogin(VerifyCodeModel model)
        {
            try
            {
                var result = await _securityService.RecoveryCodeLogin(model.Code);
                if (result.Status == LoginStatus.Failed)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new LoginResponse { Message = "There was an error processing your request, please try again." });
            }
        }

        //
        // GET: /Account/LoginProviders
        [HttpGet("LoginProviders")]
        [Produces("application/json", Type = typeof(ResponseModel<List<AuthenticationSchemeModel>>))]
        public async Task<IActionResult> GetLoginProviders()
        {
            try
            {
                var result = await _securityService.GetLoginProviders();
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // GET: /Account/LoginProperties
        [AllowAnonymous]
        [HttpGet("LoginProperties")]
        [Produces("application/json", Type = typeof(ResponseModel<Microsoft.AspNetCore.Authentication.AuthenticationProperties>))]
        public async Task<IActionResult> GetLoginProperties(string provider, string redirectUrl, string userId = null)
        {
            try
            {
                var result = await _securityService.GetLoginProperties(provider, redirectUrl, userId);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/AddLogin
        [Authorize]
        [HttpPost("AddLogin")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> AddLogin(AddLoginModel model)
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.AddLogin(userId, model.Provider, model.ProviderKey, model.ProviderDisplayName);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/RemoveLogin
        [Authorize]
        [HttpPost("RemoveLogin")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> RemoveLogin(RemoveLoginModel model)
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.RemoveLogin(userId, model.Provider, model.ProviderKey);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // GET: /Account/Profile
        [Authorize]
        [HttpGet("Profile")]
        [Produces("application/json", Type = typeof(ResponseModel<IdentityUserResponseModel>))]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = User.GetId() ?? AuthenticationHelper.GetUserId(await HttpContext.GetTokenAsync("access_token"));
                var result = await _securityService.GetUser(userId);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // GET: /Account/ExternalUserProfile
        [Authorize]
        [HttpGet("ExternalUserProfile")]
        [Produces("application/json", Type = typeof(ResponseModel<IdentityUserResponseModel>))]
        public async Task<IActionResult> GetExternalUser(string loginProvider, string providerKey)
        {
            try
            {
                var result = await _securityService.GetExternalUser(loginProvider, providerKey);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }


        //
        // GET: /Account/Users
        [Authorize]
        [HttpGet("Users")]
        [Produces("application/json", Type = typeof(ResponseModel<List<IdentityUserResponseModel>>))]
        public async Task<IActionResult> Users()
        {
            try
            {
                var result = await _securityService.GetUsers();
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/Update
        [Authorize]
        [HttpPost("Update")]
        [Produces("application/json", Type = typeof(ResponseModel<IdentityUserModel>))]
        public async Task<IActionResult> Update(UpdateUserModel model)
        {
            try
            {
                model.Id = User.GetId();
                if (!ModelState.IsValid)
                {
                    var errorMessage = ModelState.Select(x => x.Value.Errors).FirstOrDefault()?.FirstOrDefault()?.ErrorMessage;
                    return new BadRequestObjectResult(ResponseModel.Failed(message: errorMessage));
                }

                var result = await _securityService.UpdateUser(model, false);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/AddClaim
        [Authorize]
        [HttpPost("AddClaim")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> AddClaim(string claimType, string claimValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(claimType))
                    return new BadRequestObjectResult(ResponseModel.Failed(message: "Invalid claim type"));

                if (string.IsNullOrWhiteSpace(claimType))
                    return new BadRequestObjectResult(ResponseModel.Failed(message: "Invalid claim value"));

                var userId = User.GetId();
                var response = await _securityService.AddUserClaim(userId, claimType, claimValue);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An error occured while processing your request, please try again"));
            }
        }

        //
        // GET: /Account/Claims
        [Authorize]
        [HttpGet("Claims")]
        [Produces("application/json", Type = typeof(ResponseModel<IList<Claim>>))]
        public async Task<IActionResult> GetClaim()
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.GetUserClaim(userId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An error occured while processing your request, please try again"));
            }
        }

        //
        // POST: /Account/RemoveClaim
        [Authorize]
        [HttpPost("RemoveClaim")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> RemoveClaim(string claimType, string claimValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(claimType))
                    return new BadRequestObjectResult(ResponseModel.Failed(message: "Invalid claim type"));

                if (string.IsNullOrWhiteSpace(claimType))
                    return new BadRequestObjectResult(ResponseModel.Failed(message: "Invalid claim value"));

                var userId = User.GetId();
                var response = await _securityService.RemoveUserClaim(userId, claimType, claimValue);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An error occured while processing your request, please try again"));
            }
        }

        //
        // POST: /Account/ForgotPassword
        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> ForgotPassword(ResetModel model)
        {
            try
            {
                var result = await _securityService.ForgotPassword(model.Email);
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An errro occured while processing your request, please try again."));
            }
        }

        //
        // POST: /Account/ResetPassword
        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel<IdentityResult>))]
        public async Task<IActionResult> ResetPassword(PasswordResetModel model)
        {
            try
            {
                var result = await _securityService.ResetPassword(model.Email, model.Code, model.NewPassword);
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An errro occured while processing your request, please try again."));
            }
        }

        //
        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost("ChangePassword")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel<IdentityResult>))]
        public async Task<IActionResult> ChangePassword(PasswordChangeModel model)
        {
            try
            {
                var userName = User.GetName();
                var result = await _securityService.ChangePassword(userName, model.OldPassword, model.NewPassword);
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An errro occured while processing your request. Please try again later."));
            }
        }

        //
        // POST: /Account/SetPassword
        [Authorize]
        [HttpPost("SetPassword")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> SetPassword(SetPasswordModel model)
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.SetPassword(userId, model.NewPassword);
                if (!response.Success)
                    return new BadRequestObjectResult(response);

                var updateUserInfoModel = new UpdateUserModel
                {
                    Id = userId,
                    FirstName = User.GetFistName(),
                    LastName = User.GetLastName()
                };
                var result = await _securityService.UpdateUser(updateUserInfoModel, false);
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An errro occured while processing your request. Please try again later."));
            }
        }

        //
        // POST: /Account/SendChangeEmailCode
        [Authorize]
        [HttpPost("SendChangeEmailCode")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> SendChangeEmailCode(ChangeEmailModel model)
        {
            try
            {
                var userId = User.GetId();
                var result = await _securityService.GenerateChangeEmailToken(userId, model.Email);
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An errro occured while processing your request. Please try again later."));
            }
        }

        //
        // POST: /Account/ChangeEmail
        [Authorize]
        [HttpPost("ChangeEmail")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> ChangeEmail(ConfirmEmailModel model)
        {
            try
            {
                var userId = User.GetId();
                var result = await _securityService.ChangeEmail(userId, model.Email, model.Code);
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "An errro occured while processing your request. Please try again later."));
            }
        }

        //
        // POST: /Account/SendChangePhoneCode
        [Authorize]
        [HttpPost("SendChangePhoneCode")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> SendChangePhoneCode(AddPhoneNumberModel model)
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.GenerateChangePhoneNumberToken(userId, model.PhoneNumber);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/VerifyPhoneNumber
        [Authorize]
        [HttpPost("VerifyPhoneNumber")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberModel model)
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.ValidateChangePhoneNumberToken(userId, model.PhoneNumber, model.Code);
                if (response.Success)
                    return await ChangePhoneNumber(model);

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/ChangePhoneNumber
        [Authorize]
        [HttpPut("ChangePhoneNumber")]
        [ServiceFilter(typeof(ValidateModelState))]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> ChangePhoneNumber(VerifyPhoneNumberModel model)
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.ChangePhoneNumber(userId, model.PhoneNumber, model.Code);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/RemovePhoneNumber
        [Authorize]
        [HttpDelete("RemovePhoneNumber")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.RemovePhoneNumber(userId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // GET: /Account/GetSharedKeyAndQrCodeUri
        [Authorize]
        [HttpGet("GetSharedKeyAndQrCodeUri")]
        [Produces("application/json", Type = typeof(ResponseModel<AuthenticatorModel>))]
        public async Task<ActionResult> GetSharedKeyAndQrCodeUri()
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.GetSharedKeyAndQrCodeUri(userId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/TwoFactorRecoveryCodes
        [Authorize]
        [HttpGet("TwoFactorRecoveryCodes")]
        [Produces("application/json", Type = typeof(ResponseModel<IEnumerable<string>>))]
        public async Task<ActionResult> GenerateTwoFactorRecoveryCodes(int numberOfCodes = 0)
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.GenerateTwoFactorRecoveryCodes(userId, numberOfCodes);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/EnableTwoFactorAuthentication
        [Authorize]
        [HttpPost("EnableTwoFactorAuthentication")]
        [Produces("application/json", Type = typeof(ResponseModel<IEnumerable<string>>))]
        public async Task<ActionResult> EnableTwoFactorAuthentication(VerifyCodeModel model)
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.EnableTwoFactorAuthentication(userId, model.Provider, model.Code);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/ResetAuthenticator
        [Authorize]
        [HttpGet("ResetAuthenticator")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> ResetAuthenticator()
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.ResetAuthenticator(userId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/DisableTwoFactorAuthentication
        [Authorize]
        [HttpGet("DisableTwoFactorAuthentication")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            try
            {
                var userId = User.GetId();
                var response = await _securityService.DisableTwoFactorAuthentication(userId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/SendTwoFactorCode
        [HttpPost("SendTwoFactorCode")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> SendTwoFactorCode(SendCodeModel model)
        {
            try
            {
                var userName = User.GetName();
                var response = await _securityService.SendTwoFactorToken(userName, model.SelectedProvider);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/VerifyTwoFactorCode
        [HttpPost("VerifyTwoFactorCode")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<ActionResult> VerifyTwoFactorCode(VerifyCodeModel model)
        {
            try
            {
                var response = await _securityService.VerifyTwoFactorToken(model.UserName, model.Provider, model.Code);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }

        //
        // POST: /Account/Logout
        [Authorize]
        [HttpPost("Logout")]
        [Produces("application/json", Type = typeof(ResponseModel))]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await _securityService.Logout();
                if (!result.Success)
                    return new BadRequestObjectResult(result);

                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ResponseModel.Failed(message: "There was an error processing your request, please try again."));
            }
        }


        #region private methods

        private IActionResult CreateResponse(AuthenticationResponse response, string ErrorMessage)
        {
            if (response.ResponseType == ResponseType.Error)
                return new BadRequestObjectResult(ResponseModel.Failed(message: response.Data));

            if (response.ResponseType == ResponseType.Success)
            {
                var baseModel = JsonSerializer.Deserialize<ResponseModel>(response.Data);
                return new OkObjectResult(baseModel);
            }
            return new BadRequestObjectResult(ResponseModel.Failed(message: ErrorMessage));
        }

        #endregion
    }
}
