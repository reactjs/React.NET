/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
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
		private readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// Creates a new <see cref="HttpContextLifetimeProvider" />.
		/// </summary>
		public HttpContextLifetimeProvider(IHttpContextAccessor httpContextAccessor)
		{
			if (httpContextAccessor == null)
			{
				throw new ReactNotInitialisedException(
					"IHttpContextAccessor is not registered correctly. Please add it to your " +
					"application startup:\n" +
					"services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();"
				);
			}
			_httpContextAccessor = httpContextAccessor;
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
				var requestServices = _httpContextAccessor.HttpContext.RequestServices;
				var registrations = requestServices.GetService<PerRequestRegistrations>();
				if (registrations == null)
				{
					throw new ReactNotInitialisedException(
						"ReactJS.NET has not been initialised correctly. Please ensure you have " +
						"called services.AddReact() and app.UseReact() in your Startup.cs file."
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