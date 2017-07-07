using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace FrontEnd.Authentication
{
    public class OpenIdConnectOptionsSetup : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly AzureAdB2COptions _azureAdB2COptions;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<OpenIdConnectOptionsSetup> _logger;

        public OpenIdConnectOptionsSetup(IOptions<AzureAdB2COptions> b2cOptions, IHostingEnvironment hostingEnvironment, ILogger<OpenIdConnectOptionsSetup> logger)
        {
            _azureAdB2COptions = b2cOptions.Value;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            if (string.Equals(name, OpenIdConnectDefaults.AuthenticationScheme))
            {
                Configure(options);
            }
        }

        public void Configure(OpenIdConnectOptions options)
        {
            options.ClientId = _azureAdB2COptions.ClientId;
            options.Authority = _azureAdB2COptions.Authority;

            options.SaveTokens = true; // Save the access_token so we can use it for the API

            options.UseTokenLifetime = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                NameClaimType = "name",
            };

            options.Events = new OpenIdConnectEvents()
            {
                OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
                OnRemoteFailure = OnRemoteFailure,
                OnAuthorizationCodeReceived = OnAuthorizationCodeReceived
            };
        }

        private Task OnTokenValidated(TokenValidatedContext arg)
        {
            throw new NotImplementedException();
        }

        public Task OnRedirectToIdentityProvider(RedirectContext context)
        {
            var defaultPolicy = _azureAdB2COptions.DefaultPolicy;
            var apiScopes = _azureAdB2COptions.ApiScopes.Split(' ').Select(s => $"https://{_azureAdB2COptions.Tenant}/{_azureAdB2COptions.ApiId}/{s}");
            context.ProtocolMessage.Scope += $" offline_access {string.Join(' ', apiScopes)}";
            context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            return Task.FromResult(0);
        }

        public async Task OnRemoteFailure(RemoteFailureContext context)
        {
            var requestId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
            _logger.LogError(context.Failure, "[Request: {requestId}] Authentication Failure", requestId);
            //if (_hostingEnvironment.IsDevelopment())
            //{
            //    // Handle in-place and report the error
            //    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            //    await context.Response.WriteAsync(context.Failure.ToString());
            //}
            //else
            //{
                var url = $"/Error?failedRequestId={requestId}";
                context.Response.Redirect(url);
                context.HandleResponse();
            //}
        }

        public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            // Use MSAL to swap the code for an access token
            // Extract the code from the response notification
            var code = context.ProtocolMessage.Code;

            string signedInUserID = context.Result.Ticket.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

            // TODO: Cache tokens?
            var tokenCache = new TokenCache();

            var redirectUri = new UriBuilder(context.Request.Scheme, context.Request.Host.Host, context.Request.Host.Port ?? 80, context.Request.PathBase + "/signin-oidc");
            ConfidentialClientApplication cca = new ConfidentialClientApplication(_azureAdB2COptions.ClientId, _azureAdB2COptions.Authority, redirectUri.Uri.ToString(), new ClientCredential(_azureAdB2COptions.ClientSecret), tokenCache, null);
            try
            {
                AuthenticationResult result = await cca.AcquireTokenByAuthorizationCodeAsync(code, _azureAdB2COptions.ScopeString.Split(' '));

                context.HandleCodeRedemption(result.AccessToken, result.IdToken);
            }
            catch (Exception)
            {
                //TODO: Handle
                throw;
            }
        }
    }
}
