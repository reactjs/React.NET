/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

#if NET40 || NET45
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace React
{
	/// <summary>
	/// Memory cache implementation for React.ICache. Uses System.Runtime.Caching.
	/// </summary>
	public class MemoryFileCache : ICache
	{
		private readonly ObjectCache _cache;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryFileCache"/> class.
		/// </summary>		
		public MemoryFileCache()
		{
			_cache = MemoryCache.Default;
		}

		/// <summary>
		/// Get an item from the cache. Returns <paramref name="fallback"/> if the item does
		/// not exist.
		/// </summary>
		/// <typeparam name="T">Type of data</typeparam>
		/// <param name="key">The cache key</param>
		/// <param name="fallback">Value to return if item is not in the cache</param>
		/// <returns>Data from cache, otherwise <paramref name="fallback"/></returns>
		public T Get<T>(string key, T fallback = default(T))
		{
			return (T)(_cache.Get(key) ?? fallback);
		}

		/// <summary>
		/// Sets an item in the cache.
		/// </summary>
		/// <typeparam name="T">Type of data</typeparam>
		/// <param name="key">The cache key</param>
		/// <param name="data">Data to cache</param>
		/// <param name="slidingExpiration">
		/// Sliding expiration, if cache key is not accessed in this time period it will 
		/// automatically be removed from the cache
		/// </param>
		/// <param name="cacheDependencyFiles">
		/// Filenames this cached item is dependent on. If any of these files change, the cache
		/// will be cleared automatically
		/// </param>
		public void Set<T>(string key, T data, TimeSpan slidingExpiration, IEnumerable<string> cacheDependencyFiles = null)
		{
			if (data == null)
			{
				_cache.Remove(key);
				return;
			}

			var policy = new CacheItemPolicy { SlidingExpiration = slidingExpiration };

			if (cacheDependencyFiles != null && cacheDependencyFiles.Any())
				policy.ChangeMonitors.Add(new HostFileChangeMonitor(cacheDependencyFiles.ToList()));

			_cache.Set(key, data, policy);
		}
	}
}
#endif
