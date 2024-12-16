using Microsoft.Extensions.Caching.Memory;
using OnlinePropertyBookingPlatform.Utility;

namespace OnlinePropertyBookingPlatform.Utility
{

    //предотвратява изпълнението на злонамерен JavaScript код, който може да бъде инжектиран чрез текстови полета(например коментари, имейли, заглавия).
    //Тази защита е критична за сигурността на приложение.


    public static class InputValidator
    {
         public static string SanitizeInput(string input)
         {
            // Basic sanitization for input fields
            return System.Net.WebUtility.HtmlEncode(input);
         }
    }

    public static class RateLimiter
    {
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        public static bool IsRateLimited(string key, int limit, TimeSpan duration)
        {
            if (Cache.TryGetValue(key, out int count))
            {
                if (count >= limit)
                {
                    return true;
                }
                Cache.Set(key, count + 1, duration);
            }
            else
            {
                Cache.Set(key, 1, duration);
            }

            return false;
        }
    }
}