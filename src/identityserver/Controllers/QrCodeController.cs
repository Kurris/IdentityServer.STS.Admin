using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer.STS.Admin.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IdentityServer.STS.Admin.Controllers
{
    /// <summary>
    /// 二维码控制器
    /// </summary>
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodeController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<QrCodeController> _logger;
        private readonly IDataProtector _dataProtector;

        public QrCodeController(IMemoryCache cache, IDataProtectionProvider protectionProvider, ILogger<QrCodeController> logger)
        {
            _cache = cache;
            _logger = logger;
            _dataProtector = protectionProvider.CreateProtector("@ligy-login-qrCode");
        }

        /// <summary>
        /// 获取二维码key
        /// </summary>
        /// <returns></returns>
        [HttpGet("new")]
        public string GenerateQrGuid()
        {
            var id = GenerateStringId();

            //5分钟过期
            _cache.Set(id, QrCodeScanType.Wait.ToString(), DateTime.Now.AddMinutes(5));
            var key = _dataProtector.Protect(id);
            return $"login:{key}";
        }

        /// <summary>
        /// 获取扫描结果
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("scanResult")]
        public string GetScanResult(string key)
        {
            string id = string.Empty;
            try
            {
                key = key.Split(':').Last();
                id = _dataProtector.Unprotect(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            if (string.IsNullOrEmpty(id))
            {
                return QrCodeScanType.NotExists.ToString();
            }

            return _cache.TryGetValue(id, out string value)
                ? value
                : QrCodeScanType.Expired.ToString();
        }

        /// <summary>
        /// 扫描结果
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost("scan")]
        public string Scan(Dictionary<string, string> dic)
        {
            string key = dic["key"];
            string subjectId = dic["subjectId"];

            var resultString = GetScanResult(key);
            var scanType = Enum.Parse<QrCodeScanType>(resultString);

            if (scanType == QrCodeScanType.Wait)
            {
                key = key.Split(':').Last();
                var id = _dataProtector.Unprotect(key);

                _cache.Set(id, $"{QrCodeScanType.WaitConfirm.ToString()}:{subjectId}", DateTime.Now.AddSeconds(30));
                return QrCodeScanType.WaitConfirm.ToString();
            }

            return resultString;
        }


        private static string GenerateStringId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= b + 1;
            }

            return $"{i - DateTime.Now.Ticks:x}";
        }
    }
}