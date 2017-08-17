using System.Collections.Generic;

namespace ConferencePlanner.Models
{
    public class TagResponse : Tag
    {
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
