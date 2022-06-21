using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace IdentityServer.STS.Admin.Helpers
{
    /// <summary>
    /// 邮件html生成
    /// </summary>
    public class EmailGenerateService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _confirmEmailTemplate = "emailConfirm.html";
        private readonly string _resetPasswordTemplate = "resetPassword.html";
        private const string EmailTemplates = "EmailTemplates";

        public EmailGenerateService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        public async Task<string> GetEmailConfirmHtml(string userName, string confirmUrl, string minute)
        {
            var path = Path.Combine(_environment.ContentRootPath, EmailTemplates, _confirmEmailTemplate);
            var content = await File.ReadAllTextAsync(path);

            return content.Replace("$user", userName)
                .Replace("$url", confirmUrl)
                .Replace("$minute", minute);
        }

        public async Task<string> GetResetPasswordHtml(string confirmUrl, string minute)
        {
            var path = Path.Combine(_environment.ContentRootPath, EmailTemplates, _resetPasswordTemplate);
            var content = await File.ReadAllTextAsync(path);

            return content
                .Replace("$url", confirmUrl)
                .Replace("$minute", minute);
        }
    }
}