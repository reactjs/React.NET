using System;

namespace React.Exceptions
{
	/// <summary>
	/// Base class for all React.NET exceptions
	/// </summary>
	public class ReactException : Exception
	{
		public ReactException() : base() { }
		public ReactException(string message) : base(message) { }
		public ReactException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
