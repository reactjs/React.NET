/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System.Diagnostics;
using System.IO;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace React.MSBuild
{
	/// <summary>
	/// MSBuild task that handles transforming JavaScript via Babel
	/// </summary>
	public class TransformBabel : Task
	{
		/// <summary>
		/// The ReactJS.NET environment
		/// </summary>
		private IReactEnvironment _environment;

		/// <summary>
		/// Directory to process JavaScript files in. All subdirectories will be searched.
		/// </summary>
		[Required]
		public string SourceDir { get; set; }

		/// <summary>
		/// The file extension pattern of the JavaScript files to transpile. Optional, defaults to *.jsx
		/// </summary>
		public string FileExtensionPattern { get; set; } = "*.jsx";

		/// <summary>
		/// Executes the task.
		/// </summary>
		/// <returns><c>true</c> on success</returns>
		public override bool Execute()
		{
			MSBuildHost.EnsureInitialized();
			var config = React.AssemblyRegistration.Container.Resolve<IReactSiteConfiguration>();
			config
				.SetReuseJavaScriptEngines(false);

			JsEngineSwitcher.Current.DefaultEngineName = MsieJsEngine.EngineName;
			JsEngineSwitcher.Current.EngineFactories.AddMsie();

			_environment = ReactEnvironment.Current;

			Log.LogMessage("Starting Babel transform");
			var stopwatch = Stopwatch.StartNew();
			var result = ExecuteInternal();
			Log.LogMessage("Babel transform completed in {0}", stopwatch.Elapsed);
			return result;
		}

		/// <summary>
		/// The core of the task. Locates all JSX files and transforms them to JavaScript.
		/// </summary>
		/// <returns><c>true</c> on success</returns>
		private bool ExecuteInternal()
		{
			var files = Directory.EnumerateFiles(SourceDir, FileExtensionPattern, SearchOption.AllDirectories);
			foreach (var path in files)
			{
				var relativePath = path.Substring(SourceDir.Length + 1);
				Log.LogMessage(" -> Processing {0}", relativePath);
				_environment.Babel.TransformAndSaveFile(path);
			}

			return true;
		}
	}
}
