namespace IdentityServer.STS.Admin.Models.Manager
{
    public class TwoFactorAuthenticationOuputModel
    {
        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }
    }
}
