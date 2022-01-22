using System.ComponentModel.DataAnnotations;

namespace IdentityServer.STS.Admin.Models.Manager
{
    public class DeletePersonalDataInputModel
    {
        public bool RequirePassword { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}
