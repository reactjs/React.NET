using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Jering.Javascript.NodeJS;
using Newtonsoft.Json;
using React;

namespace React.NodeServices
{
	public class NodeJsEngine : INodeJsEngine, IDisposable
	{
		private Dictionary<string, bool> _dict = new Dictionary<string, bool>();
		private object _lock = new object();
		private readonly INodeJSService _nodeJSService;
		private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii };

		private NodeJsEngine(INodeJSService nodeJSService)
		{
			_nodeJSService = nodeJSService;
		}

		private string WrapAsModule(string code) => $@"
let wrappedCode = () => vm.runInThisContext({JsonConvert.SerializeObject(code, _settings)});

module.exports = function(callback, ...args) {{
	callback(null, wrappedCode(args));
}}";

		public static INodeJsEngine CreateEngine(INodeJSService nodeJSService)
		{
			return new NodeJsEngine(nodeJSService);
		}

		public string Name => nameof(NodeJsEngine);

		public string Version => "0.0.1";

		public T CallFunctionReturningJson<T>(string function, object[] args)
		{
			return _nodeJSService.InvokeFromStringAsync<T>(WrapAsModule($"{function}(...{JsonConvert.SerializeObject(args ?? new object[0])})")).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public void Dispose()
		{
			_nodeJSService.Dispose();
		}

		public T Evaluate<T>(string code)
		{
			return _nodeJSService.InvokeFromStringAsync<T>(WrapAsModule(code)).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public void Execute(string contents, string file) => Execute(contents);

		public void Execute(string code)
		{
			_nodeJSService.InvokeFromStringAsync(WrapAsModule(code)).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public void ExecuteFile(IFileSystem fileSystem, string path)
		{
			_nodeJSService.InvokeFromStringAsync(WrapAsModule($"require(path.resolve({JsonConvert.SerializeObject(fileSystem.MapPath(path), _settings)}));")).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public void ExecuteResource(string resourceName, Assembly assembly)
		{
			string result;
			using(Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}
			Execute(result);
		}

		public bool HasVariable(string key)
		{
			lock(_lock)
			{
				return _dict.ContainsKey(key);
			}
		}

		public void SetVariableValue(string key, bool value)
		{
			lock(_lock)
			{
				_dict[key] = value;
			}
		}
	}
}
