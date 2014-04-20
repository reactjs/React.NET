/*
 *  Copyright (c) 2014, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Linq;
using JavaScriptEngineSwitcher.Core;
using Newtonsoft.Json;

using OriginalJsEngine = VroomJs.JsEngine;
using OriginalJsException = VroomJs.JsException;

namespace React.JavaScriptEngine.VroomJs
{
	/// <summary>
	/// Basic implementation of a VroomJs JavaScript engine for JavaScriptEngineSwitcher.
	/// Connects to the V8 JavaScript engine on Linux and Mac OS X.
	/// </summary>
	public class VroomJsEngine : JsEngineBase
	{
		/// <summary>
		/// The VroomJs engine
		/// </summary>
		private readonly OriginalJsEngine _jsEngine;

		/// <summary>
		/// Name of JavaScript engine
		/// </summary>
		public override string Name
		{
			get { return "VroomJs JavaScript engine"; }
		}

		/// <summary>
		/// Version of original JavaScript engine
		/// </summary>
		public override string Version
		{
			get { return "86f8558d (Aug 17, 2013)"; }
		}

		/// <summary>
		/// Constructs instance of adapter for VroomJs
		/// </summary>
		public VroomJsEngine()
		{
			try
			{
				_jsEngine = new OriginalJsEngine();
			}
			catch (Exception e)
			{
				
				throw new JsEngineLoadException(
					string.Format(
						JavaScriptEngineSwitcher.Core.Resources.Strings.Runtime_JsEngineNotLoaded,
						Name,
						e.Message
					), 
					Name,
					Version,
					e
				);
			}
		}

		/// <summary>
		/// Converts a VroomJs exception into a <see cref="JsRuntimeException" />.
		/// </summary>
		/// <param name="jsException">VroomJs exception</param>
		/// <returns>JavaScriptEngineSwitcher exception</returns>
		private JsRuntimeException ConvertJavaScriptExceptionToJsRuntimeException(
			OriginalJsException jsException	
		)
		{
			dynamic nativeEx = jsException.NativeException;
			return new JsRuntimeException(nativeEx.stack, Name, Version)
			{
				Category = nativeEx.name,
			};
		}

		/// <summary>
		/// Evaluates the specified JavaScript expression
		/// </summary>
		/// <param name="expression">Expression to evaluate</param>
		/// <returns>Result of the expression</returns>
		protected override object InnerEvaluate(string expression)
		{
			try
			{
				return _jsEngine.Execute(expression);
			}
			catch (OriginalJsException ex)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(ex);
			}
		}

		/// <summary>
		/// Evaluates the specified JavaScript expression
		/// </summary>
		/// <param name="expression">Expression to evaluate</param>
		/// <typeparam name="T">Return type</typeparam>
		/// <returns>Result of the expression</returns>
		protected override T InnerEvaluate<T>(string expression)
		{
			return (T) InnerEvaluate(expression);
		}

		/// <summary>
		/// Executes the specified JavaScript code
		/// </summary>
		/// <param name="code">Code to execute</param>
		protected override void InnerExecute(string code)
		{
			try
			{
				_jsEngine.Execute(code);
			}
			catch (OriginalJsException ex)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(ex);
			}
		}

		/// <summary>
		/// Calls the specified JavaScript function
		/// </summary>
		/// <param name="functionName">Function to call</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <returns>Result of the function</returns>
		protected override object InnerCallFunction(string functionName, params object[] args)
		{
			var code = string.Format(
				"{0}({1})",
				functionName,
				string.Join(", ", args.Select(JsonConvert.SerializeObject))
			);

			try
			{
				return _jsEngine.Execute(code);
			}
			catch (OriginalJsException ex)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(ex);
			}
		}

		/// <summary>
		/// Calls the specified JavaScript function
		/// </summary>
		/// <param name="functionName">Function to call</param>
		/// <param name="args">Arguments to pass to function</param>
		/// <typeparam name="T">Return type of the function</typeparam>
		/// <returns>Result of the function</returns>
		protected override T InnerCallFunction<T>(string functionName, params object[] args)
		{
			return (T) InnerCallFunction(functionName, args);
		}

		/// <summary>
		/// Determines if the specified variable has been set
		/// </summary>
		/// <param name="variableName">Name of the variable</param>
		/// <returns><c>true</c> if the variable is defined</returns>
		protected override bool InnerHasVariable(string variableName)
		{
			var code = string.Format("typeof {0} !== 'undefined'", variableName);
			return InnerEvaluate<bool>(code);
		}

		/// <summary>
		/// Gets the value of the specified variable
		/// </summary>
		/// <param name="variableName">Variable to get value of</param>
		/// <returns>Value</returns>
		protected override object InnerGetVariableValue(string variableName)
		{
			try
			{
				return _jsEngine.GetVariable(variableName);
			}
			catch (OriginalJsException ex)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(ex);
			}
		}

		/// <summary>
		/// Gets the value of the specified variable
		/// </summary>
		/// <param name="variableName">Variable to get value of</param>
		/// <typeparam name="T">Type of the variable</typeparam>
		/// <returns>Value</returns>
		protected override T InnerGetVariableValue<T>(string variableName)
		{
			return (T)InnerGetVariableValue(variableName);
		}

		/// <summary>
		/// Sets the value of the specified variable
		/// </summary>
		/// <param name="variableName">Variable to get value of</param>
		/// <param name="value">New value to set</param>
		/// <returns>Value</returns>
		protected override void InnerSetVariableValue(string variableName, object value)
		{
			try
			{
				_jsEngine.SetVariable(variableName, value);
			}
			catch (OriginalJsException ex)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(ex);
			}
		}

		/// <summary>
		/// Deletes a variable
		/// </summary>
		/// <param name="variableName">Variable to delete</param>
		protected override void InnerRemoveVariable(string variableName)
		{
			var code = string.Format("{0} = undefined", variableName);
			try
			{
				_jsEngine.Execute(code);
			}
			catch (OriginalJsException ex)
			{
				throw ConvertJavaScriptExceptionToJsRuntimeException(ex);
			}
		}

		/// <summary>
		/// Releases resources used by this engine.
		/// </summary>
		public override void Dispose()
		{
			if (!_disposed)
			{
				_jsEngine.Dispose();
				_disposed = true;
			}
		}
	}
}
