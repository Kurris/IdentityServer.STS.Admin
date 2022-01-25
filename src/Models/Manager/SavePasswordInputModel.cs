using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Manager
{
    public class SavePasswordInputModel
    {
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "二次密码不匹配")]
        public string ConfirmPassword { get; set; }
    }
}