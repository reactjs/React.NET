using System;

namespace React
{
    /// <summary>
    /// Implementation of <see cref="IFileSystem"/> that throw <see cref="NotImplementedException"/>
    /// </summary>
    public class NullFileSystem : IFileSystem
    {
        /// <summary>
        /// Converts a path from an application relative path (~/...) to a full filesystem path
        /// </summary>
        /// <param name="relativePath">App-relative path of the file</param>
        /// <returns>Full path of the file</returns>
        public string MapPath(string relativePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the contents of a file as a string.
        /// </summary>
        /// <param name="relativePath">App-relative path of the file</param>
        /// <returns>Contents of the file</returns>
        public string ReadAsString(string relativePath)
        {
            throw new NotImplementedException();
        }
    }
}