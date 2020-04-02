/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

#if NETCOREAPP2_0 || NETSTANDARD2_0
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace React.AspNet
{
	/// <summary>
	/// Memory cache implementation for React.ICache. Uses IMemoryCache from .NET Core.
	/// </summary>
	public class MemoryFileCacheCore : ICache
	{
		private readonly IMemoryCache _cache;
		private readonly IWebHostEnvironment _hostingEnv;

		/// <summary>
		/// Initializes a new instance of the <see cref="MemoryFileCacheCore" /> class.
		/// </summary>
		/// <param name="cache">The cache to use</param>
		/// <param name="hostingEnv">The ASP.NET hosting environment.</param>
		public MemoryFileCacheCore(IMemoryCache cache, IWebHostEnvironment hostingEnv)
		{
			_cache = cache;
			_hostingEnv = hostingEnv;
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

			if (cacheDependencyFiles != null)
			{
				foreach (var file in cacheDependencyFiles)
				{
					var relativePath = file.Replace(_hostingEnv.WebRootPath, string.Empty).TrimStart('\\', '/');
					options.AddExpirationToken(_hostingEnv.WebRootFileProvider.Watch(relativePath));
				}
			}

			_cache.Set(key, data, options);
		}
	}
}
