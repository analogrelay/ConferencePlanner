using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackEnd.Data;

namespace BackEnd
{
    public static class AttendeeHelper
    {
        public static async Task<Attendee> GetAndUpdateAttendeeAsync(ApplicationDbContext db, ClaimsPrincipal user)
        {
            // Get or create the Attendee record for the current user.
            var objectId = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            Debug.Assert(objectId != null);

            var attendee = db.Attendees.FirstOrDefault(a => a.DirectoryObjectId == objectId);

            var attendeeChanged = false;
            if (attendee == null)
            {
                attendee = new Attendee()
                {
                    DirectoryObjectId = objectId
                };
                db.Attendees.Add(attendee);
                attendeeChanged = true;
            }

            attendeeChanged = UpdateAttendeeFromClaims(attendee, user) || attendeeChanged;

            // Update the local copy from the claims if necessary.
            if (attendeeChanged)
            {
                await db.SaveChangesAsync();
            }

            return attendee;
        }

        private static bool UpdateAttendeeFromClaims(Attendee attendee, ClaimsPrincipal user)
        {
            var firstName = user.FindFirstValue(ClaimTypes.GivenName);
            var lastName = user.FindFirstValue(ClaimTypes.Surname);
            var userName = user.FindFirstValue("name");
            var emailAddress = user.FindFirstValue("emails");

            var changed = false;

            if(!string.Equals(attendee.FirstName, firstName))
            {
                attendee.FirstName = firstName;
                changed = true;
            }

            if(!string.Equals(attendee.LastName, lastName))
            {
                attendee.LastName = lastName;
                changed = true;
            }

            if(!string.Equals(attendee.UserName, userName))
            {
                attendee.UserName = userName;
                changed = true;
            }

            if(!string.Equals(attendee.EmailAddress, emailAddress))
            {
                attendee.EmailAddress = emailAddress;
                changed = true;
            }

            return changed;
        }
    }
}
