using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.FrontEnd.Services;
using ConferencePlanner.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class SearchModel : PageModel
    {
        private IApiClient _apiClient;

        public SearchModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public string Term { get; set; }

        public List<object> SearchResults { get; set; }

        public async Task OnGetAsync(string term)
        {
            Term = term;
            var results = await _apiClient.SearchAsync(term);
            SearchResults = results.Select(sr =>
                                    {
                                        switch (sr.Type)
                                        {
                                            case SearchResultType.Session:
                                                return (object)sr.Value.ToObject<SessionResponse>();
                                            case SearchResultType.Speaker:
                                                return (object)sr.Value.ToObject<SpeakerResponse>();
                                            default:
                                                return (object)sr;
                                        }
                                    })
                                    .ToList();
        }
    }
}
