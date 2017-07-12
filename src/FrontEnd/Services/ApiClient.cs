using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ConferenceDTO;
using FrontEnd.Infrastructure;

namespace FrontEnd.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AttendeeResponse> GetMeAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/attendees/@me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                // The user hasn't been registered. Return null in that case
                return null;
            }
            else
            {
                // Any other error is unexpected
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsJsonAsync<AttendeeResponse>();
            }
        }

        public async Task<AttendeeResponse> AddAttendeeAsync(Attendee attendee, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/attendees");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendJsonAsync(request, attendee);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonAsync<AttendeeResponse>();
        }

        public async Task<AttendeeResponse> GetAttendeeAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var response = await _httpClient.GetAsync($"/api/attendees/{name}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonAsync<AttendeeResponse>();
        }

        public async Task<SessionResponse> GetSessionAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/sessions/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonAsync<SessionResponse>();
        }

        public async Task<List<SessionResponse>> GetSessionsAsync()
        {
            var response = await _httpClient.GetAsync("/api/sessions");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonAsync<List<SessionResponse>>();
        }

        public async Task DeleteSessionAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/sessions/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }

        public async Task<SpeakerResponse> GetSpeakerAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/speakers/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonAsync<SpeakerResponse>();
        }

        public async Task<List<SpeakerResponse>> GetSpeakersAsync()
        {
            var response = await _httpClient.GetAsync("/api/speakers");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonAsync<List<SpeakerResponse>>();
        }

        public async Task PutSessionAsync(Session session)
        {
            var response = await _httpClient.PutJsonAsync($"/api/sessions/{session.ID}", session);

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<SearchResult>> SearchAsync(string query)
        {
            var term = new SearchTerm
            {
                Query = query
            };

            var response = await _httpClient.PostJsonAsync($"/api/search", term);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsJsonAsync<List<SearchResult>>();
        }

        public async Task AddSessionToAttendeeAsync(string name, int sessionId)
        {
            var response = await _httpClient.PostAsync($"/api/attendees/{name}/sessions/{sessionId}", null);

            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveSessionFromAttendeeAsync(string name, int sessionId)
        {
            var response = await _httpClient.DeleteAsync($"/api/attendees/{name}/sessions/{sessionId}");

            response.EnsureSuccessStatusCode();
        }

        public async Task<List<SessionResponse>> GetSessionsByAttendeeAsync(string name)
        {
            // TODO: Add backend API for this
            var sessionsTask = GetSessionsAsync();
            var attendeeTask = GetAttendeeAsync(name);

            await Task.WhenAll(sessionsTask, attendeeTask);

            var sessions = await sessionsTask;
            var attendee = await attendeeTask;

            if (attendee == null)
            {
                return new List<SessionResponse>();
            }

            var sessionIds = attendee.Sessions.Select(s => s.ID);

            sessions.RemoveAll(s => !sessionIds.Contains(s.ID));

            return sessions;
        }
    }
}
