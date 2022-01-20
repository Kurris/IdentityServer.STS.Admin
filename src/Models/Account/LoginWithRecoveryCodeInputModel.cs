using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class LoginWithRecoveryCodeInputModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string RecoveryCode { get; set; }

        public string ReturnUrl { get; set; }
    }
}
