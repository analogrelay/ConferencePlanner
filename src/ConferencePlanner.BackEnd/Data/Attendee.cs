using System.Collections.Generic;

namespace ConferencePlanner.BackEnd.Data
{
    public class Attendee : ConferencePlanner.Models.Attendee
    {
        public virtual ICollection<ConferenceAttendee> ConferenceAttendees { get; set; }

        public virtual ICollection<SessionAttendee> SessionsAttendees { get; set; }
    }
}
