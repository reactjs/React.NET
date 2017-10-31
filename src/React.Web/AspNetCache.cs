/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;

namespace React.Web
{
	/// <summary>
	/// Implementation of <see cref="ICache"/> using ASP.NET cache.
	/// </summary>
	public class AspNetCache : ICache
	{
		/// <summary>
		/// The ASP.NET cache
		/// </summary>
		private readonly Cache _cache;

		/// <summary>
		/// Initializes a new instance of the <see cref="AspNetCache"/> class.
		/// </summary>
		/// <param name="cache">The Web application cache</param>
		public AspNetCache(Cache cache)
		{
			_cache = cache;
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
			return (T)(_cache[key] ?? fallback);
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
		public void Set<T>(
			string key,
			T data,
			TimeSpan slidingExpiration,
			IEnumerable<string> cacheDependencyFiles = null
		)
		{
			var cacheDependency = new CacheDependency(
				(cacheDependencyFiles ?? Enumerable.Empty<string>()).ToArray()
			);
			_cache.Insert(key, data, cacheDependency, Cache.NoAbsoluteExpiration, slidingExpiration);
		}
	}
}
