using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 用户
    /// <remarks>
    /// 如果需要自定义主键Id类型，需要继承<see cref="IdentityUser{T}"/> ,否则默认以<see cref="Guid"/>的<see cref="string"/>类型为key
    /// </remarks>
    /// </summary>
    [Table("Users")]
    public class User : IdentityUser
    {
    }
}