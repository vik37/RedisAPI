using System.Text.Json;
using RedisAPI.Models;
using StackExchange.Redis;

namespace RedisAPI.Data;

public class RedisPlatformRepo : IPlatformRepo
{
    private readonly IConnectionMultiplexer _redis;
    private IDatabase _db;
    public RedisPlatformRepo(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }
    public void CreatePlatform(Platform platform)
    {
        if(platform is null)
            throw new ArgumentOutOfRangeException(nameof(platform));

        var serialPlatform = JsonSerializer.Serialize(platform);

    /* ************* SCAN SET - STRING (KEY-VALUE pairs) ********************** */
        //_db.StringSet(platform.Id,serialPlatform);
        //_db.SetAdd("PlatformSet",serialPlatform);

    /* ************** HASHSES (KEY-[FIELD-VALUE] pairs) ***************************** */
        _db.HashSet("hashplatform", new HashEntry[]
        {new HashEntry(platform.Id,serialPlatform)});
    }

    public IEnumerable<Platform?>? GetAllPlatforms()
    {
        //var completeSet = _db.SetMembers("PlatformSet");
        var completeHash = _db.HashGetAll("hashplatform");
        if(completeHash.Length > 0)
        {
            //var obj = Array.ConvertAll(completeSet, val => JsonSerializer.Deserialize<Platform>(val)).ToList();
            var obj = Array.ConvertAll(completeHash, val => JsonSerializer.Deserialize<Platform>(val.Value)).ToList();
            return obj;
        }

        return null;
    }

    public Platform? GetPlatformById(string id)
    {
        //var plat = _db.StringGet(id);
        var plat = _db.HashGet("hashplatform", id);

        if(!string.IsNullOrEmpty(plat))
            return JsonSerializer.Deserialize<Platform>(plat);

        return null;
    }
}