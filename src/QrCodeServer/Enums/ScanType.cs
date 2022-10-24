namespace QrCodeServer.Enums
{
    public enum QrCodeScanType
    {
        Wait = 0,
        Expired = 1,
        Denied = 2,
        NotExists = 3,
        WaitConfirm = 4,
        Success = 99
    }
}