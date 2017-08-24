using System.Collections.Generic;
using System.Threading.Tasks;
using ConferencePlanner.FrontEnd.Services;
using ConferencePlanner.Models;
using Microsoft.Extensions.Logging;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class MyAgendaModel : IndexModel
    {
        private readonly ILogger<MyAgendaModel> _logger;

        public MyAgendaModel(IApiClient client, ILogger<MyAgendaModel> logger) :
            base(client)
        {
            _logger = logger;
        }

        protected override async Task<List<SessionResponse>> GetSessionsAsync()
        {
            _logger.LogDebug("Fetching agenda for {UserName}", User.Identity.Name);
            var result = await _apiClient.GetSessionsByAttendeeAsync(User.Identity.Name);
            _logger.LogDebug("Fetched agenda of {Count} sessions for {UserName}", result.Count, User.Identity.Name);
            return result;
        }
    }
}
