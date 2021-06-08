using System.Security.Claims;
using System.Security.Principal;

namespace Jewellis
{
    /// <summary>
    /// Represents extension methods for <see cref="IIdentity"/>.
    /// </summary>
    public static class IdentityExtensions
    {

        /// <summary>
        /// Returns the user id.
        /// </summary>
        /// <returns>Returns the user id.</returns>
        public static int? GetId(this IIdentity identity)
        {
            if (identity.IsAuthenticated)
            {
                ClaimsIdentity claims = (identity as ClaimsIdentity);
                Claim claim = claims.FindFirst(ClaimTypes.PrimarySid);
                int id;
                if (int.TryParse(claim.Value, out id))
                {
                    return id;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the user role name.
        /// </summary>
        /// <returns>Returns the user role name.</returns>
        public static string GetRole(this IIdentity identity)
        {
            if (identity.IsAuthenticated)
            {
                ClaimsIdentity claims = (identity as ClaimsIdentity);
                Claim claim = claims.FindFirst(ClaimTypes.Role);
                return claim.Value;
            }
            return null;
        }

    }
}
