using TimeLogger.Client.Mvc.Helpers;
using TimeLogger.Client.Mvc.Options;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TimeLogger.Client.Mvc
{
	public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            if (SecurityOptions.Service == "AspnetIdentity")
            {
                // Enable the application to use a cookie to store information for the signed in user
                // and to use a cookie to temporarily store information about a user logging in with a third party login provider
                // Configure the sign in cookie
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    CookieName = "TimeLoggerCookie",
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Account/Login"),
                    Provider = new CookieAuthenticationProvider
                    {
                        // Enables the application to validate the security stamp when the user logs in.
                        OnValidateIdentity = async (context) =>
                        {
                            var accessToken = context.Identity.FindFirst(ClaimTypes.PrimarySid)?.Value;
                            if (!string.IsNullOrWhiteSpace(accessToken))
                                await HttpClientHelper.SetBearerToken(accessToken);
                            else
                                context.RejectIdentity();
                        }
                    }
                });

                app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

                // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
                app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

                // Enables the application to remember the second login verification factor such as phone or email.
                // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
                // This is similar to the RememberMe option when you log in.
                app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

                app.UseMicrosoftAccountAuthentication(
                    clientId: "68669dee-ad51-4ab0-8a8f-16f456a05917",
                    clientSecret: "xwaxyXEPRO726#@}icBG05@"
                );

                app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
                {
                    ClientId = "434467402013-4ehq09dvqp7qu57jucr1rra56fs0glcv.apps.googleusercontent.com",
                    ClientSecret = "k4kvo8ckstA6u1Da5Skkiqaj"
                });

                // Todo:
                //app.UseFacebookAuthentication(new FacebookAuthenticationOptions
                //{
                //    AppId = _config.CommonSecrets.FacebookAppId,
                //    AppSecret = _config.CommonSecrets.FacebookAppSecret,
                //    Provider = new FacebookAuthenticationProvider
                //    {
                //        OnAuthenticated = (context) =>
                //        {
                //            context.Identity.AddClaim(new Claim("FacebookToken", context.AccessToken));
                //            return Task.FromResult(0);
                //        }
                //    }
                //});


                //app.UseTwitterAuthentication(
                //   consumerKey: "",
                //   consumerSecret: "");

                //app.UseFacebookAuthentication(
                //   appId: "",
                //   appSecret: "");
            }
            else if (SecurityOptions.Service == "SingleSignOn")
            {
                app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
                var authenticationBuilder = app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                    CookieName = "MvcClient"
                })
                .UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    Authority = SecurityOptions.IdentityServerUrl,
                    RedirectUri = SecurityOptions.RedirectUri,
                    RequireHttpsMetadata = SecurityOptions.RequireHttpsMetadata,
                    ClientId = SecurityOptions.ClientId,
                    ClientSecret = SecurityOptions.ClientSecret,
                    ResponseType = SecurityOptions.OidcResponseType,
                    SaveTokens = true,
                    //GetClaimsFromUserInfoEndpoint = true;
                    Scope = SecurityOptions.Scopes,
                    SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                    //ClaimActions.MapJsonKey(SecurityOptions.TokenValidationClaimRole, SecurityOptions.TokenValidationClaimRole, SecurityOptions.TokenValidationClaimRole);

                    TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = SecurityOptions.TokenValidationClaimName,
                        RoleClaimType = SecurityOptions.TokenValidationClaimRole
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        //RedirectToIdentityProvider = async (context) =>
                        //{
                        //    await Task.CompletedTask;
                        //},
                        AuthorizationCodeReceived = async (context) =>
                        {
                            await Task.CompletedTask;
                        },
                        TokenResponseReceived = async (context) =>
                        {
                            //var accessToken = context.TokenEndpointResponse.AccessToken;
                            //if (!string.IsNullOrWhiteSpace(accessToken))
                            //    await HttpClientHelper.SetBearerToken(accessToken);

                            await Task.FromResult(0);
                        },
                        SecurityTokenValidated = async (context) =>
                        {
                            //var accessToken = context.TokenEndpointResponse.AccessToken;
                            //if (!string.IsNullOrWhiteSpace(accessToken))
                            //    await HttpClientHelper.SetBearerToken(accessToken);

                            //var accessToken = context.Identity.FindFirst(ClaimTypes.PrimarySid)?.Value;
                            //if (!string.IsNullOrWhiteSpace(accessToken))
                            //    await HttpClientHelper.SetBearerToken(accessToken);
                            //else
                            //    context.RejectIdentity();

                            await Task.CompletedTask;
                        }
                    }
                });
            }
            else
            {
                // Implement others if required.
            }
        }
    }
}