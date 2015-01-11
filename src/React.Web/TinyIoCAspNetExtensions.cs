/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Linq;
using System.Web;
using React.TinyIoC;
using React.Web.Exceptions;

namespace React.Web.TinyIoC
{
	/// <summary>
	/// Scopes IoC registrations to the context of an ASP.NET web request. All instantiated
	/// components will be automatically disposed at the end of the request.
	/// </summary>
	public class HttpContextLifetimeProvider : TinyIoCContainer.ITinyIoCObjectLifetimeProvider
	{
		/// <summary>
		/// Prefix to use on HttpContext items
		/// </summary>
		private const string PREFIX = "TinyIoC.HttpContext.";
		/// <summary>
		/// Name of the key for this particular registration
		/// </summary>
		private readonly string _keyName = PREFIX + Guid.NewGuid();

		/// <summary>
		/// Gets the stored object if it exists, or null if not
		/// </summary>
		/// <returns>Object instance or null</returns>
		public object GetObject()
		{
			var context = HttpContext.Current;
			return context == null ? null : context.Items[_keyName];
		}

		/// <summary>
		/// Store the object
		/// </summary>
		/// <param name="value">Object to store</param>
		public void SetObject(object value)
		{
			var context = HttpContext.Current;
			if (context == null)
			{
				throw new ReactAspNetException(
					"Trying to store item in HttpContext.Current while not in an ASP.NET " +
					"request!"
				);
			}
			context.Items[_keyName] = value;
		}

		/// <summary>
		/// Release the object
		/// </summary>
		public void ReleaseObject()
		{
			var item = GetObject() as IDisposable;

			if (item != null)
				item.Dispose();

			SetObject(null);
		}

		/// <summary>
		/// Disposes all instantiated components
		/// </summary>
		public static void DisposeAll()
		{
			var items = HttpContext.Current.Items;
			var disposableItems = items.Keys.OfType<string>()
				.Where(key => key.StartsWith(PREFIX))
				.Select(key => items[key])
				.Where(item => item is IDisposable);

			foreach (var item in disposableItems)
			{
				((IDisposable)item).Dispose();
			}
		}
	}
}