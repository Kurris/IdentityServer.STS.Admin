using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.STS.Admin.Entities;


[Table("Tenants")]
public class Tenant
{
    [Key]
    public int Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }
}
