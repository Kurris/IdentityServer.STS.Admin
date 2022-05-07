using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 角色
    /// </summary>
    [Table("Roles")]
    public class Role : IdentityRole<int>
    {
        [Key]
        public override int Id { get; set; }
    }
}