using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class LoginInputModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool RememberLogin { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string RequestType { get; set; }
    }
}