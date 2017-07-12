using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ConferenceDTO;

namespace FrontEnd.Authentication
{
    internal static class AttendeeClaimMapper
    {
        public static void UpdateClaims(ClaimsIdentity identity, Attendee attendee)
        {
            foreach (var claim in GetClaims(attendee))
            {
                var currentClaim = identity.Claims.FirstOrDefault(c => c.Type.Equals(claim.Type, StringComparison.Ordinal));
                if (currentClaim != null)
                {
                    identity.RemoveClaim(currentClaim);
                }
                identity.AddClaim(claim);
            }
        }

        public static IEnumerable<Claim> GetClaims(Attendee attendee)
        {
            yield return new Claim("attendeeId", attendee.ID.ToString());
            yield return new Claim(ClaimTypes.Name, attendee.UserName);
            yield return new Claim(ClaimTypes.GivenName, attendee.FirstName);
            yield return new Claim(ClaimTypes.Surname, attendee.LastName);
        }
    }
}
