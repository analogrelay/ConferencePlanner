using System.ComponentModel.DataAnnotations;

namespace ConferenceDTO
{
    public class Attendee
    {
        public int ID { get; set; }

        public string DirectoryObjectId { get; set; }

        [StringLength(200)]
        public virtual string FirstName { get; set; }

        [StringLength(200)]
        public virtual string LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string UserName { get; set; }

        [StringLength(256)]
        public virtual string EmailAddress { get; set; }
    }
}
