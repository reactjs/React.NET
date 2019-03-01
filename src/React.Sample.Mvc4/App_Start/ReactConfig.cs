/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(React.Sample.Mvc4.ReactConfig), "Configure")]

namespace React.Sample.Mvc4
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(true)
				.SetAllowJavaScriptPrecompilation(true)
				.AddScriptWithoutTransform("~/Content/lib/reactstrap.min.js")
				.AddScript("~/Content/Sample.jsx");

			JsEngineSwitcher.Current.DefaultEngineName = V8JsEngine.EngineName;
			JsEngineSwitcher.Current.EngineFactories.AddV8();
		}
	}
}
