using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TimeLogger.Client.NetCore.Extensions;
using TimeLogger.Client.NetCore.Helpers;
using TimeLogger.Client.NetCore.Models;
using TimeLogger.Client.NetCore.Options;
using TimeLogger.Client.NetCore.Utilities.Constants;
using TimeLogger.Client.NetCore.Utilities.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TimeLogger.Client.NetCore.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SecurityOptions _securityOptions;
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);
        public AccountController(IOptionsSnapshot<SecurityOptions> securityOptions)
        {
            _securityOptions = securityOptions.Value;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (_securityOptions.Service == "AspnetIdentity")
                return View();
            else if (_securityOptions.Service == "SingleSignOn")
                return new ChallengeResult("oidc", new AuthenticationProperties { RedirectUri = "Account/RegisterCallback" });
            else return null; // Implement others if required.
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (string.IsNullOrWhiteSpace(model.UserName))
                model.UserName = model.Email;
            if (ModelState.IsValid)
            {
                var result = await HttpClientHelper.PostAsync<IdentityResult>(model, HttpContentType.FormUrlEncoded, "api/Account/Create");
                if (result.Success)
                    return RedirectToAction(nameof(EmailConfirmationStatus), "Account", new { isConfirmed = false });

                ModelState.AddModelError("", result.Message);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/RegisterCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCallback(string returnUrl = null, string remoteError = null)
        {
            var response = await HttpClientHelper.GetAsync<IdentityUserResponseModel>("api/Account/Profile");
            if (!response.Success)
                return View("Error");

            return RedirectToAction(nameof(HomeController.Index), "Home"); //return RedirectToLocal(returnUrl);
        }

        ////
        //// POST: /Account/RegisterMerchant
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RegisterMerchant(RegisterMerchantViewModel model, string returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    if (string.IsNullOrWhiteSpace(model.UserName))
        //        model.UserName = model.Email;
        //    if (ModelState.IsValid)
        //    {
        //        var result = await HttpClientHelper.PostAsync<IdentityResult>(model, HttpContentType.FormUrlEncoded, "api/Account/RegisterMerchant");
        //        if (result.Success)
        //            return RedirectToAction(nameof(EmailConfirmationStatus), "Account", new { isConfirmed = false });

        //        ModelState.AddModelError("", result.Message);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return View("Error");

            await HttpClientHelper.SetBearerToken(null);
            if (!string.IsNullOrWhiteSpace(User.GetUserId()))
            {
                var logoutResult = await HttpClientHelper.PostAsync<dynamic>(null, HttpContentType.FormUrlEncoded, "api/Account/Logout");
                if (!logoutResult.Success)
                    return View("Error");

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            var model = new ConfirmEmailViewModel
            {
                UserId = userId,
                Code = code
            };
            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/ConfirmEmail");
            return result.Success
                            ? RedirectToAction(nameof(EmailConfirmationStatus), new { isConfirmed = true })
                            : RedirectToAction("Error", "Account");
        }

        //
        // GET: /Account/EmailConfirmationStatus
        [HttpGet]
        [AllowAnonymous]
        public ActionResult EmailConfirmationStatus(bool isConfirmed)
        {
            ViewBag.IsConfirmed = isConfirmed;
            return View("ConfirmEmail");
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null, string errorMessage = null)
        {
            if (_securityOptions.Service == "AspnetIdentity")
            {
                ViewData["ReturnUrl"] = returnUrl;
                var result = await HttpClientHelper.GetAsync<List<AuthenticationSchemeModel>>("api/Account/LoginProviders");
                var model = new LoginViewModel();
                if (result.Success)
                    model.ExternalAuthenticationProviders = result.Data;
                else
                    model.ExternalAuthenticationProviders = new List<AuthenticationSchemeModel>();

                if (errorMessage != null)
                    ModelState.AddModelError(string.Empty, errorMessage);

                return View(model);
            }
            else if (_securityOptions.Service == "SingleSignOn")
                return new ChallengeResult("oidc", new AuthenticationProperties { RedirectUri = "Account/LoginCallback" });
            else return null; // Implement others if required.
        }

        //
        // GET: /Account/LoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCallback(string returnUrl = null, string remoteError = null)
        {
            var response = await HttpClientHelper.GetAsync<IdentityUserResponseModel>("api/Account/Profile");
            if (!response.Success)
                return View("Error");

            return RedirectToAction(nameof(HomeController.Index), "Home"); //return RedirectToLocal(returnUrl);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            TempData["UserName"] = model.Email;

            if (!ModelState.IsValid)
                return View(model);

            //// This doesn't count login failures towards account lockout
            //// To enable password failures to trigger account lockout, change to shouldLockout: true
            if (string.IsNullOrWhiteSpace(model.UserName))
                model.UserName = model.Email;

            var tokenResponse = await HttpClientHelper.TokenAsync(model, HttpContentType.FormUrlEncoded, "api/Account/Login");
            if (tokenResponse.Status == LoginStatus.Succeded)
            {
                await HttpClientHelper.SetBearerToken(tokenResponse.Data.AccessToken);
                var userResponse = await IdentityLogin(tokenResponse.Data.AccessToken, model.RememberMe);
                if (userResponse.Item1)
                    return RedirectToAction(nameof(HomeController.Index), "Home"); //return RedirectToLocal(returnUrl);

                //ModelState.AddModelError(string.Empty, userResponse.Item2);
                //return View(model);
                return RedirectToAction(nameof(Login), new { returnUrl, errorMessage = userResponse.Item2 });
            }
            else if (tokenResponse.Status == LoginStatus.RequiresTwoFactor)
            {
                if(tokenResponse.Data != null)
                {
                    TempData["Provider"] = tokenResponse.Data.TwoFactorType.Name;
                    return RedirectToAction(nameof(TwoFactorLogin), new { returnUrl, model.RememberMe });
                }
                //ModelState.AddModelError(string.Empty, tokenResponse.Message);
                //return View(model);
                return RedirectToAction(nameof(Login), new { returnUrl, errorMessage = tokenResponse.Message });
            }
            //else if (result.IsLockedOut)
            //{
            //    _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
            //    return RedirectToAction(nameof(Lockout));
            //}
            //ModelState.AddModelError(string.Empty, tokenResponse.Message);
            //return View(model);
            return RedirectToAction(nameof(Login), new { returnUrl, errorMessage = tokenResponse.Message });
        }

        //
        // GET: /Account/TwoFactorLogin
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TwoFactorLogin(bool rememberMe, string returnUrl = null)
        {
            var userName = TempData["UserName"].ToString();
            var provider = TempData["Provider"].ToString();

            var model = new VerifyCodeViewModel { UserName = userName, Provider = provider, RememberMe = rememberMe };

            TempData["UserName"] = userName;
            TempData["Provider"] = provider;
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        //
        // POST: /Account/TwoFactorLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactorLogin(VerifyCodeViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var authenticatorCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var tokenResponse = await HttpClientHelper.TokenAsync(model, HttpContentType.FormUrlEncoded, "api/Account/TwoFactorLogin");
            if (tokenResponse.Status == LoginStatus.Succeded)
            {
                await HttpClientHelper.SetBearerToken(tokenResponse.Data.AccessToken);
                var userResponse = await IdentityLogin(tokenResponse.Data.AccessToken, model.RememberMe);
                if (userResponse.Item1)
                    return RedirectToAction(nameof(HomeController.Index), "Home"); //return RedirectToLocal(returnUrl);

                ModelState.AddModelError(string.Empty, userResponse.Item2);
                return View(model);
            }
            //else if (result.IsLockedOut)
            //{
            //    _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
            //    return RedirectToAction(nameof(Lockout));
            //}
            else
            {
                ModelState.AddModelError(string.Empty, tokenResponse.Message);
                return View(model);
            }
        }

        //
        // GET: /Account/RecoveryCodeLogin
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryCodeLogin(string returnUrl = null)
        {
            string userName = string.Empty;
            if(TempData["UserName"] != null)
                userName = TempData["UserName"].ToString();

            var model = new VerifyCodeViewModel { UserName = userName };
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/RecoveryCodeLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoveryCodeLogin(VerifyCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["UserName"] = model.UserName;
                return View(model);
            }

            if(string.IsNullOrWhiteSpace(model.UserName))
            {
                TempData["UserName"] = model.UserName;
                ModelState.AddModelError(string.Empty, "Error accored.");
                return View(model);
            }

            var recoveryCode = model.Code.Replace(" ", string.Empty);

            var tokenResponse = await HttpClientHelper.TokenAsync(model, HttpContentType.FormUrlEncoded, "api/Account/RecoveryCodeLogin");
            if (tokenResponse.Status == LoginStatus.Succeded)
            {
                await HttpClientHelper.SetBearerToken(tokenResponse.Data.AccessToken);
                var userResponse = await IdentityLogin(tokenResponse.Data.AccessToken, model.RememberMe);
                if (userResponse.Item1)
                    return RedirectToAction(nameof(HomeController.Index), "Home"); //return RedirectToLocal(returnUrl);

                ModelState.AddModelError(string.Empty, userResponse.Item2);
                return View(model);
            }
            //else if (result.IsLockedOut)
            //{
            //    _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
            //    return RedirectToAction(nameof(Lockout));
            //}
            else
            {
                ModelState.AddModelError(string.Empty, tokenResponse.Message);
                return View(model);
            }
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var loginProperties = await HttpClientHelper.GetAsync<Microsoft.AspNetCore.Authentication.AuthenticationProperties>("api/Account/LoginProperties?provider=" + provider + "&redirectUrl=" + redirectUrl);
            if (loginProperties.Success)
                return new ChallengeResult(provider, loginProperties.Data);
            else
                return RedirectToAction(nameof(Login));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var loginInfo = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (loginInfo == null)
                return RedirectToAction(nameof(ExternalLoginFailure));

            var loginProvder = loginInfo.Properties.Items.FirstOrDefault(x => x.Key == "LoginProvider").Value;
            var providerKey = loginInfo.Principal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            var firstName = loginInfo.Principal.Claims.Where(c => c.Type == ClaimTypes.GivenName).Select(c => c.Value).SingleOrDefault();
            var lastName = loginInfo.Principal.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault();
            var emailAddress = loginInfo.Principal.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

            var model = new ExternalLoginViewModel
            {
                LoginProvider = loginProvder,
                ProviderKey = providerKey
            };

            var tokenResponse = await HttpClientHelper.TokenAsync(model, HttpContentType.FormUrlEncoded, "api/Account/ExternalLogin");
            if (tokenResponse.Status == LoginStatus.Succeded)
            {
                await HttpClientHelper.SetBearerToken(tokenResponse.Data.AccessToken);
                var userResponse = await IdentityExternalLogin(loginInfo.Principal.Claims, tokenResponse.Data.AccessToken, loginProvder, providerKey);
                if (userResponse.Item1)
                    return RedirectToAction(nameof(HomeController.Index), "Home"); //return RedirectToLocal(returnUrl);

                return RedirectToAction(nameof(ExternalLoginFailure));
            }
            else if (tokenResponse.Status == LoginStatus.RequiresTwoFactor)
            {
                if (tokenResponse.Data != null)
                {
                    TempData["UserName"] = emailAddress;
                    TempData["Provider"] = tokenResponse.Data.TwoFactorType.Name;
                    return RedirectToAction(nameof(TwoFactorLogin), new { returnUrl, rememberMe = false});
                }
                //ModelState.AddModelError(string.Empty, tokenResponse.Message);
                //return View(model);
                return RedirectToAction(nameof(Login), new { returnUrl, errorMessage = tokenResponse.Message });
            }
            //else if (result.IsLockedOut)
            //{
            //    _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
            //    return RedirectToAction(nameof(Lockout));
            //}
            else
            {
                ViewData["ReturnUrl"] = returnUrl;

                var registerExternalModel = new RegisterExternalViewModel
                {
                    Email = emailAddress,
                    FirstName = firstName,
                    LastName = lastName,
                    Provider = loginProvder,
                    ProviderKey = providerKey,
                    ProviderDisplayName = loginProvder
                };
                return View("ExternalLogin", registerExternalModel);
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(RegisterExternalViewModel model, string returnUrl = null)
        {
            var loginInfo = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (loginInfo == null)
                return RedirectToAction(nameof(ExternalLoginFailure));

            var tokenResponse = await HttpClientHelper.TokenAsync(model, HttpContentType.FormUrlEncoded, "api/Account/CreateExternal");
            if (tokenResponse.Status == LoginStatus.Succeded)
            {
                await HttpClientHelper.SetBearerToken(tokenResponse.Data.AccessToken);
                var userResponse = await IdentityExternalLogin(loginInfo.Principal.Claims, tokenResponse.Data.AccessToken, model.Provider, model.ProviderKey);
                if (userResponse.Item1)
                    return RedirectToAction(nameof(HomeController.Index), "Home"); //return RedirectToLocal(returnUrl);

                return RedirectToAction(nameof(ExternalLoginFailure));
            }
            //else if (result.IsLockedOut)
            //{
            //    _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
            //    return RedirectToAction(nameof(Lockout));
            //}
            else
            {
                return RedirectToAction(nameof(ExternalLoginFailure));
            }
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        private async Task<Tuple<bool, string>> IdentityLogin(string accessToken, bool rememberBrowser = false, bool rememberMachine = false)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return new Tuple<bool, string>(false, "Token not found.");

            var claims = AuthenticationHelper.GetUserClaims(accessToken).ToList();
            claims.Add(new Claim(ClaimTypes.PrimarySid, accessToken));
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
            var properties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                IsPersistent = rememberBrowser,
            };

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimPrincipal,
                properties
            );
            return new Tuple<bool, string>(true, "");
        }

        private async Task<Tuple<bool, string>> IdentityExternalLogin(IEnumerable<Claim> externalClaims, string accessToken, string loginProvider, string providerKey, bool rememberBrowser = false, bool rememberMachine = false)
        {
            var userResponse = await HttpClientHelper.GetAsync<IdentityUserResponseModel>("api/Account/ExternalUserProfile?loginProvider=" + loginProvider + "&providerKey=" + providerKey);
            if (userResponse.Success)
            {
                var claim = userResponse.Data;
                var name = externalClaims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
                var givenName = externalClaims.Where(c => c.Type == ClaimTypes.GivenName).Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrWhiteSpace(givenName))
                    givenName = name.Split(' ').FirstOrDefault();
                var surName = externalClaims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrWhiteSpace(surName))
                    surName = name.Split(' ').LastOrDefault();
                var emailAddress = externalClaims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();
                List<Claim> claims = new List<Claim>();
                claims.AddRange(new List<Claim>
                        {
                            new Claim(ClaimTypes.PrimarySid, accessToken),
                            new Claim(ClaimTypes.Sid, claim.Id),
                            new Claim(ClaimTypes.Name, claim.UserName),
                            new Claim(ClaimTypes.Email, emailAddress),
                            new Claim(CustomClaimTypes.FirstName.ToString(), givenName),
                            new Claim(CustomClaimTypes.LastName.ToString(), surName),
                            new Claim(CustomClaimTypes.User.ToString(), JsonSerializer.Serialize(claim))
                        });

                foreach (var role in claim.Roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
                var properties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                    IsPersistent = rememberBrowser,
                };

                Thread.CurrentPrincipal = claimPrincipal;
                //await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(_httpContext, claimPrincipal);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimPrincipal,
                    properties
                );
                return new Tuple<bool, string>(true, "");
            }

            return new Tuple<bool, string>(false, userResponse.Message);
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/ForgotPassword");
                if (result.Success)
                {
                    ViewBag.Message = result.Message;
                    return View("ForgotPasswordConfirmation");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await HttpClientHelper.PostAsync<IdentityResult>(model, HttpContentType.FormUrlEncoded, "api/Account/ResetPassword");
                if (result.Success)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/UserProfile
        public async Task<IActionResult> UserProfile(string message = null)
        {
            if (_securityOptions.Service == "AspnetIdentity")
            {
                ViewBag.StatusMessage = message;

                var response = await HttpClientHelper.GetAsync<IdentityUserResponseModel>("api/Account/Profile");
                if (response.Success)
                    return View(response.Data);

                return View("Error");
            }
            else if (_securityOptions.Service == "SingleSignOn")
                return Redirect(_securityOptions.ProfileUri);
            else return null; // Implement others if required.
        }

        //
        // GET: /Account/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await HttpClientHelper.PostAsync<IdentityResult> (model, HttpContentType.FormUrlEncoded, "api/Account/ChangePassword");
            if (result.Success)
                return RedirectToAction(nameof(UserProfile), new { message = "Your password has been changed." });

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        //
        // GET: /Account/SetPassword
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Account/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/SetPassword");
            if (result.Success)
                return RedirectToAction(nameof(UserProfile), new { message = result.Message });

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        //
        // POST: /Account/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChangeEmail", model);

            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/SendChangeEmailCode");
            if (result.Success)
                return RedirectToAction(nameof(UserProfile), new { message = result.Message });

            ModelState.AddModelError(string.Empty, result.Message);
            return PartialView("_ChangeEmail", model);
        }

        //
        // GET: /Account/ChangeEmail
        [AllowAnonymous]
        public async Task<IActionResult> ChangeEmail(string userId, string email, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(email))
                return View("Error");

            var model = new ConfirmEmailViewModel
            {
                UserId = userId,
                Email = email,
                Code = code
            };
            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/ChangeEmail");
            return result.Success
                            ? RedirectToAction(nameof(UserProfile), new { message = result.Message })
                            : RedirectToAction(nameof(UserProfile));
        }

        //
        // GET: /Account/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Account/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AddPhoneNumber", model);
            }
            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/SendChangePhoneCode");
            if (result.Success)
                return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });

            ModelState.AddModelError(string.Empty, result.Message);
            return PartialView("_AddPhoneNumber", model);
        }

        //
        // GET: /Account/VerifyPhoneNumber
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            //return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
            return PartialView("_VerifyPhoneNumber", new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Account/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_VerifyPhoneNumber", model);

            var result = await HttpClientHelper.PutAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/ChangePhoneNumber");
            if (result.Success)
                return RedirectToAction(nameof(UserProfile), new { message = "Your phone number is been added successfully." });

            ModelState.AddModelError(string.Empty, result.Message);
            return PartialView("_VerifyPhoneNumber", model);
        }

        //
        // POST: /Account/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var result = await HttpClientHelper.DeleteAsync<dynamic>("api/Account/RemovePhoneNumber");
            if (result.Success)
                return RedirectToAction(nameof(UserProfile), new { message = "Your phone number is been deleted successfully." });

            ModelState.AddModelError(string.Empty, result.Message);
            return RedirectToAction(nameof(UserProfile), new { message = result.Message });
        }

        //
        // GET: /Account/TwoFactorAuthentication
        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication(string message = null)
        {
            ViewBag.StatusMessage = message;

            var authenticationResponse = await HttpClientHelper.GetAsync<IdentityUserResponseModel>("api/Account/Profile");
            if (authenticationResponse.Success)
                return View(authenticationResponse.Data);

            ModelState.AddModelError(string.Empty, authenticationResponse.Message);
            return View(authenticationResponse.Data);
        }

        //
        // GET: /Account/EnableAuthenticator
        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var model = new AuthenticatorViewModel ();
            var response = await LoadSharedKeyAndQrCodeUri(model);
            if (response.Item1)
                return View(model);

            ModelState.AddModelError(string.Empty, response.Item2);
            return View(model);
        }

        private async Task<Tuple<bool, string>> LoadSharedKeyAndQrCodeUri(AuthenticatorViewModel model)
        {
            var response = await HttpClientHelper.GetAsync<AuthenticatorModel>("api/Account/GetSharedKeyAndQrCodeUri");
            if (!response.Success)
                return new Tuple<bool, string>(response.Success, response.Message);

            model.AuthenticatorUri = response.Data.AuthenticatorUri;
            model.SharedKey = response.Data.SharedKey;
            return new Tuple<bool, string>(response.Success, response.Message);
        }

        //
        // POST: /Account/EnableAuthenticator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(AuthenticatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var response = await LoadSharedKeyAndQrCodeUri(model);
                return View(model);
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var verifyCodeModel = new VerifyCodeViewModel
            {
                Provider = Utilities.Constants.TwoFactorTypes.Authenticator,
                Code = model.Code
            };
            var authenticatorResult = await HttpClientHelper.PostAsync<IEnumerable<string>>(verificationCode, HttpContentType.FormUrlEncoded, "api/Account/EnableTwoFactorAuthentication");
            if (!authenticatorResult.Success)
            {
                ModelState.AddModelError("Code", authenticatorResult.Message);
                var response = await LoadSharedKeyAndQrCodeUri(model);
                return View(model);
            }

            var recoveryCodes = authenticatorResult.Data;
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        //
        // GET: /Account/ResetAuthenticatorWarning
        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        //
        // POST: /Account/ResetAuthenticator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var resetAuthenticatorResult = await HttpClientHelper.GetAsync<dynamic>("api/Account/ResetAuthenticator");

            return RedirectToAction(nameof(EnableAuthenticator), new { message = resetAuthenticatorResult.Message });
        }

        //
        // GET: /Account/EnableEmailOtp
        [HttpGet]
        public async Task<ActionResult> EnableEmailOtp()
        {
            var model = new SendCodeViewModel
            {
                SelectedProvider = Utilities.Constants.TwoFactorTypes.Email
            };

            var otpResult = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/SendTwoFactorCode");
            if (!otpResult.Success)
                ModelState.AddModelError("Code", otpResult.Message);

            return View(nameof(EnableOtp), new VerifyCodeViewModel { Provider = model.SelectedProvider });
        }

        //
        // POST: /Account/EnableOtp
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableOtp(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var otpResult = await HttpClientHelper.PostAsync<IEnumerable<string>>(model, HttpContentType.FormUrlEncoded, "api/Account/EnableTwoFactorAuthentication");
            if (!otpResult.Success)
            {
                ModelState.AddModelError("Code", otpResult.Message);
                return View(model);
            }

            var recoveryCodes = otpResult.Data;
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        //
        // GET: /Account/DisableTwoFactorAuthenticationWarning
        [HttpGet]
        public async Task<IActionResult> DisableTwoFactorAuthenticationWarning()
        {
            return View(nameof(DisableTwoFactorAuthentication));
        }

        //
        // POST: /Account/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var disable2faResult = await HttpClientHelper.GetAsync<dynamic>("api/Account/DisableTwoFactorAuthentication");
            ///if (!disable2faResult.Success)

            return RedirectToAction(nameof(TwoFactorAuthentication), new { message = disable2faResult.Message });
        }

        //
        // GET: /Account/GenerateRecoveryCodesWarning
        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            return View(nameof(GenerateRecoveryCodes));
        }

        //
        // POST: /Account/GenerateRecoveryCodes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var recoveryCodesResult = await HttpClientHelper.GetAsync<IEnumerable<string>>("api/Account/TwoFactorRecoveryCodes");
            if (!recoveryCodesResult.Success)
            {
                ModelState.AddModelError("", recoveryCodesResult.Message);
                return View(nameof(ShowRecoveryCodes), new RecoveryCodesViewModel());
            }

            var recoveryCodes = recoveryCodesResult.Data;
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();
            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        //
        // GET: /Account/ShowRecoveryCodes
        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new RecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
            return View(model);
        }


        ////
        //// GET: /Account/SendCode
        ////[AllowAnonymous]
        ////public async Task<IActionResult> SendCode(string returnUrl, bool rememberMe)
        ////{
        ////    //var userId = await SignInManager.GetVerifiedUserIdAsync();
        ////    //if (userId == null)
        ////    //{
        ////    //    return View("Error");
        ////    //}
        ////    //var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        ////    //var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        ////    //return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        ////    return null;
        ////}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SendCode(SendCodeViewModel model)
        //{
        //    var otpResult = await HttpClientHelper.PostAsync<string>("api/Account/SendCode", model);
        //    if (!otpResult.Success)
        //    {
        //        ModelState.AddModelError("Code", otpResult.Message);
        //        return View();
        //    }
        //    return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<IActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    model.UserId = GetUserId();
        //    var otpVerificationResult = await HttpClientHelper.PostAsync<string>("api/Account/VerifyCode", model);
        //    if (!otpVerificationResult.Success)
        //    {
        //        ModelState.AddModelError("Code", otpVerificationResult.Message);
        //        return View();
        //    }
        //    return RedirectToAction(nameof(EnableOtp), new { OtpOption = "test" });
        //}

        //
        // GET: /Account/ManageLogins
        public async Task<IActionResult> ManageLogins(string message = null)
        {
            ViewBag.StatusMessage = message;

            var authenticationResponse = await HttpClientHelper.GetAsync<IdentityUserResponseModel>("api/Account/Profile");
            if (authenticationResponse.Success)
                return View("ExternalLogins", new ExternalLoginsViewModel
                {
                    CurrentLogins = authenticationResponse.Data.Logins,
                    OtherLogins = authenticationResponse.Data.OtherLogins,
                    ShowRemoveButton = authenticationResponse.Data.HasPassword || authenticationResponse.Data.Logins.Count > 1
                });

            ModelState.AddModelError(string.Empty, authenticationResponse.Message);
            return View("ExternalLogins", new ExternalLoginsViewModel());
        }

        //
        // POST: /Account/AddLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(AddLoginCallback), "Account");
            var userId = User.GetUserId();
            var loginProperties = await HttpClientHelper.GetAsync<Microsoft.AspNetCore.Authentication.AuthenticationProperties>("api/Account/LoginProperties?provider=" + provider + "&redirectUrl=" + redirectUrl + "&userId=" + userId);
            if (loginProperties.Success)
                return new ChallengeResult(provider, loginProperties.Data);
            else
                return RedirectToAction(nameof(ManageLogins), new { message = loginProperties.Message });
        }
    
        //
        // GET: /Account/AddLoginCallback
        public async Task<IActionResult> AddLoginCallback()
        {
            var login = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            var loginProvder = login.Properties.Items.FirstOrDefault(x => x.Key == "LoginProvider").Value;
            var providerKey = login.Principal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            
            var model = new AddLoginViewModel
            {
                Provider = loginProvder,
                ProviderKey = providerKey,
                ProviderDisplayName = loginProvder
            };

            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/AddLogin");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return RedirectToAction(nameof(ManageLogins), new { message = result.Message });
        }

        //
        // POST: /Account/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(string provider, string providerKey)
        {
            var result = await HttpClientHelper.PostAsync<dynamic>(new RemoveLoginViewModel { Provider = provider, ProviderKey = providerKey }, HttpContentType.FormUrlEncoded, "api/Account/RemoveLogin");
            return RedirectToAction(nameof(ManageLogins), new { message = result.Message });
        }

        //
        // GET: /Account/Lockout
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        //
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (_securityOptions.Service == "AspnetIdentity")
            {
                var result = await HttpClientHelper.PostAsync<dynamic>(null, HttpContentType.FormUrlEncoded, "api/Account/Logout");
                if (!result.Success)
                    return View("Error");

                await HttpClientHelper.SetBearerToken(null);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (_securityOptions.Service == "SingleSignOn")
                return new SignOutResult(new List<string> { "Cookies", "oidc" });
            else return null; // Implement others if required.            
        }

        //
        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        

        #region Helpers

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }
        //}

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion Helpers
    }
}
