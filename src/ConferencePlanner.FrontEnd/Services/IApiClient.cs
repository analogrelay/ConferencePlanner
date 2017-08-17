using System.Collections.Generic;
using System.Threading.Tasks;
using ConferencePlanner.Models;

namespace ConferencePlanner.FrontEnd.Services
{
    public interface IApiClient
    {
        // TODO: Consider retrieving the access token somehow?
        Task<AttendeeResponse> GetMeAsync(string accessToken);
        Task<AttendeeResponse> AddAttendeeAsync(Attendee attendee, string accessToken);

        Task<List<SessionResponse>> GetSessionsByAttendeeAsync(string name);
        Task<List<SessionResponse>> GetSessionsAsync();
        Task<SessionResponse> GetSessionAsync(int id);
        Task<List<SpeakerResponse>> GetSpeakersAsync();
        Task<SpeakerResponse> GetSpeakerAsync(int id);
        Task PutSessionAsync(Session session);
        Task<List<SearchResult>> SearchAsync(string query);
        Task<AttendeeResponse> GetAttendeeAsync(string name);
        Task DeleteSessionAsync(int id);
        Task AddSessionToAttendeeAsync(string name, int sessionId);
        Task RemoveSessionFromAttendeeAsync(string name, int sessionId);
    }
}
