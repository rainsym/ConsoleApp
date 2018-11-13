using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Microservice.Common.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public const string USER_ID_CLAIM_TYPE = "USER:ID";

        public static int? GetAccountId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                return null;
            }

            if (user.Claims == null)
            {
                return null;
            }

            string userId = user.FindFirstValue(USER_ID_CLAIM_TYPE);

            if (int.TryParse(userId, out int id))
            {
                return id;
            }
            else
            {
                return null;
            }
        }
    }
}
