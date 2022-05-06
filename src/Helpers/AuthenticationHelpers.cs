using Microsoft.AspNetCore.Http;

namespace IdentityServer.STS.Admin.Helpers
{
    public class AuthenticationHelpers
    {
        public static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

                if (!httpContext.Request.IsHttps || DisallowsSameSiteNone(userAgent))
                {
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

        private static bool DisallowsSameSiteNone(string userAgent)
        {
            if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
                return true;

            if (userAgent.Contains("Macintosh; Intel Mac Os X 10_14")
                && userAgent.Contains("Version/")
                && userAgent.Contains("Safari"))
                return true;


            return userAgent.Contains("Chome/5") || userAgent.Contains("Chome/6");
        }
    }
}
