using System;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace IdentityServer.STS.Admin.Helpers;

public class RedisClient
{
    private readonly IDatabase _database;

    /// <summary>
    /// 初始化 <see cref="RedisClient"/> 类的新实例。
    /// </summary>
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

public class RedisOptions
{
    public bool Enable { get; set; }

    public string IP { get; set; }

    public int Port { get; set; }

    public int Db { get; set; }

    public string Password { get; set; }
}