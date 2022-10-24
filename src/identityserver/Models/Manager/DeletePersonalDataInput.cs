using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Manager
{
    public class DeletePersonalDataInput
    {
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
    }
}
