# SimpleCache
ever had get requests in your api that calls the db but change rarely why not cache it quickly and efficiently controlled totally by code.
This little library allows you to call it parsing a function.

## usage
```c#
public IactionResult GetDbEntitys()
{
    Mkb.CacheItSimple.Run(() =>
    {
       // what ever                      // full function
       return someSeriveToDb();
      }, 10 * 60, f => f.StatusCode == 200);  // followed by ttl in seconds we have set it to 10 mins , and a wanted out put i.e if status code out is not 200 don't cache this
} 
```
on first run it will cache this result and return it with out calling the function on all subsequent requests for 10 mins at that point the cache will go stale and it will rerun the func on next call


the tests show more enjoy
