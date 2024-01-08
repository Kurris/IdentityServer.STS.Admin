using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.STS.Admin.Entities;

[Table("UserTenants")]
public class UserTenant
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string TenantCode { get; set; }
}
