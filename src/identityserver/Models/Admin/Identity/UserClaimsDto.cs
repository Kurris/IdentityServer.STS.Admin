using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Admin.Identity
{
    public class UserClaimsDto
    {
        public int ClaimId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
    }
}