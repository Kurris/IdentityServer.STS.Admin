using System;
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
        private static string _baseUrl;
        private static string _successUrl;
        private static string _errorUrl;

        public static void Initialize(string baseUrl, string successUrl = null, string errorUrl = null)
        {
            _baseUrl = baseUrl;
            _successUrl = successUrl;
            _errorUrl = errorUrl;
        }

        /// <summary>
        /// 重定向
        /// </summary>
        /// <param name="url">重定向url</param>
        /// <param name="queries">url参数</param>
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
        /// <param name="url">重定向url</param>
        /// <param name="queries">url参数</param>
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

        public static async Task<RedirectResult> Success(Dictionary<string, string> queries = null)
        {
            if (string.IsNullOrEmpty(_successUrl))
                throw new ArgumentNullException(nameof(_successUrl));

            var url = _baseUrl + _successUrl;
            return await Go(url, queries);
        }

        public static async Task<RedirectResult> Error(Dictionary<string, string> queries = null)
        {
            if (string.IsNullOrEmpty(_errorUrl))
                throw new ArgumentNullException(nameof(_errorUrl));

            var url = _baseUrl + _errorUrl;
            return await Go(url, queries);
        }
    }
}