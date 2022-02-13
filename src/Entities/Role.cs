using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 角色
    /// </summary>
    [Table("Roles")]
    public class Role : IdentityRole
    {

    }
}