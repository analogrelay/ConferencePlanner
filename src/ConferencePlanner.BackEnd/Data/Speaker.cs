using System.Collections.Generic;

namespace ConferencePlanner.BackEnd.Data
{
    public class Speaker : ConferencePlanner.Models.Speaker
    {
        public virtual ICollection<SessionSpeaker> SessionSpeakers { get; set; } = new List<SessionSpeaker>();
    }
}
