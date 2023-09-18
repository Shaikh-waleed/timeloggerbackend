using IdentityModel;
using TimeLogger.Client.NetCore.Utilities.Enums;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Client.NetCore.Helpers
{
    public static class AuthenticationHelper
    {
        public static bool TryGetToken(string authHeader, out string token)
        {
            token = string.Empty;
            if (authHeader != null && authHeader.Contains("Bearer"))
            {
                token = authHeader.Replace("Bearer ", string.Empty);
            }
            return !string.IsNullOrWhiteSpace(token);
        }

        /// <summary>
        /// Extracts list of roles from the access token. The code assumes that roles are returned as part of the token.
        /// </summary>
        /// <param name="accessToken">provider generate access token.</param>
        /// <returns>List of roles.</returns>
        /// 
        public static IEnumerable<string> GetRoles(JwtSecurityToken accessToken)
        {
            var roles = Enumerable.FirstOrDefault<Claim>(accessToken?.Claims, c => c.Type == CustomClaimTypes.Roles.ToString());
            return roles != null ? roles.Value.Split(',') : Array.Empty<string>();
        }

        public static IEnumerable<string> GetRoles(string accessToken)
        {
            var claims = GetUserClaims(accessToken);
            var roles = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Role) ?? claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Roles.ToString());

            return roles != null
                    ? roles.Value.Split(',')
                    : Array.Empty<string>();
        }

        /// <summary>
        /// Indicates whether this user is admin.
        /// </summary>
        /// <returns>True if admin.</returns>
        public static bool IsAdmin(string accessToken)
        {
            var roles = GetRoles(accessToken);
            return roles.Any(x => x.Equals("Admin"));
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
        public static IEnumerable<Claim> GetUserClaims(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return claims;
        }

        public static bool HasClaim(string token, Expression<Func<Claim, bool>> expression)
        {
            var claims = GetUserClaims(token).AsQueryable();
            var hasClaims = claims.Any(expression);
            return hasClaims;
        }

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
