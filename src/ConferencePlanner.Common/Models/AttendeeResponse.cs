using System.Collections.Generic;

namespace ConferencePlanner.Models
{
    public class AttendeeResponse : Attendee
    {
        public ICollection<Conference> Conferences { get; set; } = new List<Conference>();

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
