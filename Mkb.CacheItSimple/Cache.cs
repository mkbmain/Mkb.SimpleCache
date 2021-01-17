using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Tests")]

namespace Mkb.CacheItSimple
{
    public static class Cache
    {
        internal static Dictionary<string, CacheData> _cache = new Dictionary<string, CacheData>();

        private static object basicAsync = new object();

        public static T Run<T>(Func<T> func, int ttl,Func<T,bool> exp = null)
        {
            var magicName = MagicName(Assembly.GetCallingAssembly().GetName().Name);
            return Run(magicName, func, ttl, exp);
        }
        
        private static string MagicName(string callingAssembly)
        {
            var details = (new System.Diagnostics.StackTrace()).GetFrame(2);
            var method = details.GetMethod();
            var magicName = $"{callingAssembly}.{method.ReflectedType?.Name ?? ""}.{method.Name}";
            return magicName;
        }

        private static T Run<T>(string magicName, Func<T> func, int ttl, Func<T, bool> exp)
        {
            if (_cache.TryGetValue(magicName, out var value))
            {
                if (value.KillTime < DateTime.Now)
                {
                    lock (basicAsync)
                    {
                        value.KillTime = DateTimeOffset.Now.AddSeconds(ttl);
                        value.Data = func.Invoke();
                    }
                }

                return (T) value.Data;
            }

            value = new CacheData
            {
                KillTime = DateTimeOffset.Now.AddSeconds(ttl),
                Data = func.Invoke()
            };
            if (exp == null || exp.Invoke((T) value.Data))
            {
                lock (basicAsync)
                {
                    _cache.Add(magicName, value);
                }
            }

            return (T) value.Data;
        }
    }

    internal class CacheData
    {
        public object Data { get; set; }
        public DateTimeOffset KillTime { get; set; }
    }
}