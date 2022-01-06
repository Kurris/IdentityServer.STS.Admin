namespace IdentityServer.STS.Admin.Models
{
    public class ApiResult<T>
    {
        public int Code { get; set; }

        public T Data { get; set; }

        public string Msg { get; set; }

        public bool Redirect { get; set; }
    }
}