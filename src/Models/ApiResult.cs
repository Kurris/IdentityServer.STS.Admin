namespace IdentityServer.STS.Admin.Models
{
    public class ApiResult<T>
    {
        public ApiResult()
        {
            this.Code = 200;
        }


        public int Code { get; set; }

        public T Data { get; set; }

        public string Msg { get; set; }

        public DefineRoute Route { get; set; }
    }


    public enum DefineRoute
    {
        None = 0,
        Redirect = 1,
        HomePage = 2,
        LoadingPage = 3,
        LoginWith2Fa = 4,
        Lockout = 5,
        Login = 6,
        ExternalLoginFailure = 7,
        ExternalLoginConfirmation = 8,
        LoginOut = 9,
        LoggedOut = 10,
    }
}