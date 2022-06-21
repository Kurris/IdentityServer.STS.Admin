using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Manager
{
    /// <summary>
    /// 密码设置/修改入参
    /// </summary>
    public class SavePasswordInput
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "新密码不能为空")]
        public string NewPassword { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [Compare(nameof(NewPassword), ErrorMessage = "确认密码不匹配")]
        public string ConfirmPassword { get; set; }
    }
}