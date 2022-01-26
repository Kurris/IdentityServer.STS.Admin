using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Admin.Identity
{
    public class UserDto
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "用户名不能为空")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "错误的邮件格式")]
        [Required(ErrorMessage = "用户邮件地址不能为空")]
        public string Email { get; set; }

        public bool IsConfirmEmail { get; set; }
        public string Phone { get; set; }
        public bool IsConfirmPhone { get; set; }
        public bool IsEnableLockout { get; set; }
        public DateTimeOffset? LockoutExpire { get; set; }

        public DateTime? LockoutLocalExpire => LockoutExpire?.LocalDateTime;
        public bool IsEnableTwoFactor { get; set; }
        public int LoginFailCount { get; set; }
    }
}