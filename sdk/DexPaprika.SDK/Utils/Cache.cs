using System;
using System.Collections.Generic;
using System.Threading;

namespace DexPaprika.SDK.Utils
{
    /// <summary>
    /// Configuration for <see cref="Cache{T}"/>.
    /// </summary>
    public class CacheConfig
    {
        /// <summary>Time-to-live for each entry.</summary>
        public TimeSpan Ttl { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>Maximum number of entries held in the cache.</summary>
        public int MaxSize { get; set; } = 1000;
        /// <summary>Whether caching is active. Defaults to <c>true</c>.</summary>
        public bool Enabled { get; set; } = true;
    }

    /// <summary>
    /// Thread-safe LRU cache with per-entry TTL expiry.
    /// </summary>
    /// <remarks>
    /// Creates a new cache with the supplied configuration.
    /// </remarks>
    public class Cache<T>(CacheConfig? config = null)
    {
        private sealed class Entry(T data, long expiresAtTicks)
        {
            public T Data { get; set; } = data;
            public long ExpiresAtTicks { get; set; } = expiresAtTicks;
            public long LastAccessedTicks { get; set; } = DateTime.UtcNow.Ticks;
        }

        private readonly CacheConfig _config = config ?? new CacheConfig();
        private readonly Lock _lock = new();
        private readonly Dictionary<string, Entry> _store = [];

        /// <summary>Current number of entries (including potentially expired ones).</summary>
        public int Count
        {
            get { lock (_lock) { return _store.Count; } }
        }
        public bool Enabled { get => _config.Enabled; set => _config.Enabled = value; }

        /// <summary>
        /// Retrieves the value for <paramref name="key"/>, or returns <c>default</c>
        /// if the key is absent or the entry has expired.
        /// </summary>
        public T? Get(string key)
        {
            if (!_config.Enabled) return default;

            lock (_lock)
            {
                if (!_store.TryGetValue(key, out var entry))
                    return default;

                if (DateTime.UtcNow.Ticks > entry.ExpiresAtTicks)
                {
                    _store.Remove(key);
                    return default;
                }

                entry.LastAccessedTicks = DateTime.UtcNow.Ticks;
                return entry.Data;
            }
        }
        /// <summary>
        /// Stores a value under <paramref name="key"/>. No-ops when caching is disabled.
        /// </summary>
        public void Set(string key, T value)
        {
            if (!_config.Enabled) return;

            lock (_lock)
            {
                if (_store.Count >= _config.MaxSize)
                    EvictLru();

                var expiresAt = DateTime.UtcNow.Add(_config.Ttl).Ticks;
                _store[key] = new Entry(value, expiresAt);
            }
        }
        /// <summary>
        /// Returns <c>true</c> when <paramref name="key"/> exists and has not expired.
        /// </summary>
        public bool Has(string key)
        {
            if (!_config.Enabled) return false;

            lock (_lock)
            {
                if (!_store.TryGetValue(key, out var entry))
                    return false;

                if (DateTime.UtcNow.Ticks > entry.ExpiresAtTicks)
                {
                    _store.Remove(key);
                    return false;
                }

                return true;
            }
        }
        /// <summary>
        /// Removes the entry for <paramref name="key"/>.
        /// </summary>
        /// <returns><c>true</c> if the key was present.</returns>
        public bool Delete(string key)
        {
            lock (_lock)
            {
                return _store.Remove(key);
            }
        }
        /// <summary>
        /// Removes all entries from the cache.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _store.Clear();
            }
        }

        public bool TryGetValue(string key, out T? t)
        {
            t = default;

            if (!_config.Enabled) return false;

            lock (_lock)
            {
                if (!_store.TryGetValue(key, out Entry? entry))
                    return false;

                if (DateTime.UtcNow.Ticks > entry.ExpiresAtTicks)
                {
                    _store.Remove(key);
                    return false;
                }

                t = entry.Data;

                return true;
            }
        }

        /// <summary>
        /// Removes the least-recently-accessed entry. Caller must hold _lock.
        /// </summary>
        private void EvictLru()
        {
            string? oldestKey = null;
            long oldestTicks = long.MaxValue;

            foreach (var (key, entry) in _store)
            {
                if (entry.LastAccessedTicks < oldestTicks)
                {
                    oldestTicks = entry.LastAccessedTicks;
                    oldestKey = key;
                }
            }

            if (oldestKey != null)
                _store.Remove(oldestKey);
        }
    }
}
