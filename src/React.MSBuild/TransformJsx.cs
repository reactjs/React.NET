/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace React.MSBuild
{
	/// <summary>
	/// MSBuild task that handles transforming JSX to JavaScript
	/// </summary>
	public class TransformJsx : Task
	{
		/// <summary>
		/// The ReactJS.NET environment
		/// </summary>
		private IReactEnvironment _environment;

		/// <summary>
		/// Directory to process JSX files in. All subdirectories will be searched.
		/// </summary>
		[Required]
		public string SourceDir { get; set; }

		/// <summary>
		/// A value indicating if es6 syntax should be rewritten.
		/// </summary>
		/// <returns><c>true</c> if support for es6 syntax should be rewritten.</returns>
		public bool UseHarmony { get; set; }

		/// <summary>
		/// Gets or sets whether Flow types should be stripped.
		/// </summary>
		public bool StripTypes { get; set; }

		/// <summary>
		/// Executes the task.
		/// </summary>
		/// <returns><c>true</c> on success</returns>
		public override bool Execute()
		{
			MSBuildHost.EnsureInitialized();
			var config = React.AssemblyRegistration.Container.Resolve<IReactSiteConfiguration>();
			config.UseHarmony = UseHarmony;
			config.StripTypes = StripTypes;

			_environment = React.AssemblyRegistration.Container.Resolve<IReactEnvironment>();

			Log.LogMessage("Starting TransformJsx");
			var stopwatch = Stopwatch.StartNew();
			var result = ExecuteInternal();
			Log.LogMessage("TransformJsx completed in {0}", stopwatch.Elapsed);
			return result;
		}

		/// <summary>
		/// The core of the task. Locates all JSX files and transforms them to JavaScript.
		/// </summary>
		/// <returns><c>true</c> on success</returns>
		private bool ExecuteInternal()
		{
			var files = Directory.EnumerateFiles(SourceDir, "*.jsx", SearchOption.AllDirectories);
			foreach (var path in files)
			{
				var relativePath = path.Substring(SourceDir.Length + 1);
				Log.LogMessage(" -> Processing {0}", relativePath);
				_environment.JsxTransformer.TransformAndSaveJsxFile(path);
			}

			return true;
		}
	}
}
