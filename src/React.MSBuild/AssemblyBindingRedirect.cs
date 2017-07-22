/*
 *  Copyright (c) 2017-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace React.MSBuild
{
	/// <summary>
	/// Hacks around the fact that it's not possible to do assembly binding redirects in MSBuild.
	/// 
	/// https://github.com/Microsoft/msbuild/issues/1309
	/// http://blog.slaks.net/2013-12-25/redirecting-assembly-loads-at-runtime/
	/// </summary>
	public static class AssemblyBindingRedirect
	{
		/// <summary>
		/// Redirects that have been configured
		/// </summary>
		private static readonly Dictionary<string, Version> _redirects = new Dictionary<string, Version>();

	    static AssemblyBindingRedirect()
	    {
			// This is in a static constructor because it needs to run as early as possible
		    ConfigureRedirect("JavaScriptEngineSwitcher.Core");
			AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
		}

		/// <summary>
		/// Enables assembly binding redirects
		/// </summary>
	    public static void Enable()
	    {
			// Intentionally empty.  This is just meant to ensure the static constructor
			// has run.
		}

		/// <summary>
		/// Configures a redirect for the specified assembly. Redirects to the version in the bin directory.
		/// </summary>
		/// <param name="name">Name of the assembly to redirect</param>
		private static void ConfigureRedirect(string name)
		{
			var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
			var redirectAssemblyPath = Path.Combine(
				Path.GetDirectoryName(currentAssemblyPath),
				name + ".dll"
			);

			try
			{
				var realAssembly = Assembly.LoadFile(redirectAssemblyPath);
				var version = realAssembly.GetName().Version;
				_redirects[name] = version;
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Warning: Could not determine version of " + name + " to use! " + ex.Message);
			}
		}

		/// <summary>
		/// Overrides assembly resolution to redirect if necessary.
		/// </summary>
		private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
		{
			var requestedAssembly = new AssemblyName(args.Name);

			if (_redirects.ContainsKey(requestedAssembly.Name) && requestedAssembly.Version != _redirects[requestedAssembly.Name])
			{
				requestedAssembly.Version = _redirects[requestedAssembly.Name];
				return Assembly.Load(requestedAssembly);
			}
			return null;
		}
	}
}
