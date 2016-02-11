/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Diagnostics;
using React;

namespace Cassette.React
{
	/// <summary>
	/// Cassette has two modes of operating - Web (ASP.NET) and MSBuild. IoC registration for web
	/// is already covered by React.Web. For the MSBuild mode, we need to initialise ReactJS.NET's
	/// IoC container here.
	/// </summary>
	public class CassetteMSBuildStartup : IStartUpTask
	{
		/// <summary>
		/// Handles initialisation of ReactJS.NET in Cassette. Only relevant when running in an
		/// MSBuild context.
		/// </summary>
		public void Start()
		{
			if (!MSBuildUtils.IsInMSBuild())
			{
				return;
			}

			// All "per-request" registrations should be singletons in MSBuild, since there's no
			// such thing as a "request"
			Initializer.Initialize(requestLifetimeRegistration: registration => registration.AsSingleton());
		}
	}
}
