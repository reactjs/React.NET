/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;

namespace React.Sample.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Initialize();

			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(false)
				.AddScript("Sample.jsx");

			JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;
			JsEngineSwitcher.Current.EngineFactories.AddChakraCore();

			var environment = ReactEnvironment.Current;
			var component = environment.CreateComponent("HelloWorld", new { name = "Daniel" });
			// renderServerOnly omits the data-reactid attributes
			var html = component.RenderHtml(renderServerOnly: true);

			Console.WriteLine(html);
			Console.ReadKey();
		}

		private static void Initialize()
		{
			Initializer.Initialize(registration => registration.AsSingleton());
			var container = React.AssemblyRegistration.Container;
			// Register some components that are normally provided by the integration library
			// (eg. React.AspNet or React.Web.Mvc4)
			container.Register<ICache, NullCache>();
			container.Register<IFileSystem, SimpleFileSystem>();
		}
	}
}
