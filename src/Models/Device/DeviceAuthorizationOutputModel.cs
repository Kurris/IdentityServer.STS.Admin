using IdentityServer.STS.Admin.Models.Consent;

namespace IdentityServer.STS.Admin.Models.Device
{
    public class DeviceAuthorizationOutputModel : ConsentOutputModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}