/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace React
{
	internal class ReactAppAssetManifest
	{
		public Dictionary<string, string> Files { get; set; }
		public List<string> Entrypoints { get; set; }

		public static ReactAppAssetManifest LoadManifest(IReactSiteConfiguration config, IFileSystem fileSystem, ICache cache, bool useCacheRead)
		{
			string cacheKey = "REACT_APP_MANIFEST";

			if (useCacheRead)
			{
				var cachedManifest = cache.Get<ReactAppAssetManifest>(cacheKey);
				if (cachedManifest != null)
					return cachedManifest;
			}

			var manifestString = fileSystem.ReadAsString($"{config.ReactAppBuildPath}/asset-manifest.json");
			var manifest = JsonConvert.DeserializeObject<ReactAppAssetManifest>(manifestString);

			cache.Set(cacheKey, manifest, TimeSpan.FromHours(1));
			return manifest;
		}

		public static ReactAppAssetManifest LoadManifestFromWebpackDevServer(IReactSiteConfiguration _config)
		{
			var manifestString = FetchStringFromUrl(new Uri(_config.WebpackDevServerUrl + "asset-manifest.json"));
			return JsonConvert.DeserializeObject<ReactAppAssetManifest>(manifestString);
		}

		public static string FetchStringFromUrl(Uri url)
		{
			// Really wish we could use HttpClient here, but no async is a recipe for potential deadlocks
			// An async request pipeline would be needed to pull that off, which this library (today) has no concept of
			var request = WebRequest.Create(url);
			request.Timeout = 1000;
			{
				using (var response = (HttpWebResponse) request.GetResponse())
				{
					if (response.StatusCode != HttpStatusCode.OK)
						return null;

					return GetString(response);
				}
			}
		}

		private static string GetString(HttpWebResponse response)
		{
			using (var responseStream = response.GetResponseStream())
			{
				StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
				return reader.ReadToEnd();
			}
		}
	}
}
