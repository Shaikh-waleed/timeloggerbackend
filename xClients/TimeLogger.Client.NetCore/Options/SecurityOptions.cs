namespace TimeLogger.Client.NetCore.Options
{
    public class SecurityOptions
    {
        public string Service { get; set; }
        public string IdentityServerUrl { get; set; }
        public string ProfileUri { get; set; }
        public string ProfileApi { get; set; }
        public string RedirectUri { get; set; }
        public string CookieName { get; set; }
        public double CookieExpiresUtcHours { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string TokenValidationClaimName { get; set; }
        public string TokenValidationClaimRole { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string OidcResponseType { get; set; }
        public string[] Scopes { get; set; }
    }
}
