using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using System;

namespace OrderManagerCLI.CacheManagers
{
    public class RedisCacheManager
    {
        private static IDatabase Connection => redisConnection.Value.GetDatabase();

        private static Lazy<ConnectionMultiplexer> redisConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            ConfigurationOptions conf = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = 100,
                Password = Common.Constants.RedisPass
            };

            conf.EndPoints.Add(Common.Constants.InternalHost, 6379);

            return ConnectionMultiplexer.Connect(conf.ToString());
        });

        public string GetKey(string key)
        {
            return Connection.StringGet(key);
        }

        public bool SetKey(string key, string value, TimeSpan expiration)
        {
            bool isLock = false;
            try
            {
                isLock = Connection.StringSet(key, value, expiration, When.NotExists);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error When Write: {0}", ex.Message);
                isLock = true;
            }
            return isLock;
        }

        public void RemoveLockKey(string key)
        {
            Connection.KeyDelete(key);
        }
    }
}
