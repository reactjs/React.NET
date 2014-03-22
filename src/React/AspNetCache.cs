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
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace React
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
		/// <param name="context">The HTTP context</param>
		public AspNetCache(HttpContextBase context)
		{
			_cache = context.Cache;
		}

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
		public T GetOrInsert<T>(
			string key, 
			TimeSpan slidingExpiration,
			Func<T> getData,
			IEnumerable<string> cacheDependencyFiles = null,
			IEnumerable<string> cacheDependencyKeys = null
		)
		{
			// Check for data in cache
			var data = (T)(_cache[key] ?? default(T));

			// http://stackoverflow.com/questions/65351/null-or-default-comparsion-of-generic-argument-in-c-sharp
			if (object.Equals(data, default(T)))
			{
				// Load data and save into cache
				data = getData();
				var cacheDependency = new CacheDependency(
					(cacheDependencyFiles ?? Enumerable.Empty<string>()).ToArray(),
					(cacheDependencyKeys ?? Enumerable.Empty<string>()).ToArray()
				);
				_cache.Insert(key, data, cacheDependency, Cache.NoAbsoluteExpiration, slidingExpiration);
			}

			return data;
		}
	}
}
