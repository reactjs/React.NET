/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using JavaScriptEngineSwitcher.V8;
using React.TinyIoC;

namespace React.JavaScriptEngine.ClearScriptV8
{
	/// <summary>
	/// Handles registration of ClearScript V8 for ReactJS.NET.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			// Only supported on Windows
			if (!ClearScriptV8Utils.IsEnvironmentSupported())
			{
				return;
			}

			ClearScriptV8Utils.EnsureEngineFunctional();
			container.Register(new JavaScriptEngineFactory.Registration
			{
				Factory = () => new V8JsEngine(),
				Priority = 10
			}, "ClearScriptV8");
		}
	}
}
