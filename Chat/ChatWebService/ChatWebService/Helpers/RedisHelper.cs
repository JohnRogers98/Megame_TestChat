using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace ChatWebService.Helpers
{
    public static class RedisHelper
    {
        public static async void SetCache(this IDistributedCache cache, String key, Object value, DistributedCacheEntryOptions options = null)
        {
            var serializedObject = JsonConvert.SerializeObject(value);
            var serializedBytes = Encoding.UTF8.GetBytes(serializedObject);

            if (options == null)
            {
                options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));
            }

            await cache.SetAsync(key, serializedBytes, options);
        }
    }
}
