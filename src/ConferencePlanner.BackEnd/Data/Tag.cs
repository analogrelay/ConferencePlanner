using System.Collections.Generic;

namespace ConferencePlanner.BackEnd.Data
{
    public class Tag : ConferencePlanner.Models.Tag
    {
        public virtual ICollection<SessionTag> SessionTags { get; set; }
    }
}
