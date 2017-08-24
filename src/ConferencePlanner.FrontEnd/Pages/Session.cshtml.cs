using System;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.FrontEnd.Services;
using ConferencePlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class SessionModel : PageModel
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<SessionModel> _logger;

        public SessionModel(IApiClient apiClient, ILogger<SessionModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public SessionResponse Session { get; set; }

        public bool IsInPersonalAgenda { get; set; }

        public int? DayOffset { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            _logger.LogDebug("Fetching data for session {SessionId}", id);
            Session = await _apiClient.GetSessionAsync(id);

            if (Session == null)
            {
                _logger.LogDebug("Session not found with ID {SessionId}", id);
                return RedirectToPage("/Index");
            }

            _logger.LogDebug("Session found with ID {SessionId}", id);

            _logger.LogDebug("Fetching sessions for user {UserName}", User.Identity.Name);
            var sessions = await _apiClient.GetSessionsByAttendeeAsync(User.Identity.Name);
            _logger.LogDebug("Fetched {SessionCount} sessions for user {UserName}", userSessions.Count, User.Identity.Name);

            IsInPersonalAgenda = sessions.Any(s => s.ID == id);

            _logger.LogDebug("Fetching all sessions");
            var allSessions = await _apiClient.GetSessionsAsync();
            _logger.LogDebug("Fetched {SessionCount} sessions", result.Count);

            var startDate = allSessions.Min(s => s.StartTime?.Date);

            DayOffset = Session.StartTime?.DateTime.Subtract(startDate ?? DateTime.MinValue).Days;

            if (!string.IsNullOrEmpty(Session.Abstract))
            {
                Session.Abstract = "<p>" + String.Join("</p><p>", Session.Abstract.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)) + "</p>";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int sessionId)
        {
            _logger.LogDebug("Adding {UserName} as attendee for Session {SessionId}", User.Identity.Name, sessionId);
            await _apiClient.AddSessionToAttendeeAsync(User.Identity.Name, sessionId);
            _logger.LogDebug("Added {UserName} as attendee for Session {SessionId}", User.Identity.Name, sessionId);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int sessionId)
        {
            _logger.LogDebug("Removing {UserName} from attendees of Session {SessionId}", User.Identity.Name, sessionId);
            await _apiClient.RemoveSessionFromAttendeeAsync(User.Identity.Name, sessionId);
            _logger.LogDebug("Removed {UserName} from attendees of Session {SessionId}", User.Identity.Name, sessionId);

            return RedirectToPage();
        }
    }
}
