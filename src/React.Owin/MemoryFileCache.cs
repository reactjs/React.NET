/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace React.Owin
{
    /// <summary>
    /// Memory cache implementation for React.Owin.ICache. Uses System.Runtime.Caching.
    /// </summary>
    internal class MemoryFileCache : ICache
    {
        private readonly ObjectCache _cache;

        public MemoryFileCache()
        {
            _cache = MemoryCache.Default;
        }

        public T Get<T>(string key, T fallback = default(T))
        {
            return (T)_cache.Get(key);
        }

        public void Set<T>(string key, T data, TimeSpan slidingExpiration, IEnumerable<string> cacheDependencyFiles = null, IEnumerable<string> cacheDependencyKeys = null)
        {
            if (data == null)
            {
                _cache.Remove(key);
                return;
            }

            var policy = new CacheItemPolicy { SlidingExpiration = slidingExpiration };

            if (cacheDependencyFiles != null && cacheDependencyFiles.Any())
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(cacheDependencyFiles.ToList()));

            if (cacheDependencyKeys != null && cacheDependencyKeys.Any())
                policy.ChangeMonitors.Add(_cache.CreateCacheEntryChangeMonitor(cacheDependencyKeys));

            _cache.Set(key, data, policy);
        }
    }
}