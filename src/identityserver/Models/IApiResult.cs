namespace IdentityServer.STS.Admin.Models
{
    public interface IApiResult
    {
        IApiResult GetDefaultSuccessApiResult<TResult>(TResult apiResult);

        IApiResult GetDefaultErrorApiResult<TResult>(TResult apiResult);

        IApiResult GetDefaultValidateApiResult<TResult>(TResult apiResult);
    }
}