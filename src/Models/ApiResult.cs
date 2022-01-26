namespace IdentityServer.STS.Admin.Models
{
    public class ApiResult<T> : IApiResult
    {
        public ApiResult()
        {
            this.Code = 200;
            this.Route = DefineRoute.None;
            this.Msg = "操作成功";
        }


        public int Code { get; set; }

        public T Data { get; set; }

        public string Msg { get; set; }

        public DefineRoute Route { get; set; }

        public IApiResult GetDefaultSuccessApiResult<TResult>(TResult apiResult)
        {
            return new ApiResult<TResult>()
            {
                Data = apiResult
            };
        }

        public IApiResult GetDefaultErrorApiResult<TResult>(TResult apiResult)
        {
            return new ApiResult<TResult>()
            {
                Code = 500,
                Msg = "操作有误",
                Data = apiResult
            };
        }

        public IApiResult GetDefaultValidateApiResult<TResult>(TResult apiResult)
        {
            return new ApiResult<TResult>()
            {
                Code = 400,
                Msg = "实体验证失败",
                Data = apiResult
            };
        }
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
        ExternalLoginCallback = 11,
        Error = 12,
        RecoveryCodes = 13,
        TwoFactorAuthentication = 14,
        ConfirmEmail = 15,
    }
}