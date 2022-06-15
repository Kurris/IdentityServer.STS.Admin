using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.STS.Admin.Helpers
{
    /// <summary>
    /// 重定向帮助类
    /// </summary>
    public class RedirectHelper
    {
        /// <summary>
        /// 重定向
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        public static async Task<RedirectResult> Go(string url, Dictionary<string, string> queries = null)
        {
            if (queries != null && queries.Any())
            {
                using (var ps = new FormUrlEncodedContent(queries))
                {
                    return new RedirectResult(url + "?" + await ps.ReadAsStringAsync());
                }
            }

            return new RedirectResult(url);
        }

        /// <summary>
        /// 获取重定向地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        public static async Task<string> Get(string url, Dictionary<string, string> queries = null)
        {
            if (queries != null && queries.Any())
            {
                using (var ps = new FormUrlEncodedContent(queries))
                {
                    return url + "?" + await ps.ReadAsStringAsync();
                }
            }

            return url;
        }
    }
}