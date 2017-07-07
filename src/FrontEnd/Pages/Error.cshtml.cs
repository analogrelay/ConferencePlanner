using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FrontEnd.Pages
{
    public class ErrorModel : PageModel
    {
        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public void OnGet(string failedRequestId)
        {
            if (string.IsNullOrEmpty(failedRequestId))
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            }
            else
            {
                // Log a message to indicate that we're tracing an error
                _logger.LogError("Redirected to error page by request {failedRequestId}", failedRequestId);
                RequestId = failedRequestId;
            }
        }
    }
}
