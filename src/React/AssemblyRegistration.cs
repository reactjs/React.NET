/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using JavaScriptEngineSwitcher.Msie;
using JavaScriptEngineSwitcher.Msie.Configuration;
using React.TinyIoC;

namespace React
{
	/// <summary>
	/// Handles registration of core React.NET components.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Gets the IoC container. Try to avoid using this and always use constructor injection.
		/// This should only be used at the root level of an object heirarchy.
		/// </summary>
		public static TinyIoCContainer Container
		{
			get { return TinyIoCContainer.Current; }
		}

		/// <summary>
		/// Registers standard components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			// One instance shared for the whole app
			container.Register<IReactSiteConfiguration>(ConfigurationFactory.GetConfiguration);
			// Force MSIE to use Chakra ActiveScript engine.
			// Chakra JS RT engine runs out of stack space when processing JSX
			container.Register<MsieConfiguration>(new MsieConfiguration
			{
				EngineMode = JsEngineMode.ChakraActiveScript
			});

			container.Register<IReactEnvironment, ReactEnvironment>().AsReactSingleton();
			container.Register<IJavaScriptEngineFactory, JavaScriptEngineFactory>().AsReactSingleton();
		}
	}
}
