using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class CacheTests
    {
        [Fact]
        public void Assert_returns_cache()
        {
            string g = "f";
            var r = Mkb.CacheItSimple.Run(() => $"{g}t", 33);
            var b = Mkb.CacheItSimple.Run(() => $"{g}e", 180);
            Assert.Equal("ft", b);
            Assert.Equal(r, b);
            
        }
        
        [Fact]
        public void Assert_we_only_cache_expected_results_if_supplied()
        {
            string g = "f";
            var one = Mkb.CacheItSimple.Run(() => $"{g}t", 180, f=> f=="ee"); // should not cache
            var two = Mkb.CacheItSimple.Run(() => $"{g}e", 180,f=> f==$"{g}e"); // should cache
            var three = Mkb.CacheItSimple.Run(() => $"{g}e2", 180,f=> f==$"{g}e11"); // should get last cache not this
            Assert.Equal("ft", one);
            Assert.Equal("fe", two);
            Assert.Equal("fe", three);
        }
        
        [Fact]
        public void Assert_magic_string_connect()
        {
            string g = "f";
            var r = Mkb.CacheItSimple.Run(() => $"{g}t", 33);

            Assert.True(Mkb.CacheItSimple._cache.ContainsKey($"Tests.{nameof(CacheTests)}.{nameof(Assert_magic_string_connect)}"));
        }

        [Fact]
        public async Task Assert_will_kill_cache_after_set_Time()
        {
            string g = "f";
            var r = Mkb.CacheItSimple.Run(() => $"{g}t", 1);
            await Task.Delay(1200);
            var b = Mkb.CacheItSimple.Run(() => $"{g}e", 180);
            Assert.Equal("ft", r);
            Assert.Equal("fe", b);
        }
    }
}