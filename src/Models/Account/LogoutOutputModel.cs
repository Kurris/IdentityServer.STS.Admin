namespace IdentityServer.STS.Admin.Models.Account
{
    public class LogoutOutputModel  :LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; } = true;

    }
}