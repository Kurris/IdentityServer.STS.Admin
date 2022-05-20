using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class ResetPasswordInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}