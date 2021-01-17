using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Assert_returns_cache()
        {
            string g = "f";
            var r = Mkb.CacheItSimple.Cache.Run(() => $"{g}t", 33);
            var b = Mkb.CacheItSimple.Cache.Run(() => $"{g}e", 180);
            Assert.Equal("ft", b);
            Assert.Equal(r, b);
        }

        [Fact]
        public async Task Assert_will_kill_cache_after_set_Time()
        {
            string g = "f";
            var r = Mkb.CacheItSimple.Cache.Run(() => $"{g}t", 1);
            await Task.Delay(1200);
            var b = Mkb.CacheItSimple.Cache.Run(() => $"{g}e", 180);
            Assert.Equal("ft", r);
            Assert.Equal("fe", b);
        }
    }
}