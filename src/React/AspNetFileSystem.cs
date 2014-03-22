using System.IO;
using System.Web;

namespace React
{
	/// <summary>
	/// Handles file system functionality, such as reading files. Maps all paths from 
	/// application-relative (~/...) to full paths using ASP.NET's MapPath method.
	/// </summary>
	public class AspNetFileSystem : IFileSystem
	{
		/// <summary>
		/// The ASP.NET server utilities
		/// </summary>
		private readonly HttpServerUtilityBase _serverUtility;

		/// <summary>
		/// Initializes a new instance of the <see cref="AspNetFileSystem"/> class.
		/// </summary>
		/// <param name="serverUtility">The server utility.</param>
		public AspNetFileSystem(HttpServerUtilityBase serverUtility)
		{
			_serverUtility = serverUtility;
		}

		/// <summary>
		/// Converts a path from an application relative path (~/...) to a full filesystem path
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Full path of the file</returns>
		public string MapPath(string relativePath)
		{
			return _serverUtility.MapPath(relativePath);
		}

		/// <summary>
		/// Reads the contents of a file as a string.
		/// </summary>
		/// <param name="relativePath">App-relative path of the file</param>
		/// <returns>Contents of the file</returns>
		public string ReadAsString(string relativePath)
		{
			return File.ReadAllText(MapPath(relativePath));
		}
	}
}
