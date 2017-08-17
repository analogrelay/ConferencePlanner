using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.Models
{
    public class Conference
    {
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }
    }
}
