using TimeLogger.Client.Mvc.Helpers;
using TimeLogger.Client.Mvc.Models;
using TimeLogger.Client.Mvc.Utilities.Enums;
using TimeLogger.Client.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TimeLogger.Client.Mvc.Options;

namespace TimeLogger.Client.Mvc.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public AccountController()
        {
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (SecurityOptions.Service == "AspnetIdentity")
                return View();
            else if (SecurityOptions.Service == "SingleSignOn")
                return new ChallengeResult("oidc", returnUrl ?? "Account/RegisterCallback");
            else return null; // Implement others if required.
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterUserViewModel model)
        {
           if(string.IsNullOrWhiteSpace(model.UserName))
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
        public async Task<ActionResult> RegisterCallback(string returnUrl = null, string remoteError = null)
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
        //public async Task<ActionResult> RegisterMerchant(RegisterMerchantViewModel model)
        //{
        //    if (string.IsNullOrWhiteSpace(model.UserName))
        //        model.UserName = model.Email;

        //    if (ModelState.IsValid)
        //    {
        //        var result = await HttpClientHelper.PostAsync<RegisterMerchantViewModel>("api/Account/RegisterMerchant", model);
        //        if (result.Success)
        //            return RedirectToAction(nameof(EmailConfirmationStatus), "Account", new { isConfirmed = false });

        //        ModelState.AddModelError("", result.Message);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return View("Error");

            await HttpClientHelper.SetBearerToken(null);
            if (!string.IsNullOrWhiteSpace(User.GetUserId()))
            {
                var logoutResult = await HttpClientHelper.PostAsync<dynamic>(null, HttpContentType.FormUrlEncoded, "api/Account/Logout");
                if (!logoutResult.Success)
                    return View(nameof(Error));
            }

            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);

            var model = new ConfirmEmailViewModel
            {
                UserId = userId,
                Code = code
            };
            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/ConfirmEmail");
            return result.Success 
                            ? RedirectToAction(nameof(EmailConfirmationStatus), "Account", new { isConfirmed = true }) 
                            : RedirectToAction(nameof(Error), "Account");
        }

        //
        // GET: /Account/EmailConfirmationStatus
        [AllowAnonymous]
        public ActionResult EmailConfirmationStatus(bool isConfirmed)
        {
            ViewBag.IsConfirmed = isConfirmed;
            return View(nameof(ConfirmEmail));
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl, string errorMessage = null)
        {
            if (SecurityOptions.Service == "AspnetIdentity")
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
            else if (SecurityOptions.Service == "SingleSignOn")
            {
                var redirectUrl = Url.Action("LoginCallback", "Account");
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? redirectUrl });
                return null;
            }
            else return null; // Implement others if required.
        }

        //
        // GET: /Account/LoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> LoginCallback(string returnUrl = null, string remoteError = null)
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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
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
                var accessToken = tokenResponse.Data.AccessToken;
                await HttpClientHelper.SetBearerToken(accessToken);
                var userResponse = await IdentityLogin(accessToken, model.RememberMe);
                if (userResponse.Item1)
                    return RedirectToAction(nameof(HomeController.Index), "Home"); //return RedirectToLocal(returnUrl);

                //ModelState.AddModelError(string.Empty, userResponse.Item2);
                //return View(model);
                return RedirectToAction(nameof(Login), new { returnUrl, errorMessage = userResponse.Item2 });
            }
            else if (tokenResponse.Status == LoginStatus.RequiresTwoFactor)
            {
                if (tokenResponse.Data != null)
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
        public async Task<ActionResult> TwoFactorLogin(bool rememberMe, string returnUrl = null)
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
        public async Task<ActionResult> TwoFactorLogin(VerifyCodeViewModel model, bool rememberMe, string returnUrl = null)
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
        public async Task<ActionResult> RecoveryCodeLogin(string returnUrl = null)
        {
            string userName = string.Empty;
            if (TempData["UserName"] != null)
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
        public async Task<ActionResult> RecoveryCodeLogin(VerifyCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                TempData["UserName"] = model.UserName;
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.UserName))
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
        public async Task<ActionResult> ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var loginProperties = await HttpClientHelper.GetAsync<AuthenticationProperties>("api/Account/LoginProperties?provider=" + provider + "&redirectUrl=" + redirectUrl);
            if (loginProperties.Success)
                return new ChallengeResult(provider, loginProperties.Data.RedirectUri);
            else
                return RedirectToAction(nameof(Login));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await ControllerContext.HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction(nameof(ExternalLoginFailure));

            var loginProvder = loginInfo.Login.LoginProvider;
            var providerKey = loginInfo.Login.ProviderKey;
            var name = loginInfo.ExternalIdentity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault()?.Split(' ');
            var firstName = name?.First(); // loginInfo.Login.Claims.Where(c => c.Type == ClaimTypes.GivenName).Select(c => c.Value).SingleOrDefault();
            var lastName = name?.Last(); // loginInfo.Login.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault();
            var emailAddress = loginInfo.ExternalIdentity.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

            var model = new ExternalLoginViewModel
            {
                LoginProvider = loginProvder,
                ProviderKey = providerKey
            };

            var tokenResponse = await HttpClientHelper.TokenAsync(model, HttpContentType.FormUrlEncoded, "api/Account/ExternalLogin");
            if (tokenResponse.Status == LoginStatus.Succeded)
            {
                await HttpClientHelper.SetBearerToken(tokenResponse.Data.AccessToken);
                var userResponse = await IdentityExternalLogin(loginInfo.ExternalIdentity.Claims, tokenResponse.Data.AccessToken, loginProvder, providerKey);
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
                    return RedirectToAction(nameof(TwoFactorLogin), new { returnUrl, rememberMe = false });
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
        public async Task<ActionResult> ExternalLoginConfirmation(RegisterExternalViewModel model, string returnUrl)
        {
            var loginInfo = await ControllerContext.HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction(nameof(ExternalLoginFailure));

            var tokenResponse = await HttpClientHelper.TokenAsync(model, HttpContentType.FormUrlEncoded, "api/Account/CreateExternal");
            if (tokenResponse.Status == LoginStatus.Succeded)
            {
                await HttpClientHelper.SetBearerToken(tokenResponse.Data.AccessToken);
                var userResponse = await IdentityExternalLogin(loginInfo.ExternalIdentity.Claims, tokenResponse.Data.AccessToken, model.Provider, model.ProviderKey);
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
            var claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

            Thread.CurrentPrincipal = claimPrincipal;
            var ctx = Request.GetOwinContext();
            var auuthenticationManager = ctx.Authentication;
            auuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            auuthenticationManager.SignIn(claimsIdentity);

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

                var claimsIdentity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

                Thread.CurrentPrincipal = claimPrincipal;
                var ctx = Request.GetOwinContext();
                var auuthenticationManager = ctx.Authentication;
                auuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                auuthenticationManager.SignIn(claimsIdentity);

                return new Tuple<bool, string>(true, "");
            }

            return new Tuple<bool, string>(false, userResponse.Message);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/ForgotPassword");
                if (result.Success)
                {
                    ViewBag.Message = result.Message;
                    return View(nameof(ForgotPasswordConfirmation));
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await HttpClientHelper.PostAsync<IdentityResult>(model, HttpContentType.FormUrlEncoded, "api/Account/ResetPassword");
                if (result.Success)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/UserProfile
        public async Task<ActionResult> UserProfile(string message = null)
        {
            if (SecurityOptions.Service == "AspnetIdentity")
            {
                ViewBag.StatusMessage = message;

                var response = await HttpClientHelper.GetAsync<IdentityUserResponseModel>("api/Account/Profile");
                if (response.Success)
                    return View(response.Data);

                return View(nameof(Error));
            }
            else if (SecurityOptions.Service == "SingleSignOn")
                return Redirect(SecurityOptions.ProfileUri);
            else return null; // Implement others if required.           
        }

        //
        // GET: /Account/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await HttpClientHelper.PostAsync<IdentityResult>(model, HttpContentType.FormUrlEncoded, "api/Account/ChangePassword");
            if (result.Success)
                return RedirectToAction(nameof(UserProfile), new { message = "Your password has been changed." });

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        //
        // GET: /Account/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Account/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
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
        // GET: /Account/ChangeEmail
        public ActionResult ChangeEmailView()
        {
            return View(nameof(ChangeEmail));
        }

        //
        // POST: /Account/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/SendChangeEmailCode");
            if (result.Success)
                return RedirectToAction(nameof(UserProfile), new { message = result.Message });

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        //
        // GET: /Account/ChangeEmail
        [AllowAnonymous]
        public async Task<ActionResult> ChangeEmail(string userId, string email, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return View(nameof(Error));

            var model = new ConfirmEmailViewModel
            {
                UserId = userId,
                Email = email,
                Code = code
            };
            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/ChangeEmail");
            return result.Success
                            ? RedirectToAction(nameof(UserProfile), new { message = result.Message })
                            : RedirectToAction(nameof(Error), "Account");
        }

        //
        // GET: /Account/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Account/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/SendChangePhoneCode");
            if (result.Success)
                return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        //
        // GET: /Account/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            return phoneNumber == null ? View(nameof(Error)) : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Account/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await HttpClientHelper.PutAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/ChangePhoneNumber");
            if (result.Success)
                return RedirectToAction(nameof(UserProfile), new { message = "Your phone number is been added successfully." });

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        //
        // POST: /Account/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
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
        public async Task<ActionResult> TwoFactorAuthentication(string message = null)
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
        public async Task<ActionResult> EnableAuthenticator()
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
        public async Task<ActionResult> EnableAuthenticator(AuthenticatorViewModel model)
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
        public ActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        //
        // POST: /Account/ResetAuthenticator
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetAuthenticator()
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

            return View(nameof(EnableOtp), new VerifyCodeViewModel { Provider = model.SelectedProvider});

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
            return RedirectToAction(nameof(UserProfile), new { message = otpResult.Message });
        }

        //
        // GET: /Account/DisableTwoFactorAuthenticationWarning
        [HttpGet]
        public async Task<ActionResult> DisableTwoFactorAuthenticationWarning()
        {
            return View(nameof(DisableTwoFactorAuthentication));
        }

        //
        // POST: /Account/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            var disable2faResult = await HttpClientHelper.GetAsync<dynamic>("api/Account/DisableTwoFactorAuthentication");
            ///if (!disable2faResult.Success)

            return RedirectToAction(nameof(TwoFactorAuthentication), new { message = disable2faResult.Message });
        }

        //
        // GET: /Account/GenerateRecoveryCodesWarning
        [HttpGet]
        public async Task<ActionResult> GenerateRecoveryCodesWarning()
        {
            return View(nameof(GenerateRecoveryCodes));
        }

        //
        // POST: /Account/GenerateRecoveryCodes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GenerateRecoveryCodes()
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
        public ActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new RecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
            return View(model);
        }

        //
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    //var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    //if (userId == null)
        //    //{
        //    //    return View("Error");
        //    //}
        //    //var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    //var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    //return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //    return null;
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
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
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
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
        public async Task<ActionResult> ManageLogins(string message = null)
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
        public async Task<ActionResult> AddLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(AddLoginCallback), "Account");
            var userId = User.GetUserId();
            var loginProperties = await HttpClientHelper.GetAsync<AuthenticationProperties>("api/Account/LoginProperties?provider=" + provider + "&redirectUrl=" + redirectUrl + "&userId=" + userId);
            if (loginProperties.Success)
                return new ChallengeResult(provider, loginProperties.Data.RedirectUri, userId);
            else
                return RedirectToAction(nameof(ManageLogins), new { message = loginProperties.Message });
        }

        //
        // GET: /Account/AddLoginCallback
        public async Task<ActionResult> AddLoginCallback()
        {
            var loginInfo = await ControllerContext.HttpContext.GetOwinContext().Authentication.GetExternalLoginInfoAsync();
            var model = new AddLoginViewModel
            {
                Provider = loginInfo.Login.LoginProvider,
                ProviderKey = loginInfo.Login.ProviderKey,
                ProviderDisplayName = loginInfo.Login.LoginProvider
            };
            var result = await HttpClientHelper.PostAsync<dynamic>(model, HttpContentType.FormUrlEncoded, "api/Account/AddLogin");

            return RedirectToAction(nameof(ManageLogins), new { message = result.Message });
        }

        //
        // POST: /Account/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string provider, string providerKey)
        {
            var result = await HttpClientHelper.PostAsync<dynamic>(new RemoveLoginViewModel { Provider = provider, ProviderKey = providerKey }, HttpContentType.FormUrlEncoded, "api/Account/RemoveLogin");
            return RedirectToAction(nameof(ManageLogins), new { message = result.Message });
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            if (SecurityOptions.Service == "AspnetIdentity")
            {
                var result = await HttpClientHelper.PostAsync<dynamic>(null, HttpContentType.FormUrlEncoded, "api/Account/Logout");
                if (!result.Success)
                    return View(nameof(Error));

                await HttpClientHelper.SetBearerToken(null);

                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (SecurityOptions.Service == "SingleSignOn")
            {
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                authenticationManager.SignOut("Cookies", "oidc");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else return null; // Implement others if required.
        }

        //
        // GET: /Account/Error
        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    if (_userManager != null)
            //    {
            //        _userManager.Dispose();
            //        _userManager = null;
            //    }

            //    if (_signInManager != null)
            //    {
            //        _signInManager.Dispose();
            //        _signInManager = null;
            //    }
            //}

            //base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}