using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mkb.CacheItSimple
{
    public static class Cache
    {
        private static Dictionary<string, CacheData> _cache = new Dictionary<string, CacheData>();

        private static object basicAsync = new object();

        public static object Run(Func<object> func, int ttl)
        {
            var details = (new System.Diagnostics.StackTrace()).GetFrame(1);
            var method = details.GetMethod();
            var calling = Assembly.GetCallingAssembly().GetName();
            var magicName = $"{calling.Name}.{method.ReflectedType?.Name??""}.{ method.Name}";

            if (_cache.TryGetValue(magicName, out var value))
            {
                if (value.KillTime < DateTime.Now)
                {
                    lock (basicAsync)
                    {
                        value.KillTime = DateTime.Now.AddSeconds(ttl);
                        value.Data = func.Invoke();
                    }
                }

                return value.Data;
            }

            value = new CacheData
            {
                KillTime = DateTime.Now.AddSeconds(ttl),
                Data = func.Invoke()
            };
            lock (basicAsync)
            {
                _cache.Add(magicName, value);
            }

            return value.Data;
        }
    }

    internal class CacheData
    {
        public object Data { get; set; }
        public DateTime KillTime { get; set; }
    }
}