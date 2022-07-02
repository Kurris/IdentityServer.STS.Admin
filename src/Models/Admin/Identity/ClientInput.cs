using System.Collections.Generic;
using IdentityServer.STS.Admin.Enums;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.STS.Admin.Models.Admin.Identity
{
    public class ClientInput : Client
    {
        public ClientType ClientType { get; set; }
        public HashType HashType { get; set; }

        public new List<ClientGrantType> AllowedGrantTypes { get; set; } = new();
    }
}