using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class ForgotPasswordInputModel
    {
        [Required]
        public LoginResolutionPolicy Policy { get; set; }

        public string Content { get; set; }
    }
}