using TimeLogger.Client.NetCore.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Client.NetCore.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?
                            .Value;
        }

        public static string GetName(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?
                            .Value;
        }

        public static string GetFullName(this ClaimsPrincipal principal)
        {
            var firstName = principal.Claims
                                     .FirstOrDefault(c => c.Type == CustomClaimTypes.FirstName.ToString())?
                                     .Value
                                        ?? principal.Claims
                                                    .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                                                    .Value;

            var lastName = principal.Claims
                                    .FirstOrDefault(c => c.Type == CustomClaimTypes.LastName.ToString())?
                                    .Value
                                        ?? principal.Claims
                                                    .FirstOrDefault(c => c.Type == ClaimTypes.Surname)?
                                                    .Value;

            return $"{firstName} {lastName}";
        }

        public static string GetFistName(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == CustomClaimTypes.FirstName.ToString())?
                            .Value 
                                ?? principal.Claims
                                            .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                                            .Value;
        }

        public static string GetLastName(this ClaimsPrincipal principal)
        {
            return principal.Claims
                            .FirstOrDefault(c => c.Type == CustomClaimTypes.LastName.ToString())?
                            .Value
                                ?? principal.Claims
                                            .FirstOrDefault(c => c.Type == ClaimTypes.Surname)?
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
