using IdentityModel;
using TimeLogger.Infrastructure.Utility.Constants;
using TimeLogger.Infrastructure.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Extensions
{
    public static class UserExtensions
    {
        public static string GetId(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?
                            .Value
                                ?? principal.Claims
                                            .FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)?
                                            .Value;
        }

        public static string GetName(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?
                            .Value
                                ?? principal.Claims
                                            .FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?
                                            .Value;
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?
                            .Value
                                ?? principal.Claims
                                            .FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?
                                            .Value;
        }

        public static string GetFullName(this ClaimsPrincipal principal)
        {
            var firstName = principal.Claims
                                     .FirstOrDefault(c => c.Type == CustomClaims.FirstName)?
                                     .Value
                                        ?? principal.Claims
                                                    .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                                                    .Value
                                                        ?? principal.Claims
                                                                    .FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName)?
                                                                    .Value;

            var lastName = principal.Claims
                                    .FirstOrDefault(c => c.Type == CustomClaims.LastName)?
                                    .Value
                                        ?? principal.Claims
                                                    .FirstOrDefault(c => c.Type == ClaimTypes.Surname)?
                                                    .Value
                                                        ?? principal.Claims
                                                                    .FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?
                                                                    .Value;

            return $"{firstName} {lastName}";
        }

        public static string GetFistName(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == CustomClaims.FirstName)?
                            .Value
                                ?? principal.Claims
                                            .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                                            .Value
                                                ?? principal.Claims
                                                            .FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName)?
                                                            .Value;
        }

        public static string GetLastName(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == CustomClaims.LastName)?
                            .Value
                                ?? principal.Claims
                                            .FirstOrDefault(c => c.Type == ClaimTypes.Surname)?
                                            .Value
                                                ?? principal.Claims
                                                            .FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?
                                                            .Value;
        }

        public static DateTime? GetTokenIssuedDate(this ClaimsPrincipal principal)
        {
            var iatToken = principal.Claims
                                    .FirstOrDefault(c => c.Type == "iat")?
                                    .Value;

            if (string.IsNullOrWhiteSpace(iatToken))
                return null;

            if (long.TryParse(iatToken, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out long seconds))
                return DateTime.UnixEpoch.AddSeconds(seconds);

            return null;
        }
    }
}
