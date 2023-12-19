using System;

namespace IdentityServer.STS.Admin.Models.Pat;

public class PatOutput
{
    public string Key { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime? ExpiredTime { get; set; }

    public bool IsPermanent
    {
        get
        {
            if (!ExpiredTime.HasValue)
            {
                return true;
            }

            //大于50年
            return (ExpiredTime.Value - CreateTime).Days > 30 * 12 * 50;
        }
    }
}