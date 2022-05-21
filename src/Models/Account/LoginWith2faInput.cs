using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    /// <summary>
    /// 2fa��¼���
    /// </summary>
    public class LoginWith2faInput
    {
        /// <summary>
        /// ˫����֤��
        /// </summary>
        [Required]
        public string TwoFactorCode { get; set; }


        /// <summary>
        /// ��ס��ǰ�豸
        /// </summary>
        public bool RememberMachine { get; set; }

        /// <summary>
        /// ��ס��(��Դ��¼����)
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// �ض����ַ
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}