using System;
using System.Reflection;

namespace React
{
	/// <summary>
	/// The wrapper for the Node engine
	/// </summary>
	public interface INodeJsEngine
	{
		string Name { get; }
		string Version { get; }

		bool HasVariable(string uSER_SCRIPTS_LOADED_KEY);
		void Execute(string contents, string file);
		void SetVariableValue(string uSER_SCRIPTS_LOADED_KEY, bool v);
		void Execute(string code);
		T Evaluate<T>(string code);

		void ExecuteFile(IFileSystem fileSystem, string path);

		T CallFunctionReturningJson<T>(string function, object[] args);

		void ExecuteResource(string resourceName, Assembly assembly);
	}
}
