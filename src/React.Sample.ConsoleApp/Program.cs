/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;

namespace React.Sample.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Initialize();

			var sampleScript = @"
var HelloWorld2 = React.createClass({
	render() {
		return (
			<div>
				Hello {this.props.name} (a second time)!
			</div>
		);
	}
});
			";

			ReactSiteConfiguration.Configuration
				.SetReuseJavaScriptEngines(false)
				.AddScriptLiteral("Sample2", sampleScript)                                                
				.AddScript("Sample.jsx");

			var environment = ReactEnvironment.Current;
			var component = environment.CreateComponent("HelloWorld", new { name = "Daniel" });
			
			// renderServerOnly omits the data-reactid attributes
			var html = component.RenderHtml(renderServerOnly: true);

			html += environment.CreateComponent("HelloWorld2", new { name = "aBe" })
				.RenderHtml(renderServerOnly: true);

			Console.WriteLine(html);
			Console.ReadKey();
		}

		private static void Initialize()
		{
			Initializer.Initialize(registration => registration.AsSingleton());
			var container = React.AssemblyRegistration.Container;
			// Register some components that are normally provided by the integration library
			// (eg. React.AspNet or React.Web.Mvc6)
			container.Register<ICache, NullCache>();
			container.Register<IFileSystem, SimpleFileSystem>();
		}
	}
}
