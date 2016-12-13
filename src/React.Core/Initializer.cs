/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RegisterOptions = React.TinyIoC.TinyIoCContainer.RegisterOptions;

#if !NET40
using Microsoft.Extensions.DependencyModel;
#endif

namespace React
{
	/// <summary>
	/// Handles initialisation of ReactJS.NET. This is only called once, at application start.
	/// </summary>
	public static class Initializer
	{
		/// <summary>
		/// Assemblies that are ignored when finding <see cref="IAssemblyRegistration"/>
		/// implementations to register dependencies.
		/// </summary>
		private readonly static ISet<string> _obsoleteAssemblies = new HashSet<string>
		{
			"React.JavaScriptEngine.VroomJs",
			"React.JavaScriptEngine.ClearScriptV8",
		};

		/// <summary>
		/// Intialise ReactJS.NET
		/// </summary>
		/// <param name="requestLifetimeRegistration">
		/// A function used to register IoC components with a per-request lifetime
		/// </param>
		public static void Initialize(Func<RegisterOptions, RegisterOptions> requestLifetimeRegistration)
		{
			InitializeIoC(requestLifetimeRegistration);
		}

		/// <summary>
		/// Initialises the IoC container by finding all <see cref="IAssemblyRegistration"/> 
		/// implementations and calling their <see cref="IAssemblyRegistration.Register"/> methods.
		/// </summary>
		/// <param name="requestLifetimeRegistration">
		/// A function used to register IoC components with a per-request lifetime
		/// </param>
		private static void InitializeIoC(Func<RegisterOptions, RegisterOptions> requestLifetimeRegistration)
		{
			TinyIoCExtensions.AsRequestLifetime = requestLifetimeRegistration;
#if NET40
			var types = AppDomain.CurrentDomain.GetAssemblies()
				// Only bother checking React assemblies
				.Where(IsReactAssembly)
#else
			var types = DependencyContext.Default.RuntimeLibraries
				.Where(library => IsReactAssembly(library.Name))
				.Select(library => Assembly.Load(new AssemblyName(library.Name)))
#endif
				.SelectMany(assembly => assembly.GetTypes())
				.Where(IsComponentRegistration);

			foreach (var type in types)
			{
				var reg = (IAssemblyRegistration)Activator.CreateInstance(type);
				reg.Register(React.AssemblyRegistration.Container);
			}
		}

		/// <summary>
		/// Determines if the specified assembly is part of ReactJS.NET
		/// </summary>
		/// <param name="assembly">The assembly</param>
		/// <returns>
		///   <c>true</c> if this is a ReactJS.NET assembly; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsReactAssembly(Assembly assembly)
		{
			var nameWithoutVersion = assembly.FullName.Split(',')[0];
			return IsReactAssembly(nameWithoutVersion);
		}

		/// <summary>
		/// Determines if the specified assembly is part of ReactJS.NET
		/// </summary>
		/// <param name="assemblyName">Name of the assembly</param>
		/// <returns>
		///   <c>true</c> if this is a ReactJS.NET assembly; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsReactAssembly(string assemblyName)
		{
			return
				(assemblyName.Equals("React", StringComparison.OrdinalIgnoreCase) ||
				assemblyName.StartsWith("React.", StringComparison.OrdinalIgnoreCase) ||
				assemblyName.EndsWith(".React", StringComparison.OrdinalIgnoreCase)) &&
				!_obsoleteAssemblies.Contains(assemblyName, StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Determines whether the specified type is an assembly registration class
		/// </summary>
		/// <param name="type">The type to check</param>
		/// <returns>
		///   <c>true</c> if the type is a component registration class; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsComponentRegistration(Type type)
		{
			return type.GetInterfaces().Contains(typeof(IAssemblyRegistration));
		}
	}
}
