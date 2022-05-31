namespace IdentityServer.STS.Admin.Models.Account
{
    public class LogoutOutput : LogoutInput
    {
        /// <summary>
        /// 是否显示退出提醒
        /// </summary>
        public bool ShowLogoutPrompt { get; set; }
    }
}