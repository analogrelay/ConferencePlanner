using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferencePlanner.Models;
using ConferencePlanner.FrontEnd.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class SpeakersModel : PageModel
    {
        private readonly IApiClient _apiClient;

        public SpeakersModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public IEnumerable<SpeakerResponse> Speakers { get; set; }

        public async Task OnGet()
        {
            var speakers = await _apiClient.GetSpeakersAsync();

            Speakers = speakers.OrderBy(s => s.Name);
        }
    }
}
