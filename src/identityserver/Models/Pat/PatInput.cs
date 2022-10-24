using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Pat
{
    /// <summary>
    /// pat入参数
    /// </summary>
    public class PatInput
    {
        /// <summary>
        /// 生命周期
        /// </summary>
        public int? LifeTime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Required(ErrorMessage = "请描述token的作用")]
        public string Description { get; set; }

        /// <summary>
        /// 受众
        /// </summary>
        public List<string> Audiences { get; set; }

        /// <summary>
        /// 作用域
        /// </summary>
        public List<string> Scopes { get; set; }
    }
}