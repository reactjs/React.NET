/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;
using JavaScriptEngineSwitcher.Msie.Configuration;
using React.Exceptions;
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

			// Unique per request
			container.Register<IFileSystem, AspNetFileSystem>().AsPerRequestSingleton();
			container.Register<IReactEnvironment, ReactEnvironment>().AsPerRequestSingleton();
			container.Register<ICache, AspNetCache>().AsPerRequestSingleton();
			container.Register<IJsxHandler, JsxHandler>().AsPerRequestSingleton();
			container.Register<HttpContextBase>((c, o) => new HttpContextWrapper(HttpContext.Current));
			container.Register<HttpServerUtilityBase>((c, o) => c.Resolve<HttpContextBase>().Server);
			container.Register<HttpRequestBase>((c, o) => c.Resolve<HttpContextBase>().Request);
			container.Register<HttpResponseBase>((c, o) => c.Resolve<HttpContextBase>().Response);

			RegisterJavascriptEngine(container);
		}

		/// <summary>
		/// Registers the most appropriate JavaScript engine for the current environment.
		/// </summary>
		/// <param name="container">IoC container</param>
		private void RegisterJavascriptEngine(TinyIoCContainer container)
		{
			// TODO: Implement shared engines rather than creating a new one per request
			// Stateless scripts can reuse engines.
			var type = GetJavascriptEngineType(container);
			container.Register(typeof(IJsEngine), type).AsPerRequestSingleton();
		}

		/// <summary>
		/// Gets the type of the most appropriate JavaScript engine for the current environment
		/// </summary>
		/// <param name="container"></param>
		/// <returns></returns>
		private Type GetJavascriptEngineType(TinyIoCContainer container)
		{
			var availableEngines = new List<Type>
			{
				typeof(MsieJsEngine)
				// TODO: Add Jint
				// TODO: Add V8
			};
			foreach (var engineType in availableEngines)
			{
				IJsEngine engine = null;
				try
				{
					// Perform a sanity test to ensure this engine is usable
					engine = (IJsEngine) container.Resolve(engineType);
					if (engine.Evaluate<int>("1 + 1") == 2)
					{
						// Success! Use this one.
						return engineType;
					}
				}
				catch (Exception ex)
				{
					// This engine threw an exception, try the next one
					Trace.WriteLine(string.Format("Error initialising {0}: {1}", engineType, ex));
				}
				finally
				{
					if (engine != null)
					{
						engine.Dispose();
					}
				}
			}
			
			// Epic fail, none of the engines worked. Nothing we can do now.
			throw new ReactException("No usable JavaScript engine found :(");
		}
	}
}
