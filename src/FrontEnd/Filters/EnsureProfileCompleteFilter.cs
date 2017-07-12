using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace FrontEnd.Filters
{
    public class EnsureProfileCompleteFilter : IAsyncResourceFilter
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly ILogger<EnsureProfileCompleteFilter> _logger;

        public EnsureProfileCompleteFilter(IUrlHelperFactory urlHelperFactory, ILogger<EnsureProfileCompleteFilter> logger)
        {
            _urlHelperFactory = urlHelperFactory;
            _logger = logger;
        }

        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            // If the user isn't logged in, or if we already have a completed profile, we don't care
            if (context.HttpContext.User.Identity.IsAuthenticated && !context.HttpContext.User.HasClaim(c => c.Type.Equals("attendeeId")))
            {
                // Ok, check if it's an ignored route
                var urlHelper = _urlHelperFactory.GetUrlHelper(context);

                var ignoredRoutes = new HashSet<string>(new[]
                {
                    urlHelper.Action("Login", "Account"),
                    urlHelper.Action("Logout", "Account"),
                    urlHelper.Page("/Welcome")
                }, StringComparer.OrdinalIgnoreCase);

                if (!ignoredRoutes.Contains(context.HttpContext.Request.Path))
                {
                    _logger.LogTrace("Redirecting to /Welcome, profile is incomplete.");
                    context.HttpContext.Response.Redirect(urlHelper.Page("/Welcome"));
                    return Task.CompletedTask;
                }
            }

            _logger.LogTrace("Profile is complete, or the page does not require authentication");
            return next();
        }
    }
}
