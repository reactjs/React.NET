/*
 *  Copyright (c) 2014, Facebook, Inc.
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
		/// Get an item from the cache. If it doesn't exist, call the function to load it
		/// </summary>
		/// <typeparam name="T">Type of data</typeparam>
		/// <param name="key">The cache key.</param>
		/// <param name="slidingExpiration">
		/// Sliding expiration, if cache key is not accessed in this time period it will 
		/// automatically be removed from the cache
		/// </param>
		/// <param name="getData">Function to load data to cache. Called if data isn't in the cache, or is stale</param>
		/// <param name="cacheDependencyFiles">
		/// Filenames this cached item is dependent on. If any of these files change, the cache
		/// will be cleared automatically
		/// </param>
		/// <param name="cacheDependencyKeys">
		/// Other cache keys this cached item is dependent on. If any of these keys change, the
		/// cache will be cleared automatically
		/// </param>
		/// <returns>Data</returns>
		public T GetOrInsert<T>(string key, TimeSpan slidingExpiration, Func<T> getData, IEnumerable<string> cacheDependencyFiles = null,
			IEnumerable<string> cacheDependencyKeys = null)
		{
			return getData();
		}
	}
}