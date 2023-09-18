using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TimeLogger.Client.Mvc.Options
{
    public static class SecurityOptions
    {
        public static string Service { get; }
        public static string IdentityServerUrl { get; }
        public static string ProfileUri { get; set; }
        public static string ProfileApi { get; set; }
        public static string RedirectUri { get; }
        public static string CookieName { get; }
        public static double CookieExpiresUtcHours { get; }
        public static bool RequireHttpsMetadata { get; }
        public static string TokenValidationClaimName { get; }
        public static string TokenValidationClaimRole { get; }
        public static string ClientId { get; }
        public static string ClientSecret { get; }
        public static string OidcResponseType { get; }
        public static string Scopes { get; }

        static SecurityOptions()
        {
            Service = ConfigurationManager.AppSettings["Service"];
            IdentityServerUrl = ConfigurationManager.AppSettings["IdentityServerUrl"];
            ProfileUri = ConfigurationManager.AppSettings["IdentityServerUrl"];
            ProfileApi = ConfigurationManager.AppSettings["IdentityServerUrl"];
            RedirectUri = ConfigurationManager.AppSettings["RedirectUri"];
            CookieName = ConfigurationManager.AppSettings["CookieName"];
            CookieExpiresUtcHours = double.Parse(ConfigurationManager.AppSettings["CookieExpiresUtcHours"]);
            RequireHttpsMetadata = bool.Parse(ConfigurationManager.AppSettings["RequireHttpsMetadata"]);
            TokenValidationClaimName = ConfigurationManager.AppSettings["TokenValidationClaimName"];
            TokenValidationClaimRole = ConfigurationManager.AppSettings["TokenValidationClaimRole"];
            ClientId = ConfigurationManager.AppSettings["ClientId"];
            ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            OidcResponseType = ConfigurationManager.AppSettings["OidcResponseType"];
            Scopes = ConfigurationManager.AppSettings["Scopes"];
        }
    }
}