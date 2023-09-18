using TimeLogger.Client.NetCore.Helpers;
using TimeLogger.Client.NetCore.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TimeLogger.Client.NetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfiguration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfiguration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Initializing Configurations
            services.InitializeConfigurations(Configuration);

            var securityOptions = services.BuildServiceProvider().GetService<Microsoft.Extensions.Options.IOptionsSnapshot<SecurityOptions>>()?.Value;
            if (securityOptions.Service == "AspnetIdentity")
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login/");
                    options.Events = new CookieAuthenticationEvents()
                    {
                        OnValidatePrincipal = async (context) =>
                        {
                            var accessToken = context.Principal.FindFirst(ClaimTypes.PrimarySid)?.Value;
                            if (!string.IsNullOrWhiteSpace(accessToken))
                                await HttpClientHelper.SetBearerToken(accessToken);
                            else
                                context.RejectPrincipal();
                        },
                        //OnSigningIn = async (context) =>
                        //{
                        //    System.Security.Claims.ClaimsIdentity identity = (ClaimsIdentity)context.Principal.Identity;
                        //    identity.AddClaim(new Claim("<claim name>", value));
                        //}
                    };
                })
                .AddCookie(IdentityConstants.ExternalScheme, options =>
                {
                    options.Cookie.Name = IdentityConstants.ExternalScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login/");
                })
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = "68669dee-ad51-4ab0-8a8f-16f456a05917";
                    microsoftOptions.ClientSecret = "xwaxyXEPRO726#@}icBG05@";
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "434467402013-4ehq09dvqp7qu57jucr1rra56fs0glcv.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "k4kvo8ckstA6u1Da5Skkiqaj";
                });
            }
            else if (securityOptions.Service == "SingleSignOn")
            {
                var authenticationBuilder = services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";

                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Cookie.Name = "CoreClient";
                    })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = securityOptions.IdentityServerUrl;
                    options.RequireHttpsMetadata = securityOptions.RequireHttpsMetadata;
                    options.ClientId = securityOptions.ClientId;
                    options.ClientSecret = securityOptions.ClientSecret;
                    options.ResponseType = securityOptions.OidcResponseType;
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Clear();
                    foreach (var scope in securityOptions.Scopes)
                    {
                        options.Scope.Add(scope);
                    }
                    options.ClaimActions.MapJsonKey(securityOptions.TokenValidationClaimRole, securityOptions.TokenValidationClaimRole, securityOptions.TokenValidationClaimRole);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = securityOptions.TokenValidationClaimName,
                        RoleClaimType = securityOptions.TokenValidationClaimRole
                    };

                    options.Events = new OpenIdConnectEvents
                    {
                        //OnMessageReceived = context =>
                        //{
                        //    return Task.FromResult(0);
                        //},
                        //OnRedirectToIdentityProvider = context =>
                        //{

                        //},
                        OnAuthorizationCodeReceived = async (context) =>
                        {
                            await Task.FromResult(0);
                        },
                        OnTokenResponseReceived = async (context) =>
                        {
                            var accessToken = context.TokenEndpointResponse.AccessToken;
                            if (!string.IsNullOrWhiteSpace(accessToken))
                                await HttpClientHelper.SetBearerToken(accessToken);

                            await Task.FromResult(0);
                        },
                        OnTokenValidated = async (context) =>
                        {
                            if (context.SecurityToken is System.IdentityModel.Tokens.Jwt.JwtSecurityToken token)
                            {
                                var accessToken = context.TokenEndpointResponse.AccessToken;
                                if (!string.IsNullOrWhiteSpace(accessToken))
                                    await HttpClientHelper.SetBearerToken(accessToken);
                            }
                            await Task.FromResult(0);
                        }
                    };
                });
            }
            else
            {
                // Implement others if required.
            }

            services.AddControllersWithViews();
            services.AddMvcCore();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
