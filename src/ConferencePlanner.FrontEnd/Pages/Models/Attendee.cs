using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.FrontEnd.Pages.Models
{
    public class Attendee : ConferencePlanner.Models.Attendee
    {
        [DisplayName("Username")]
        public override string UserName { get => base.UserName; set => base.UserName = value; }

        [DisplayName("First name")]
        public override string FirstName { get => base.FirstName; set => base.FirstName = value; }

        [DisplayName("Last name")]
        public override string LastName { get => base.LastName; set => base.LastName = value; }

        [DisplayName("Email address")]
        [DataType(DataType.EmailAddress)]
        public override string EmailAddress { get => base.EmailAddress; set => base.EmailAddress = value; }
    }
}
