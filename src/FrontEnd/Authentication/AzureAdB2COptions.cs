namespace FrontEnd.Authentication
{
    public class AzureAdB2COptions
    {
        public const string PolicyAuthenticationProperty = "Policy";

        public AzureAdB2COptions()
        {
            AzureAdB2CInstance = "https://login.microsoftonline.com/tfp";
        }

        public string ClientId { get; set; }
        public string AzureAdB2CInstance { get; set; }
        public string Tenant { get; set; }
        public string SignUpSignInPolicyId { get; set; }
        public string SignInPolicyId { get; set; }
        public string SignUpPolicyId { get; set; }
        public string ResetPasswordPolicyId { get; set; }
        public string EditProfilePolicyId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiId { get; set; }
        public string ApiScopes { get; set; }

        public string DefaultPolicy => SignUpSignInPolicyId;
        public string Authority => $"{AzureAdB2CInstance}/{Tenant}/{DefaultPolicy}/v2.0";

        // ApiScopes is space-separated, but that's exactly the format we want:
        // "https://[Tenant]/conferenceapi/read write foo bar"
        public string ScopeString => $"https://{Tenant}/{ApiId}/{ApiScopes}";
    }
}
