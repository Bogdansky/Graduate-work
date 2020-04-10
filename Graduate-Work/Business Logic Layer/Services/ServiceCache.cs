using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using Business_Logic_Layer.Helpers;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Services
{
    public class ServiceCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan commonlifeTime;
        private readonly TimeSpan inviteLifeTime;
        public ServiceCache(IMemoryCache memoryCache, IConfiguration config)
        {
            _memoryCache = memoryCache;

            var section = config.GetSection("CacheSettings");
            var time = TimeHelper.ParseTime(section.GetValue<string>("LifeTime"));
            var inviteTime = TimeHelper.ParseTime(section.GetValue<string>("LifeTimeForInvite"));
            commonlifeTime = time == default ? TimeSpan.FromMinutes(30) : time;
            inviteLifeTime = inviteTime == null ? TimeSpan.FromDays(1) : inviteTime;
        }

        public T Set<T>(string key, T item, TimeSpan expirationTime = default)
        {
            var time = expirationTime == default ? commonlifeTime : expirationTime;
            return _memoryCache.Set(key, item, time);
        }
        public T SetForInvite<T>(string key, T inviteItem, TimeSpan expirationTime = default)
        {
            var time = expirationTime == default ? inviteLifeTime : expirationTime;
            return _memoryCache.Set(key, inviteItem, time);
        }

        public bool TryGetValue<T>(string key, out T result)
        {
            return _memoryCache.TryGetValue(key, out result);
        }
    }
}
