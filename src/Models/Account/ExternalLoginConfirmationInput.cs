using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Account
{
    public class ExternalLoginConfirmationInput : IValidatableObject
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_@\-\.\+]+$", ErrorMessage = "用户名只支持数字,字母和@-.+的组合")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// 密码注册
        /// </summary>
        public bool UsePassword { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 重定向url
        /// </summary>
        public string ReturnUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (UsePassword)
            {
                if (string.IsNullOrEmpty(Password))
                {
                    results.Add(new ValidationResult("密码不能为空", new[] {nameof(Password)}));
                }
                else
                {
                    //see IdentityOptions
                    if (Password.Length < 6)
                    {
                        results.Add(new ValidationResult("密码长度需要大于等于6", new[] {nameof(Password)}));
                    }
                    else
                    {
                        if (Password != ConfirmPassword)
                        {
                            results.Add(new ValidationResult("密码不一致", new[] {nameof(ConfirmPassword)}));
                        }
                    }
                }
            }

            return results;
        }
    }
}