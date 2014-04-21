/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using React.TinyIoC;

namespace React.JavaScriptEngine.VroomJs
{
	/// <summary>
	/// Handles registration of VroomJS for ReactJS.NET.
	/// </summary>
	public class AssemblyRegistration : IAssemblyRegistration
	{
		/// <summary>
		/// Registers components in the React IoC container
		/// </summary>
		/// <param name="container">Container to register components in</param>
		public void Register(TinyIoCContainer container)
		{
			// Only supported on Linux or Mac
			if (!VroomJsUtils.IsEnvironmentSupported())
			{
				return;
			}

			JavaScriptEngineFactory.AddFactoryWithPriority(() => new VroomJsEngine(), priority: 10);
			VroomJsUtils.EnsureEngineFunctional();
		}
	}
}
