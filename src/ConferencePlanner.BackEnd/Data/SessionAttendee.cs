namespace ConferencePlanner.BackEnd.Data
{
    public class SessionAttendee
    {
        public int SessionID { get; set; }

        public Session Session { get; set; }

        public int AttendeeID { get; set; }

        public Attendee Attendee { get; set; }
    }
}
