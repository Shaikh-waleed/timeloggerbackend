using TimeLogger.Client.Mvc.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeLogger.Client.Mvc.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserId(this IPrincipal principal)
        {
            var identity = (ClaimsPrincipal)principal;
            return identity.Claims
                           .FirstOrDefault(c => c.Type == ClaimTypes.Sid)?
                           .Value;
        }

        public static string GetName(this IPrincipal principal)
        {
            var identity = (ClaimsPrincipal)principal;
            return identity.Claims
                           .FirstOrDefault(c => c.Type == ClaimTypes.Name)?
                           .Value;
        }

        public static string GetEmail(this IPrincipal principal)
        {
            var identity = (ClaimsPrincipal)principal;
            return identity.Claims
                           .FirstOrDefault(c => c.Type == ClaimTypes.Email)?
                           .Value;
        }

        public static string GetFullName(this IPrincipal principal)
        {
            var identity = (ClaimsPrincipal)principal;
            var firstName = identity.Claims
                                    .FirstOrDefault(c => c.Type == CustomClaimTypes.FirstName.ToString())?
                                    .Value
                                        ?? identity.Claims
                                                   .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                                                   .Value;

            var lastName = identity.Claims
                                   .FirstOrDefault(c => c.Type == CustomClaimTypes.LastName.ToString())?
                                   .Value
                                        ?? identity.Claims
                                                   .FirstOrDefault(c => c.Type == CustomClaimTypes.FirstName.ToString())?
                                                   .Value;

            return $"{firstName} {lastName}";
        }

        public static string GetFistName(this IPrincipal principal)
        {
            var identity = (ClaimsPrincipal)principal;
            return identity.Claims
                           .FirstOrDefault(c => c.Type == CustomClaimTypes.FirstName.ToString())?
                           .Value
                                ?? identity.Claims
                                           .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                                           .Value;
        }

        public static string GetLastName(this IPrincipal principal)
        {
            var identity = (ClaimsPrincipal)principal;
            return identity.Claims
                           .FirstOrDefault(c => c.Type == CustomClaimTypes.LastName.ToString())?
                           .Value
                                ?? identity.Claims
                                           .FirstOrDefault(c => c.Type == ClaimTypes.Surname)?
                                           .Value;
        }

        //public static DateTime? GetTokenIssuedDate(this IPrincipal principal)
        //{
        //    var identity = (ClaimsPrincipal)principal;
        //    var iatToken = identity.Claims
        //                           .Where(c => c.Type == "iat")
        //                           .Select(c => c.Value)
        //                           .SingleOrDefault();

        //    if (string.IsNullOrWhiteSpace(iatToken))
        //        return null;

        //    if (long.TryParse(iatToken, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out long seconds))
        //        return DateTime.UnixEpoch.AddSeconds(seconds);

        //    return null;
        //}
    }
}
