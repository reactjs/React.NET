﻿/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(React.Sample.Mvc4.ReactConfig), "Configure")]

namespace React.Sample.Mvc4
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			ReactSiteConfiguration.Configuration
				.SetPreferredEngineName(JavaScriptEngineSwitcher.V8.V8JsEngine.EngineName)
				.SetReuseJavaScriptEngines(true)
				.AddScript("~/Content/Sample.jsx");
		}
	}
}
