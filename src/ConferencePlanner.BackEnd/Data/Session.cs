using System.Collections.Generic;

namespace ConferencePlanner.BackEnd.Data
{
    public class Session : ConferencePlanner.Models.Session
    {
        public Conference Conference { get; set; }

        public virtual ICollection<SessionSpeaker> SessionSpeakers { get; set; }

        public Track Track { get; set; }

        public virtual ICollection<SessionTag> SessionTags { get; set; }
    }
}
