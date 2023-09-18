using IdentityModel;
using TimeLogger.Infrastructure.Utility.Constants;
using TimeLogger.Infrastructure.Utility.Enums;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    public static class AuthenticationHelper
    {
        public static bool TryGetToken(string authHeader, out string accessToken)
        {
            accessToken = string.Empty;
            if (authHeader != null && authHeader.Contains("Bearer"))
            {
                accessToken = authHeader.Replace("Bearer ", string.Empty);
            }
            return !string.IsNullOrWhiteSpace(accessToken);
        }

        public static string GetUserId(string accessToken)
        {
            var claims = GetUserClaims(accessToken);
            return claims.FirstOrDefault(c => c.Type == CustomClaims.Id)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)?
                                     .Value;
        }

        public static string GetUserId(JwtSecurityToken accessToken)
        {
            var claims = accessToken?.Claims;
            return claims.FirstOrDefault(c => c.Type == CustomClaims.Id)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)?
                                     .Value;
        }

        public static string GetUserName(string accessToken)
        {
            var claims = GetUserClaims(accessToken);
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?
                                     .Value;
        }

        public static string GetUserName(JwtSecurityToken accessToken)
        {
            var claims = accessToken?.Claims;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?
                                     .Value;
        }

        public static string GetUserEmail(string accessToken)
        {
            var claims = GetUserClaims(accessToken);
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?
                                     .Value;
        }

        public static string GetUserEmail(JwtSecurityToken accessToken)
        {
            var claims = accessToken?.Claims;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email)?
                                     .Value;
        }

        public static string GetUserFullName(string accessToken)
        {
            var firstName = GetUserFistName(accessToken);
            var lastName = GetUserLastName(accessToken);

            return $"{firstName} {lastName}";
        }

        public static string GetUserFullName(JwtSecurityToken accessToken)
        {
            var firstName = GetUserFistName(accessToken);
            var lastName = GetUserLastName(accessToken);

            return $"{firstName} {lastName}";
        }

        public static string GetUserFistName(string accessToken)
        {
            var claims = GetUserClaims(accessToken);
            return claims.FirstOrDefault(c => c.Type == CustomClaims.FirstName)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                                     .Value
                                        ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName)?
                                                 .Value;
        }

        public static string GetUserFistName(JwtSecurityToken accessToken)
        {
            var claims = accessToken?.Claims;
            return claims.FirstOrDefault(c => c.Type == CustomClaims.FirstName)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                                     .Value
                                        ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName)?
                                                 .Value;
        }

        public static string GetUserLastName(string accessToken)
        {
            var claims = GetUserClaims(accessToken);
            return claims.FirstOrDefault(c => c.Type == CustomClaims.LastName)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?
                                     .Value
                                        ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?
                                                 .Value;
        }

        public static string GetUserLastName(JwtSecurityToken accessToken)
        {
            var claims = accessToken?.Claims;
            return claims.FirstOrDefault(c => c.Type == CustomClaims.LastName)?
                         .Value
                            ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?
                                     .Value
                                        ?? claims.FirstOrDefault(c => c.Type == JwtClaimTypes.FamilyName)?
                                                 .Value;
        }

        public static IEnumerable<string> GetUserRoles(string accessToken)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var roles = Enumerable.FirstOrDefault<Claim>(token?.Claims, c => c.Type == JwtClaimTypes.Role) ?? Enumerable.FirstOrDefault<Claim>(token?.Claims, c => c.Type == CustomClaimTypes.Roles.ToString());

            return roles != null
                    ? roles.Value.Split(',')
                    : Array.Empty<string>();
        }
        public static IEnumerable<string> GetUserRoles(JwtSecurityToken accessToken)
        {
            var roles = Enumerable.FirstOrDefault<Claim>(accessToken?.Claims, c => c.Type == CustomClaimTypes.Roles.ToString());
            return roles != null ? roles.Value.Split(',') : Array.Empty<string>();
        }

        public static IEnumerable<Claim> GetUserClaims(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return claims;
        }

        public static bool HasUserClaim(string token, Expression<Func<Claim, bool>> expression)
        {
            var claims = GetUserClaims(token).AsQueryable();
            var hasClaims = claims.Any(expression);
            return hasClaims;
        }

        public static bool IsUserAdmin(string accessToken)
        {
            var roles = GetUserRoles(accessToken);
            return roles.Any(x => x.Equals(UserRoles.Admin.ToString()));
        }

        public static bool IsUserAdmin(JwtSecurityToken accessToken)
        {
            var roles = GetUserRoles(accessToken);
            return roles.Any(x => x.Equals(UserRoles.Admin.ToString()));
        }

        public static DateTime? GetTokenIssuedDate(string accessToken)
        {
            var claims = GetUserClaims(accessToken);
            var iatToken = claims.FirstOrDefault(c => c.Type == "iat")?
                                  .Value;

            if (string.IsNullOrWhiteSpace(iatToken))
                return null;

            if (long.TryParse(iatToken, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out long seconds))
                return DateTime.UnixEpoch.AddSeconds(seconds);

            return null;
        }

        /// <summary>
        /// Derived from the unlisted nuget package: https://github.com/auth0/auth0-aspnet-owin/tree/master/src/Auth0.Owin.OpenIdConnectSigningKeyResolver
        /// This only retrieves the JWKS and does not pose a security issue.
        /// </summary>
        /// <param name="authority">Auth0 authority.</param>
        /// <param name="kid">Key Id.</param>
        /// <returns>The signing key.</returns>
        public static SecurityKey[] GetSigningKey(string authority, string kid)
        {
            var cm = new ConfigurationManager<OpenIdConnectConfiguration>($"{authority.TrimEnd('/')}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            var taskFactory = new TaskFactory(System.Threading.CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
            var openIdConfig = taskFactory.StartNew(async () => await cm.GetConfigurationAsync()).Unwrap().GetAwaiter().GetResult();
            return new[] { openIdConfig.JsonWebKeySet.GetSigningKeys().FirstOrDefault(t => t.KeyId == kid) };
        }

        /// <summary>
        /// Converts the json into a collection of claims.
        /// </summary>
        /// <param name="json">Json data.</param>
        /// <returns>A collection of claims.</returns>

        /// <summary>
        /// Read the claim value from JwtSecurityToken.
        /// </summary>
        /// <param name="token">JwtSecurityToken.</param>
        /// <param name="claimName">ClaimName.</param>
        /// <returns>Claim value.</returns>
        internal static string ReadTokenClaim(this JwtSecurityToken token, string claimName)
        {
            return token?.Claims?.FirstOrDefault(c => c.Type == claimName)?.Value;
        }
    }
}
