using System;
using StackExchange.Redis;

namespace QrCodeServer
{
    public class RedisClient
    {
        private readonly IDatabase _database;

        /// <summary>
        /// 初始化 <see cref="RedisClient"/> 类的新实例。
        /// </summary>
        /// <param name="connectionMultiplexer">连接多路复用器。</param>
        public RedisClient(IConnectionMultiplexer connectionMultiplexer)
        {
            if (connectionMultiplexer != null && connectionMultiplexer.IsConnected)
            {
                _database = connectionMultiplexer.GetDatabase();
            }
            else
            {
                throw new Exception("Redis is not Connected");
            }
        }

        public bool Set(string key, string value, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return _database.StringSet(key, value, expiry);
        }

        public string Get(string key)
        {
            return _database.StringGet(key);
        }
    }
}