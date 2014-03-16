using System;

namespace React.Exceptions
{
	/// <summary>
	/// Thrown when an error occurs while reading a site configuration file.
	/// </summary>
	public class ReactConfigurationException : ReactException
	{
		public ReactConfigurationException() : base() { }
		public ReactConfigurationException(string message) : base(message) { }
		public ReactConfigurationException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}
