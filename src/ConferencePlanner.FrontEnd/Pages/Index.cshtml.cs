using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.FrontEnd.Services;
using ConferencePlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        protected readonly IApiClient _apiClient;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IApiClient apiClient, ILogger<IndexModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public IEnumerable<IGrouping<DateTimeOffset?, SessionResponse>> Sessions { get; set; }

        public IEnumerable<(int Offset, DayOfWeek? DayofWeek)> DayOffsets { get; set; }

        public List<int> UserSessions { get; set; }

        public int CurrentDayOffset { get; set; }

        [TempData]
        public string Message { get; set; }

        public bool ShowMessage => !string.IsNullOrEmpty(Message);

        protected virtual async Task<List<SessionResponse>> GetSessionsAsync()
        {
            _logger.LogDebug("Fetching all sessions");
            var result = await _apiClient.GetSessionsAsync();
            _logger.LogDebug("Fetched {Count} sessions", result.Count);
            return result;
        }

        public async Task OnGet(int day = 0)
        {
            CurrentDayOffset = day;

            _logger.LogDebug("Fetching sessions for user {UserName}", User.Identity.Name);
            var userSessions = await _apiClient.GetSessionsByAttendeeAsync(User.Identity.Name);
            _logger.LogDebug("Fetched {SessionCount} sessions for user {UserName}", userSessions.Count, User.Identity.Name);

            UserSessions = userSessions.Select(u => u.ID).ToList();

            var sessions = await GetSessionsAsync();

            var startDate = sessions.Min(s => s.StartTime?.Date);
            var endDate = sessions.Max(s => s.EndTime?.Date);

            var numberOfDays = ((endDate - startDate)?.Days) + 1;

            DayOffsets = Enumerable.Range(0, numberOfDays ?? 0)
                .Select(offset => (offset, (startDate?.AddDays(offset))?.DayOfWeek));

            var filterDate = startDate?.AddDays(day);

            Sessions = sessions.Where(s => s.StartTime?.Date == filterDate)
                               .OrderBy(s => s.TrackId)
                               .GroupBy(s => s.StartTime)
                               .OrderBy(g => g.Key);
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
