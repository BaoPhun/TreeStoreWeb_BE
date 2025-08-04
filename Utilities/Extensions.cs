using Scrypt;
using System.Security.Claims;
using System.Security.Principal;

namespace TreeStore.Utilities
{
    public static class Extensions
    {
        public static string ToScryptEncode(this string encode)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            string hashsedPassword = encoder.Encode(encode);
            return hashsedPassword;
        }
        public static bool IsEqualPassword(this string hashedPassword, string password)
        {
            ScryptEncoder encoder = new ScryptEncoder();
            bool areEquals = encoder.Compare(password, hashedPassword);
            return areEquals;
        }
        public static int GetUserId(this IPrincipal User)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                // the principal identity is a claims identity.
                // now we need to find the NameIdentifier claim
                var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                return Convert.ToInt32(userIdClaim?.Value ?? "-1");
            }
            return -1;
        }
    }
}
