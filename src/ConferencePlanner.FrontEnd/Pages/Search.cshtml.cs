using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.FrontEnd.Services;
using ConferencePlanner.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class SearchModel : PageModel
    {
        private IApiClient _apiClient;
        private readonly ILogger<SearchModel> _logger;

        public SearchModel(IApiClient apiClient, ILogger<SearchModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public string Term { get; set; }

        public List<object> SearchResults { get; set; }

        public async Task OnGetAsync(string term)
        {
            Term = term;
            _logger.LogDebug("Searching for results matching {Term}", term);
            var results = await _apiClient.SearchAsync(term);

            var sessionCount = 0;
            var speakerCount = 0;
            SearchResults = results.Select(sr =>
                                    {
                                        switch (sr.Type)
                                        {
                                            case SearchResultType.Session:
                                                sessionCount += 1;
                                                return sr.Value.ToObject<SessionResponse>();
                                            case SearchResultType.Speaker:
                                                speakerCount += 1;
                                                return sr.Value.ToObject<SpeakerResponse>();
                                            default:
                                                return (object)sr;
                                        }
                                    })
                                    .ToList();
            _logger.LogDebug("Found {SessionCount} sessions and {SpeakerCount} speakers matching {Term}", sessionCount, speakerCount, term);
        }
    }
}
