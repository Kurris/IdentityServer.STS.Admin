using System;
using System.Collections.Generic;
using System.Linq;
using Kurisu.Authentication.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QrCodeServer.Enums;
using QrCodeServer.Models;

namespace QrCodeServer.Controllers
{
    /// <summary>
    /// 二维码控制器
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodeController : ControllerBase
    {
        private readonly ICurrentUserInfoResolver _currentUserInfoResolver;
        private readonly RedisClient _redisClient;
        private readonly ILogger<QrCodeController> _logger;
        private readonly IDataProtector _dataProtector;

        public QrCodeController(IDataProtectionProvider protectionProvider
            , ICurrentUserInfoResolver currentUserInfoResolver
            , RedisClient redisClient
            , ILogger<QrCodeController> logger)
        {
            _currentUserInfoResolver = currentUserInfoResolver;
            _redisClient = redisClient;
            _logger = logger;
            _dataProtector = protectionProvider.CreateProtector("@ligy-login-qrCode");
        }

        /// <summary>
        /// 获取二维码key
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("new")]
        public string GenerateQrGuid()
        {
            var id = GenerateStringId();

            //5分钟过期
            _redisClient.Set(id, QrCodeScanType.Wait.ToString(), 5 * 60);
            var key = _dataProtector.Protect(id);
            return $"login:{key}";
        }

        /// <summary>
        /// 获取扫描结果
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowAnonymous]
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

            var value = _redisClient.Get(id);

            return string.IsNullOrEmpty(value)
                ? QrCodeScanType.Expired.ToString()
                : value;
        }

        /// <summary>
        /// 扫描结果
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("scan")]
        public string Scan(KeyInput input)
        {
            var key = input.Key;

            var resultString = GetScanResult(key);
            var scanType = Enum.Parse<QrCodeScanType>(resultString);

            if (scanType == QrCodeScanType.Wait)
            {
                key = key.Split(':').Last();
                var id = _dataProtector.Unprotect(key);

                _redisClient.Set(id, $"{QrCodeScanType.WaitConfirm.ToString()}", 30);
                return QrCodeScanType.WaitConfirm.ToString();
            }

            return resultString;
        }

        /// <summary>
        /// 处理结果
        /// </summary>
        [HttpPost("process")]
        public string Process(ScanProcessInput input)
        {
            var key = input.Key;
            var subjectId = _currentUserInfoResolver.GetSubjectId();

            var resultString = GetScanResult(key);
            var scanType = Enum.Parse<QrCodeScanType>(resultString);

            if (scanType == QrCodeScanType.WaitConfirm)
            {
                key = key.Split(':').Last();
                var id = _dataProtector.Unprotect(key);

                if (input.Allow)
                {
                    _redisClient.Set(id, $"{QrCodeScanType.Success.ToString()}", 30);
                    _redisClient.Set(input.Key, subjectId.ToString(), 30);
                    return QrCodeScanType.Success.ToString();
                }

                _redisClient.Set(id, $"{QrCodeScanType.Denied.ToString()}", 30);
            }

            return QrCodeScanType.Denied.ToString();
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