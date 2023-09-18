using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Models.Configuration
{
    public class SecurityOptions
    {
        public SecurityOptions()
        {
        }

        public static string AccessToken { get; set; }
        public int PasswordLength { get; set; }
        public int AccountLockoutTimeSpan { get; set; }
        public int AccountLoginMaximumAttempts { get; set; }
        public int PreviousPasswordValidationLimit { get; set; }
        public string Authority { get; set; }
        public string RequiredScope { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthenticatorUriFormat { get; set; }
        public int NumberOfRecoveryCodes { get; set; }
        public bool RequireConfirmedAccount { get; set; }
        public string Scopes { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public int EncryptionIterationSize { get; set; }
        public string EncryptionPassword { get; set; }
        public string EncryptionSaltKey { get; set; }
        public string EncryptionVIKey { get; set; }
        public bool MicrosoftAuthenticationAdded { get; set; }
        public bool GoogleAuthenticationAdded { get; set; }
        public bool TwitterAuthenticationAdded { get; set; }
        public bool FacebookAuthenticationAdded { get; set; }
    }
}
