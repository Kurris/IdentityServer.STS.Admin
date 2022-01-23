namespace IdentityServer.STS.Admin.Models.Manager
{
    public class RemoveLoginInputModel
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
