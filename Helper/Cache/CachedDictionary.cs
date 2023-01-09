namespace Cafet_Backend.Helper.Cache;

public class CachedDictionary<K, V>
{
    private readonly Dictionary<K, Cache<V>> Caches;
    public readonly int TTLHours;
    public CachedDictionary(int ttl)
    {
        this.TTLHours = ttl;
        this.Caches = new Dictionary<K, Cache<V>>();
    }

    public CachedDictionary()
    {
        this.TTLHours = 2;
        this.Caches = new Dictionary<K, Cache<V>>();
    }

    public V? Get(K key)
    {
        Cache<V> cache = Caches[key];
        if (cache == null)
            return default;

        if (cache.TimeStamp > DateTime.Now)
        {
            Caches.Remove(key);
            return default;
        }

        return cache.Item;
    }

    public void Set(K key, V value)
    {
        this.Caches.Add(key, new Cache<V>(value));
    }

    public bool Has(K key)
    {
        return Caches.ContainsKey(key);
    }
    
}

class Cache<V>
{
    public readonly V Item;
    public readonly DateTime TimeStamp;

    public Cache(V value)
    {
        this.Item = value;
        this.TimeStamp = DateTime.Now;
        
    }
}