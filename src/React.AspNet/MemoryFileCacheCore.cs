/*
 *  Copyright (c) 2016-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

#if NETSTANDARD1_6
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Microsoft.Extensions.FileProviders;

namespace React.AspNet
{
	/// <summary>
	/// Memory cache implementation for React.ICache. Uses IMemoryCache from .NET Core.
	/// </summary>
	public class MemoryFileCacheCore : ICache
	{
		private readonly IMemoryCache _cache;
		private readonly IFileProvider _fileProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryFileCacheCore" /> class.
		/// </summary>
		/// <param name="cache">The cache to use</param>
		/// <param name="fileProvider">The file provider.</param>
		public MemoryFileCacheCore(IMemoryCache cache, IFileProvider fileProvider)
		{
			_cache = cache;
			_fileProvider = fileProvider;
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
		/// <param name="cacheDependencyKeys">
		/// Other cache keys this cached item is dependent on. If any of these keys change, the
		/// cache will be cleared automatically
		/// </param>
		public void Set<T>(string key, T data, TimeSpan slidingExpiration, IEnumerable<string> cacheDependencyFiles = null, IEnumerable<string> cacheDependencyKeys = null)
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

			if (cacheDependencyFiles != null)
			{
				foreach (var file in cacheDependencyFiles)
				{
					options.AddExpirationToken(_fileProvider.Watch(file));
				}
			}

			if (cacheDependencyKeys != null && cacheDependencyKeys.Any())
			{
				// https://github.com/aspnet/Docs/issues/1938
				throw new NotImplementedException();
			}

			_cache.Set(key, data, options);
		}
	}
}
#endif
