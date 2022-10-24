using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Admin.Identity
{
    public class UserExistsIn
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}