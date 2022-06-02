namespace IdentityServer.STS.Admin.Models.Consent
{
    public class ScopeOutput
    {
        /// <summary>
        /// 作用域值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否提醒
        /// </summary>
        public bool Emphasize { get; set; }

        /// <summary>
        /// 是否必需
        /// </summary>
        public bool Required { get; set; }


        /// <summary>
        /// 选中
        /// </summary>
        public bool Checked { get; set; }
    }
}