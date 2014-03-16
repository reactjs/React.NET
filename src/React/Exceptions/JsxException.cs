using System;

namespace React.Exceptions
{
	/// <summary>
	/// Thrown when an error occurs with parsing JSX.
	/// </summary>
	public class JsxException : ReactException
	{
		public JsxException() : base() { }
		public JsxException(string message) : base(message) { }
		public JsxException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
