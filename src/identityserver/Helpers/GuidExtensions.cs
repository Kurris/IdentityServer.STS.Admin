using System;

namespace IdentityServer.STS.Admin.Helpers;

public static class GuidExtensions
{
    public static string GetShortId(this Guid guid)
    {
        long i = 1;
        foreach (byte b in guid.ToByteArray())
        {
            i *= b + 1;
        }

        return $"{i - DateTime.Now.Ticks:x}";
    }
}