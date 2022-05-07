using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class ExternalLoginConfirmationInput
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_@\-\.\+]+$")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }
    }
}