using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConferencePlanner.BackEnd
{
    [Route("/api/[controller]")]
    public class AttendeesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AttendeesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet("@me")]
        public async Task<IActionResult> GetMe()
        {
            var (attendee, _) = await GetAttendeeForCurrentUserAsync();
            if (attendee == null)
            {
                return NotFound();
            }
            else
            {
                var result = attendee.MapAttendeeResponse();
                return Ok(result);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Attendee attendee)
        {
            // Check if there is already an attendee for this user
            var (dbAttendee, objectId) = await GetAttendeeForCurrentUserAsync();
            if (dbAttendee == null)
            {
                dbAttendee = new Attendee()
                {
                    DirectoryObjectId = objectId
                };
                _db.Attendees.Add(dbAttendee);
            }

            dbAttendee.UserName = attendee.UserName;
            dbAttendee.FirstName = attendee.FirstName;
            dbAttendee.LastName = attendee.LastName;
            await _db.SaveChangesAsync();

            return Ok(dbAttendee.MapAttendeeResponse());
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var attendee = await _db.Attendees.Include(a => a.SessionsAttendees)
                                                .ThenInclude(sa => sa.Session)
                                              .SingleOrDefaultAsync(a => a.UserName == username);

            if (attendee == null)
            {
                return NotFound();
            }

            var result = attendee.MapAttendeeResponse();

            return Ok(result);
        }

        [HttpPost("{username}/sessions/{sessionId:int}")]
        public async Task<IActionResult> AddSession(string username, int sessionId)
        {
            var attendee = await _db.Attendees.Include(a => a.SessionsAttendees)
                                                .ThenInclude(sa => sa.Session)
                                              .Include(a => a.ConferenceAttendees)
                                                .ThenInclude(ca => ca.Conference)
                                              .SingleOrDefaultAsync(a => a.UserName == username);

            if (attendee == null)
            {
                return NotFound();
            }

            var session = await _db.Sessions.FindAsync(sessionId);

            if (session == null)
            {
                return BadRequest();
            }

            attendee.SessionsAttendees.Add(new SessionAttendee
            {
                AttendeeID = attendee.ID,
                SessionID = sessionId
            });

            await _db.SaveChangesAsync();

            var result = attendee.MapAttendeeResponse();

            return Ok(result);
        }

        [HttpDelete("{username}/sessions/{sessionId:int}")]
        public async Task<IActionResult> RemoveSession(string username, int sessionId)
        {
            var attendee = await _db.Attendees.Include(a => a.SessionsAttendees)
                                              .SingleOrDefaultAsync(a => a.UserName == username);

            if (attendee == null)
            {
                return NotFound();
            }

            var session = await _db.Sessions.FindAsync(sessionId);

            if (session == null)
            {
                return BadRequest();
            }

            var sessionAttendee = attendee.SessionsAttendees.FirstOrDefault(sa => sa.SessionID == sessionId);
            attendee.SessionsAttendees.Remove(sessionAttendee);

            await _db.SaveChangesAsync();

            return NoContent();
        }

        private async Task<(Attendee attendee, string objectId)> GetAttendeeForCurrentUserAsync()
        {
            var objectId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            Debug.Assert(objectId != null);

            var attendee = await _db.Attendees.FirstOrDefaultAsync(a => a.DirectoryObjectId == objectId);
            return (attendee, objectId);
        }
    }
}
