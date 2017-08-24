using System.Security.Claims;
using System.Threading.Tasks;
using ConferencePlanner.FrontEnd.Authentication;
using ConferencePlanner.FrontEnd.Pages.Models;
using ConferencePlanner.FrontEnd.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ConferencePlanner.FrontEnd.Pages
{
    public class WelcomeModel : PageModel
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<WelcomeModel> _logger;

        [BindProperty]
        public Attendee Attendee { get; set; }

        public WelcomeModel(IApiClient apiClient, ILogger<WelcomeModel> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            // Send the profile to the backend
            // This will throw if there's an error (TODO: Return a result code or something)
            _logger.LogDebug("Creating attendee record for user {UserName}", User.Identity.Name);
            var attendee = await _apiClient.AddAttendeeAsync(new ConferencePlanner.Models.Attendee()
            {
                UserName = Attendee.UserName,
                FirstName = Attendee.FirstName,
                LastName = Attendee.LastName
            }, await HttpContext.GetTokenAsync("access_token"));
            _logger.LogDebug("Created attendee record for user {UserName}", User.Identity.Name);

            // Update our claim with the latest data and update our cookie
            AttendeeClaimMapper.UpdateClaims((ClaimsIdentity)User.Identity, attendee);

            _logger.LogDebug("Signing user {UserName} in", User.Identity.Name);
            await HttpContext.SignInAsync(User);

            return RedirectToPage("/Index");
        }
    }
}
