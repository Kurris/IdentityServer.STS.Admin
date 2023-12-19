namespace IdentityServer.STS.Admin.Models.Admin.Identity;

/// <summary>
/// 用户分页查询类
/// </summary>
public class UserSearchInput : PageIn
{
    /// <summary>
    /// 搜索内容
    /// </summary>
    public string Content { get; set; }
}