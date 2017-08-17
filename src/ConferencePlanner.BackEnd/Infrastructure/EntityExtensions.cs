using System.Linq;

namespace ConferencePlanner.BackEnd.Data
{
    public static class EntityExtensions
    {
        public static ConferencePlanner.Models.SessionResponse MapSessionResponse(this Session session) =>
            new ConferencePlanner.Models.SessionResponse
            {
                ID = session.ID,
                Title = session.Title,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Tags = session.SessionTags?
                              .Select(st => new ConferencePlanner.Models.Tag
                              {
                                  ID = st.TagID,
                                  Name = st.Tag.Name
                              })
                               .ToList(),
                Speakers = session.SessionSpeakers?
                                  .Select(ss => new ConferencePlanner.Models.Speaker
                                  {
                                      ID = ss.SpeakerId,
                                      Name = ss.Speaker.Name
                                  })
                                   .ToList(),
                TrackId = session.TrackId,
                Track = new ConferencePlanner.Models.Track
                {
                    TrackID = session?.TrackId ?? 0,
                    Name = session.Track?.Name
                },
                ConferenceID = session.ConferenceID,
                Abstract = session.Abstract
            };

        public static ConferencePlanner.Models.SpeakerResponse MapSpeakerResponse(this Speaker speaker) =>
            new ConferencePlanner.Models.SpeakerResponse
            {
                ID = speaker.ID,
                Name = speaker.Name,
                Bio = speaker.Bio,
                WebSite = speaker.WebSite,
                Sessions = speaker.SessionSpeakers?
                    .Select(ss =>
                        new ConferencePlanner.Models.Session
                        {
                            ID = ss.SessionId,
                            Title = ss.Session.Title
                        })
                    .ToList()
            };

        public static ConferencePlanner.Models.AttendeeResponse MapAttendeeResponse(this Attendee attendee) =>
            new ConferencePlanner.Models.AttendeeResponse
            {
                ID = attendee.ID,
                FirstName = attendee.FirstName,
                LastName = attendee.LastName,
                UserName = attendee.UserName,
                Sessions = attendee.SessionsAttendees?
                    .Select(sa =>
                        new ConferencePlanner.Models.Session
                        {
                            ID = sa.SessionID,
                            Title = sa.Session.Title,
                            StartTime = sa.Session.StartTime,
                            EndTime = sa.Session.EndTime
                        })
                    .ToList(),
                Conferences = attendee.ConferenceAttendees?
                    .Select(ca =>
                        new ConferencePlanner.Models.Conference
                        {
                            ID = ca.ConferenceID,
                            Name = ca.Conference.Name
                        })
                    .ToList(),
            };
    }
}
