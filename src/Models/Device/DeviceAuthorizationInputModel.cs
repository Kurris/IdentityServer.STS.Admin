using IdentityServer.STS.Admin.Models.Consent;

namespace IdentityServer.STS.Admin.Models.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}