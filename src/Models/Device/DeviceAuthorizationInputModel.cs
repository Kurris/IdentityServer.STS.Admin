using IdentityServer.STS.Admin.Models.Consent;

namespace IdentityServer.STS.Admin.Models.Device
{
    public class DeviceAuthorizationInputModel : ConsentInput
    {
        public string UserCode { get; set; }
    }
}