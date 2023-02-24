namespace Cafet_Backend.Helper.Cache;

public class CachedDictionary<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, Cache<TValue>> Caches;
    public readonly int TTLHours;
    public CachedDictionary(int ttl)
    {
        this.TTLHours = ttl;
        this.Caches = new Dictionary<TKey, Cache<TValue>>();
    }

    public CachedDictionary()
    {
        this.TTLHours = 2;
        this.Caches = new Dictionary<TKey, Cache<TValue>>();
    }

    public TValue? Get(TKey key)
    {
        Cache<TValue> value;
        if (Caches.TryGetValue(key, out value))
        {
            if (value.TimeStamp > DateTime.Now)
            {
                Caches.Remove(key);
                return default;
            }

            return value.Item;
        }
        else
        {
            return default;
        }
    }

    public void Remove(TKey key)
    {
        this.Caches.Remove(key);
    }
    
    public void Set(TKey key, TValue value)
    {
        this.Caches.Add(key, new Cache<TValue>(value, DateTime.Now.AddHours(TTLHours)));
    }

    public bool Has(TKey key)
    {
        return Caches.ContainsKey(key);
    }
    
}

class Cache<V>
{
    public readonly V Item;
    public readonly DateTime TimeStamp;
    
    public Cache(V item, DateTime timeStamp)
    {
        Item = item;
        TimeStamp = timeStamp;
    }
}