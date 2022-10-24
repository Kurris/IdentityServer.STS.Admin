namespace IdentityServer.STS.Admin.Entities
{
    /// <summary>
    /// 客户端所有者
    /// </summary>
    public class ClientOwners
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
    }
}