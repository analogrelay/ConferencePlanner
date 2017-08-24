using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.Models;
using ConferencePlanner.FrontEnd.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class SpeakersModel : PageModel
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<SpeakersModel> _logger;

        public SpeakersModel(IApiClient apiClient, ILogger<SpeakersModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public IEnumerable<SpeakerResponse> Speakers { get; set; }

        public async Task OnGet()
        {
            _logger.LogDebug("Fetching all speakers");
            var speakers = await _apiClient.GetSpeakersAsync();
            _logger.LogDebug("Fetched {SpeakerCount} speakers", speakers.Count);

            Speakers = speakers.OrderBy(s => s.Name);
        }
    }
}
