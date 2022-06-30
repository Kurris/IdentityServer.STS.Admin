using System;
using System.Collections.Generic;
using IdentityServer.STS.Admin.Models.Account;

namespace IdentityServer.STS.Admin.Models.Manager
{
    public class GrantOutput
    {
        public UserOutput User { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expires { get; set; }
        public IEnumerable<string> IdentityGrantNames { get; set; }
        public IEnumerable<string> ApiGrantNames { get; set; }
    }
}