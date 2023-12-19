using IdentityServer.STS.Admin.Enums;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.STS.Admin.Models.Admin.Identity;

public class ClientSecretInput: ClientSecret
{
    public HashType HashType{ get; set; }
}
