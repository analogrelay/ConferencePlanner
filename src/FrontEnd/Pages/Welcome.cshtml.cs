using System.Security.Claims;
using System.Threading.Tasks;
using FrontEnd.Authentication;
using FrontEnd.Pages.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEnd.Pages
{
    public class WelcomeModel : PageModel
    {
        private readonly IApiClient _apiClient;

        [BindProperty]
        public Attendee Attendee { get; set; }

        public WelcomeModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            // Send the profile to the backend
            // This will throw if there's an error (TODO: Return a result code or something)
            var attendee = await _apiClient.AddAttendeeAsync(new ConferenceDTO.Attendee()
            {
                UserName = Attendee.UserName,
                FirstName = Attendee.FirstName,
                LastName = Attendee.LastName
            }, await HttpContext.GetTokenAsync("access_token"));

            // Update our claim with the latest data and update our cookie
            AttendeeClaimMapper.UpdateClaims((ClaimsIdentity)User.Identity, attendee);
            await HttpContext.SignInAsync(User);

            return RedirectToPage("/Index");
        }
    }
}
