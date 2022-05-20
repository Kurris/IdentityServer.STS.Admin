using System.ComponentModel.DataAnnotations;
using IdentityServer.STS.Admin.Enums;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class ForgotPasswordInput
    {
        [Required]
        public LoginResolutionPolicyType Policy { get; set; }

        public string Content { get; set; }
    }
}