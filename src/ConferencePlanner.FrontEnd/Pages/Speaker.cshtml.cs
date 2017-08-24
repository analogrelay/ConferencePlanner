using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.Models;
using ConferencePlanner.FrontEnd.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class SpeakerModel : PageModel
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<SpeakerModel> _logger;

        public SpeakerModel(IApiClient apiClient, ILogger<SpeakerModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public SpeakerResponse Speaker { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            _logger.LogDebug("Fetching data for speaker {SpeakerId}", id);
            Speaker = await _apiClient.GetSpeakerAsync(id);

            if (Speaker == null)
            {
                _logger.LogDebug("Speaker not found with ID {SpeakerId}", id);
                return NotFound();
            }

            _logger.LogDebug("Speaker found with ID {SpeakerId}", id);
            return Page();
        }
    }
}
