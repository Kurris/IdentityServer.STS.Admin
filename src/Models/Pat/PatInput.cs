using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Pat
{
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
    }
}