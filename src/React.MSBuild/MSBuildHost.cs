/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */
using System;
using System.Diagnostics;
using System.Reflection;

namespace React.MSBuild
{
	/// <summary>
	/// Handles initialisation of the MSBuild environment.
	/// </summary>
	internal static class MSBuildHost
	{
		/// <summary>
		/// Hack to use Lazy{T} for thread-safe, once-off initialisation :)
		/// </summary>
		private readonly static Lazy<bool> _initializer = new Lazy<bool>(Initialize); 

		/// <summary>
		/// Ensures the environment has been initialised.
		/// </summary>
		public static bool EnsureInitialized()
		{
			return _initializer.Value;
		}

		/// <summary>
		/// Actually perform the initialisation of the environment.
		/// </summary>
		/// <returns></returns>
		private static bool Initialize()
		{
			AssemblyBindingRedirect.Enable();

			// All "per-request" registrations should be singletons in MSBuild, since there's no
			// such thing as a "request"
			Initializer.Initialize(requestLifetimeRegistration: registration => registration.AsSingleton());

			return true;
		}

		/// <summary>
		/// Determines if the current process is MSBuild
		/// </summary>
		/// <returns><c>true</c> if we are currently in MSBuild</returns>
		public static bool IsInMSBuild()
		{
			try
			{
				return Process.GetCurrentProcess().ProcessName.StartsWith("MSBuild");
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
