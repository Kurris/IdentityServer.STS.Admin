using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Manager
{
    public class AuthenticatorInput
    {
        [Required(ErrorMessage = "验证码不能为空"), StringLength(6, ErrorMessage = "验证码需要6位长度")]
        public string Code { get; set; }
    }
}