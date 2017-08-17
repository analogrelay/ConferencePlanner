using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.BackEnd.Data
{
    public class Track : ConferencePlanner.Models.Track
    {
        [Required]
        public Conference Conference { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
