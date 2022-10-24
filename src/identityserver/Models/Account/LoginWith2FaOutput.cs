namespace IdentityServer.STS.Admin.Models.Account
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginWith2FaOutput
    {
        public string TwoFactorCode { get; set; }

        public bool RememberMachine { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}