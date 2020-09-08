/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

#if NETCOREAPP

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace React.Tests.Common
{
	/// <summary>
	/// For unit tests / benchmarking only! Some cache evicting logic has been stripped out.
	/// </summary>
	public class MemoryFileCacheCore : ICache
	{
		private readonly IMemoryCache _cache;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryFileCacheCore" /> class.
		/// </summary>
		/// <param name="cache">The cache to use</param>
		/// <param name="hostingEnv">The ASP.NET hosting environment.</param>
		public MemoryFileCacheCore()
		{
			_cache = new MemoryCache(new MemoryCacheOptions());
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

			var options = new MemoryCacheEntryOptions
			{
				SlidingExpiration = slidingExpiration,
			};

			_cache.Set(key, data, options);
		}
	}
}

#endif
