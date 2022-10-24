using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Models.Manager
{
    /// <summary>
    /// 展示关联的外部登录返回值
    /// </summary>
    public class ExternalLoginsOutput
    {
        /// <summary>
        /// 当前登录
        /// </summary>
        public IEnumerable<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        /// 其他登录
        /// </summary>
        public IEnumerable<AuthenticationScheme> OtherLogins { get; set; }

        /// <summary>
        /// 是否可以解除关联
        /// </summary>
        public bool AbleRemove { get; set; }
    }
}
