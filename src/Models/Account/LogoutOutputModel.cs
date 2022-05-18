namespace IdentityServer.STS.Admin.Models.Account
{
    public class LogoutOutputModel  :LogoutInput
    {
        public bool ShowLogoutPrompt { get; set; } = true;

    }
}