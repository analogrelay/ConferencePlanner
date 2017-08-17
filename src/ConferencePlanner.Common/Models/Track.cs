using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.Models
{
    public class Track
    {
        public int TrackID { get; set; }

        [Required]
        public int ConferenceID { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }
    }
}
