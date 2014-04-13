/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Diagnostics;
using Newtonsoft.Json;
using React.Exceptions;

namespace React
{
	/// <summary>
	/// Handles compiling JSX to JavaScript.
	/// </summary>
	public class JsxTransformer : IJsxTransformer
	{
		/// <summary>
		/// Cache key for JSX to JavaScript compilation
		/// </summary>
		private const string JSX_CACHE_KEY = "JSX_{0}";

		/// <summary>
		/// Environment this JSX Transformer has been created in
		/// </summary>
		private readonly IReactEnvironment _environment;
		/// <summary>
		/// Cache used for storing compiled JSX
		/// </summary>
		private readonly ICache _cache;
		/// <summary>
		/// File system wrapper
		/// </summary>
		private readonly IFileSystem _fileSystem;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsxTransformer"/> class.
		/// </summary>
		/// <param name="environment">The ReactJS.NET environment</param>
		/// <param name="cache">The cache to use for JSX compilation</param>
		/// <param name="fileSystem">File system wrapper</param>
		public JsxTransformer(IReactEnvironment environment, ICache cache, IFileSystem fileSystem)
		{
			_environment = environment;
			_cache = cache;
			_fileSystem = fileSystem;
		}

		/// <summary>
		/// Loads a JSX file. Results of the JSX to JavaScript transformation are cached.
		/// </summary>
		/// <param name="filename">Name of the file to load</param>
		/// <returns>File contents</returns>
		public string LoadJsxFile(string filename)
		{
			var fullPath = _fileSystem.MapPath(filename);

			return _cache.GetOrInsert(
				key: string.Format(JSX_CACHE_KEY, filename),
				slidingExpiration: TimeSpan.FromMinutes(30),
				cacheDependencyFiles: new[] { fullPath },
				getData: () =>
				{
					Trace.WriteLine(string.Format("Parsing JSX from {0}", filename));
					var contents = _fileSystem.ReadAsString(filename);
					return TransformJsx(contents);
				}
			);
		}

		/// <summary>
		/// Transforms JSX into regular JavaScript. The result is not cached. Use 
		/// <see cref="LoadJsxFile"/> if loading from a file since this will cache the result.
		/// </summary>
		/// <param name="input">JSX</param>
		/// <returns>JavaScript</returns>
		public string TransformJsx(string input)
		{
			// Just return directly if there's no JSX annotation
			if (!input.Contains("@jsx"))
			{
				return input;
			}

			try
			{
				var encodedInput = JsonConvert.SerializeObject(input);
				var output = _environment.ExecuteWithLargerStackIfRequired<string>(string.Format(
					"global.JSXTransformer.transform({0}).code",
					encodedInput
				));
				return output;
			}
			catch (Exception ex)
			{
				throw new JsxException(ex.Message, ex);
			}
		}
	}
}
