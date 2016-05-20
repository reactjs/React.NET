/*
 *  Copyright (c) 2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.Http;
using React.Exceptions;
using React.TinyIoC;
using Microsoft.Extensions.DependencyInjection;

namespace React.AspNet
{
	/// <summary>
	/// Handles registering per-request objects in the dependency injection container.
	/// </summary>
	internal class HttpContextLifetimeProvider : TinyIoCContainer.ITinyIoCObjectLifetimeProvider
	{
		private readonly IServiceProvider _appServiceProvider;

		/// <summary>
		/// Creates a new <see cref="HttpContextLifetimeProvider" />.
		/// </summary>
		/// <param name="appServiceProvider">ASP.NET dependency injection service provider</param>
		public HttpContextLifetimeProvider(IServiceProvider appServiceProvider)
		{
			_appServiceProvider = appServiceProvider;
		}

		/// <summary>
		/// Prefix to use on HttpContext items
		/// </summary>
		private const string PREFIX = "React.PerRequest.";

		/// <summary>
		/// Name of the key for this particular registration
		/// </summary>
		private readonly string _keyName = PREFIX + Guid.NewGuid();

		/// <summary>
		/// Gets the current per-request registrations for the current request.
		/// </summary>
		private PerRequestRegistrations Registrations
		{
			get
			{
				var registrations = _appServiceProvider.GetService<PerRequestRegistrations>();
				if (registrations == null)
				{
					throw new ReactNotInitialisedException(
						"ReactJS.NET has not been initialised correctly. Please ensure you have " +
						"called app.AddReact() and app.UseReact() in your Startup.cs file."
					);
				}
				return registrations;
			}
		}

		/// <summary>
		/// Gets the value of this item in the dependency injection container.
		/// </summary>
		/// <returns></returns>
		public object GetObject()
		{
			object value;
			Registrations.TryGetValue(_keyName, out value);
			return value;
		}

		/// <summary>
		/// Sets the value of this item in the dependency injection container.
		/// </summary>
		/// <param name="value">Value to set</param>
		public void SetObject(object value)
		{
			Registrations[_keyName] = value;
		}

		/// <summary>
		/// Removes this item from the dependency injection container.
		/// </summary>
		public void ReleaseObject()
		{
			object value;
			if (Registrations.TryRemove(_keyName, out value))
			{
				if (value is IDisposable)
				{
					((IDisposable)value).Dispose();
				}
			}
		}

		/// <summary>
		/// Contains all per-request dependency injection registrations.
		/// </summary>
		internal class PerRequestRegistrations : ConcurrentDictionary<string, object>, IDisposable
		{
			/// <summary>
			/// Disposes all registrations in this container.
			/// </summary>
			public void Dispose()
			{
				foreach (var kvp in this.Where(kvp => kvp.Value is IDisposable))
				{
					((IDisposable)kvp.Value).Dispose();
				}
			}
		}
	}
}