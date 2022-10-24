namespace IdentityServer.STS.Admin.Models.Admin.Identity
{
    public class UserProviderDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}