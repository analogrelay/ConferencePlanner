using System.Linq;
using System.Threading.Tasks;
using BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd
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
            Attendee attendee = await AttendeeHelper.GetAndUpdateAttendeeAsync(_db, User);

            var result = attendee.MapAttendeeResponse();

            return Ok(result);
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
    }
}
