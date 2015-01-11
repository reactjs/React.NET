﻿/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;

namespace React
{
	/// <summary>
	/// Implementation of <see cref="ICache"/> that never caches.
	/// </summary>
	public class NullCache : ICache
	{
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
			return fallback;
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
		/// <param name="cacheDependencyKeys">
		/// Other cache keys this cached item is dependent on. If any of these keys change, the
		/// cache will be cleared automatically
		/// </param>
		public void Set<T>(string key, T data, TimeSpan slidingExpiration, IEnumerable<string> cacheDependencyFiles = null,
			IEnumerable<string> cacheDependencyKeys = null)
		{
			// no-op
		}
	}
}