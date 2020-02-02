using System;
using System.Collections.Generic;
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

module.exports = function(callback, message) {{
	callback(null, wrappedCode());
}}";

		public static INodeJsEngine CreateEngine(INodeJSService nodeJSService)
		{
			return new NodeJsEngine(nodeJSService);
		}

		public string Name => throw new NotImplementedException();

		public string Version => throw new NotImplementedException();

		public T CallFunctionReturningJson<T>(string function, object[] args)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public T Evaluate<T>(string code)
		{
			return _nodeJSService.InvokeFromStringAsync<T>(WrapAsModule(code)).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		public void Execute(string contents, string file)
		{
			throw new NotImplementedException();
		}

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
			throw new NotImplementedException();
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
