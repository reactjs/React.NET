using System;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using React.TinyIoC;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(React.Initializer), "Initialize")]

namespace React
{
	/// <summary>
	/// Handles initialisation of React.NET. This is only called once, at application start.
	/// </summary>
	internal static class Initializer
	{
		/// <summary>
		/// Intialise React.NET
		/// </summary>
		public static void Initialize()
		{
			InitializeIoC();
			DynamicModuleUtility.RegisterModule(typeof(IocPerRequestDisposal));
		}

		/// <summary>
		/// Initialises the IoC container by finding all <see cref="IAssemblyRegistration"/> 
		/// implementations and calling their <see cref="IAssemblyRegistration.Register"/> methods.
		/// </summary>
		private static void InitializeIoC()
		{
			var types = AppDomain.CurrentDomain.GetAssemblies()
				// Only bother checking React assemblies
				.Where(IsReactAssembly)
				.SelectMany(assembly => assembly.GetTypes())
				.Where(IsComponentRegistration);

			foreach (var type in types)
			{
				var reg = (IAssemblyRegistration)Activator.CreateInstance(type);
				reg.Register(AssemblyRegistration.Container);
			}
		}

		/// <summary>
		/// Determines if the specified assembly is part of React.NET
		/// </summary>
		/// <param name="assembly">The assembly</param>
		/// <returns>
		///   <c>true</c> if this is a React.NET assembly; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsReactAssembly(Assembly assembly)
		{
			return assembly.FullName.StartsWith("React,") || assembly.FullName.StartsWith("React.");
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

		/// <summary>
		/// Handles disposing per-request IoC instances at the end of the request
		/// </summary>
		internal class IocPerRequestDisposal : IHttpModule
		{
			public void Init(HttpApplication context)
			{
				context.EndRequest += (sender, args) => HttpContextLifetimeProvider.DisposeAll();
			}
			public void Dispose() { }
		}
	}
}
