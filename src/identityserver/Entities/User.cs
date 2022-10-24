using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    [Table("Users")]
    public class User : IdentityUser<int>
    {
        [Key]
        public override int Id { get; set; }
    }
}