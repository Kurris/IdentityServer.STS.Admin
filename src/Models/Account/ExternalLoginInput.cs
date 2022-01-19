namespace IdentityServer.STS.Admin.Models.Account
{
    public class ExternalLoginInput
    {
        public string ReturnUrl { get; set; }
        public string Provider { get; set; }
    }
}