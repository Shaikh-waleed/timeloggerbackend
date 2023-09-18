using TimeLogger.Component.Interfaces.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using TimeLogger.Infrastructure.Models.Configuration;
using TimeLogger.Component.Security.Service;
using Microsoft.AspNetCore.HttpOverrides;
using System.Linq;

namespace TimeLogger.Component.Security
{
    public static class Securities
    {
        public static void RegisterServices(IServiceCollection services)
        {
            var componentOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<ComponentOptions>>()?.Value;
            var securityOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<SecurityOptions>>()?.Value;

            switch (componentOptions.Security.SecurityService)
            {
                case "AspnetIdentity":
                    services.AddTransient<ISecurityService, SecurityAspnetIdentity>();
                    services.AddScoped<ICompanyService, CompanyService>();
                    services.AddScoped<IAddressService, AddressService>();
                    services.AddScoped<ICityService, CityService>();
                    services.AddScoped<ICountryService, CountryService>();
                    break;
                case "SingleSignOn":
                    services.AddTransient<ISecurityService, SecuritySingleSignOn>();
                    break;
                default:
                    break;
            }

            switch (componentOptions.Security.EncryptionService)
            {
                case "AES":
                    services.AddTransient<IEncryptionService, EncryptionAES>();
                    break;
                default:
                    break;
            }

            AddAuthentication(services);

            if (componentOptions.Security.SecurityService == "AspnetIdentity")
            {
                if (securityOptions.MicrosoftAuthenticationAdded)
                    AddMicrosoftAuthentication(services);

                if (securityOptions.GoogleAuthenticationAdded)
                    AddGoogleAuthentication(services);

                if (securityOptions.TwitterAuthenticationAdded)
                    AddTwitterAuthentication(services);

                if (securityOptions.FacebookAuthenticationAdded)
                    AddFacebookAuthentication(services);
            }
        }

        private static void AddAuthentication(IServiceCollection services)
        {
            var timeLoggerOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<TimeLoggerOptions>>()?.Value;
            var componentOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<ComponentOptions>>()?.Value;
            var securityOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<SecurityOptions>>()?.Value;

            var key = Encoding.ASCII.GetBytes("Core.Secret@TimeLogger");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                if (componentOptions.Security.SecurityService == "SingleSignOn")
                {
                    options.Authority = timeLoggerOptions.IdentityServerUrl;
                    options.Audience = timeLoggerOptions.ApiName;
                }
                else if (componentOptions.Security.SecurityService == "AspnetIdentity")
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                        //ValidateIssuer = false,
                        //ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = timeLoggerOptions.ApiUrl,
                        ValidAudience = timeLoggerOptions.ApiUrl,
                    };

                //options.RequireHttpsMetadata = false;
                //options.SaveToken = true;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = async (context) =>
                    {
                        await Task.CompletedTask;
                    },
                    OnTokenValidated = async (context) =>
                    {
                        // Grab the raw value of the token, and store it as a claim so we can retrieve it again later in the request pipeline
                        if (context.SecurityToken is JwtSecurityToken token)
                        {
                            var accessToken = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").LastOrDefault();
                            if (!string.IsNullOrWhiteSpace(accessToken))
                                SecurityOptions.AccessToken = accessToken;
                        }

                        await Task.FromResult(0);
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var test = context.Response;
                        SecurityOptions.AccessToken = string.Empty;
                        return Task.FromResult(0);
                    },
                    OnForbidden = context =>
                    {
                        var test = context.Response;
                        return Task.FromResult(0);
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //AuthenticationSchemes = "Bearer"
            });
        }

        private static void AddMicrosoftAuthentication(IServiceCollection services)
        {
            var outlookOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<OutlookOptions>>();
            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = outlookOptions.Value.ApplicationId;
                microsoftOptions.ClientSecret = outlookOptions.Value.ApplicationSecret;
            });
        }

        private static void AddGoogleAuthentication(IServiceCollection services)
        {
            var googleOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<GoogleOptions>>();
            services.AddAuthentication().AddGoogle(googleAuthOptions =>
            {
                googleAuthOptions.ClientId = googleOptions.Value.ClientId;
                googleAuthOptions.ClientSecret = googleOptions.Value.ClientSecret;
            });
        }

        private static void AddTwitterAuthentication(IServiceCollection services)
        {
            var twiitterOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<TwitterOptions>>();
            services.AddAuthentication().AddTwitter(twiitterAuthOptions =>
            {
                twiitterAuthOptions.ConsumerKey = twiitterOptions.Value.ConsumerKey;
                twiitterAuthOptions.ConsumerSecret = twiitterOptions.Value.ConsumerSecret;
            });
        }

        private static void AddFacebookAuthentication(IServiceCollection services)
        {
            var facebookkOptions = services.BuildServiceProvider().GetService<IOptionsSnapshot<FacebookOptions>>();
            services.AddAuthentication().AddFacebook(facebookAuthOptions =>
            {
                facebookAuthOptions.ClientId = facebookkOptions.Value.AppId;
                facebookAuthOptions.ClientSecret = facebookkOptions.Value.AppSecret;
            });
        }

        public static void RegisterApps(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            AddForwardHeaders(app);
        }

        private static void AddForwardHeaders(IApplicationBuilder app)
        {
            var forwardingOptions = new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.All
            };

            forwardingOptions.KnownNetworks.Clear();
            forwardingOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardingOptions);
        }
    }
}
