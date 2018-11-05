/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using React.TinyIoC;
using RegisterOptions = React.TinyIoC.TinyIoCContainer.RegisterOptions;

namespace React
{
	/// <summary>
	/// ReactJS.NET extensions to TinyIoC
	/// </summary>
	public static class TinyIoCExtensions
	{
		/// <summary>
		/// Gets or sets the factory used to create per-request lifetime providers
		/// </summary>
		internal static Func<RegisterOptions, RegisterOptions> AsRequestLifetime { private get; set; }

		/// <summary>
		/// Registers a class in IoC that uses a singleton per "request". This is generally in the
		/// context of a web request.
		/// </summary>
		/// <param name="registerOptions">Class registration options</param>
		/// <returns>The class registration (fluent interface)</returns>
		public static TinyIoCContainer.RegisterOptions AsPerRequestSingleton(this TinyIoCContainer.RegisterOptions registerOptions)
		{
			if (AsRequestLifetime == null)
			{
				throw new Exception(
					"AsRequestLifetime needs to be set for per-request ReactJS.NET " +
					"assembly registrations to work. Please ensure you are calling " +
					"React.Initializer.Initialize() before using any ReactJS.NET functionality."
				);
			}

			return AsRequestLifetime(registerOptions);
		}
	}
}
