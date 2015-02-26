/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if OWIN
using Microsoft.Owin.FileSystems;
using IOwinFileSystem = Microsoft.Owin.FileSystems.IFileSystem;
#else
using Microsoft.AspNet.FileProviders;
using Microsoft.Framework.Expiration.Interfaces;
using IOwinFileSystem = Microsoft.AspNet.FileProviders.IFileProvider;
using PhysicalFileSystem = Microsoft.AspNet.FileProviders.PhysicalFileProvider;
#endif

#if OWIN
namespace React.Owin
#else
namespace React.AspNet5
#endif
{
	/// <summary>
	/// File system that serves transformed JSX files.
	/// </summary>
	public class JsxFileSystem : IOwinFileSystem
	{
		private readonly IJsxTransformer _transformer;
		private readonly IOwinFileSystem _physicalFileSystem;
		private readonly string[] _extensions;

		/// <summary>
		/// Creates a new instance of the JsxFileSystem.
		/// </summary>
		/// <param name="transformer">JSX transformer used to compile JSX files</param>
		/// <param name="root">The root directory</param>
		/// <param name="extensions">Extensions of files that will be treated as JSX files</param>
		public JsxFileSystem(IJsxTransformer transformer, string root, IEnumerable<string> extensions)
			: this(transformer, new PhysicalFileSystem(root), extensions)
		{            
		}

		/// <summary>
		/// Creates a new instance of the JsxFileSystem.
		/// </summary>
		/// <param name="transformer">JSX transformer used to compile JSX files</param>
		/// <param name="fileSystem">File system used to look up files</param>
		/// <param name="extensions">Extensions of files that will be treated as JSX files</param>
		public JsxFileSystem(IJsxTransformer transformer, IOwinFileSystem fileSystem, IEnumerable<string> extensions)
		{
			_transformer = transformer;
			_physicalFileSystem = fileSystem;

			if (extensions != null)
			{
				// Make sure the extensions start with dot
				_extensions = extensions.Select(extension => extension.StartsWith(".") ? extension : "." + extension).ToArray();
			}
		}

#if OWIN
		/// <summary>
		/// Locate a JSX file at the given path. 
		/// </summary>
		/// <param name="subpath">The path that identifies the file</param>
		/// <param name="fileInfo">The discovered file if any</param>
		/// <returns>
		/// True if a JSX file was located at the given path
		/// </returns>
		public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
		{
			IFileInfo internalFileInfo;
			fileInfo = null;

			if (!_physicalFileSystem.TryGetFileInfo(subpath, out internalFileInfo))
				return false;

			if (_extensions != null && !_extensions.Any(internalFileInfo.Name.EndsWith))
				return false;

			if (internalFileInfo.IsDirectory)
				return false;

			fileInfo = new JsxFileInfo(_transformer, internalFileInfo);
			return true;
		}

		/// <summary>
		/// Enumerate a directory at the given path, if any
		/// </summary>
		/// <param name="subpath">The path that identifies the directory</param>
		/// <param name="contents">The contents if any</param>
		/// <returns>
		/// True if a directory was located at the given path
		/// </returns>
		public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
		{
			return _physicalFileSystem.TryGetDirectoryContents(subpath, out contents);
		}
#else

		/// <summary>
		/// Locate a JSX file at the given path. 
		/// </summary>
		/// <param name="subpath">The path that identifies the file</param>
		/// <returns>The discovered file if any</returns>
		public IFileInfo GetFileInfo(string subpath)
		{
			var internalFileInfo = _physicalFileSystem.GetFileInfo(subpath);
			return new JsxFileInfo(_transformer, internalFileInfo);
		}

		/// <summary>
		/// Enumerate a directory at the given path, if any
		/// </summary>
		/// <param name="subpath">The path that identifies the directory</param>
		/// <returns>The contents if any</returns>
		public IDirectoryContents GetDirectoryContents(string subpath)
		{
			return _physicalFileSystem.GetDirectoryContents(subpath);
		}

		/// <summary>
		/// Creates a change trigger with the specified filter.
		/// </summary>
		/// <param name="filter">
		/// Filter string used to determine what files or folders to monitor. Example: **/*.cs, *.*, subFolder/**/*.cshtml.
		/// </param>
		/// <returns>
		/// An <see cref="IExpirationTrigger"/> that is triggered when a file matching <paramref name="filter"/> is added, modified or deleted.
		/// </returns>
		public IExpirationTrigger Watch(string filter)
		{
			return _physicalFileSystem.Watch(filter);
		}
#endif

		private class JsxFileInfo : IFileInfo
		{
			private readonly IJsxTransformer _jsxTransformer;
			private readonly IFileInfo _fileInfo;
			private readonly Lazy<byte[]> _content;

			public JsxFileInfo(IJsxTransformer jsxTransformer, IFileInfo fileInfo)
			{
				_jsxTransformer = jsxTransformer;
				_fileInfo = fileInfo;

				_content = new Lazy<byte[]>(
					() => Encoding.UTF8.GetBytes(_jsxTransformer.TransformJsxFile(fileInfo.PhysicalPath))
				);
			}

			public Stream CreateReadStream()
			{
				return new MemoryStream(_content.Value);
			}

			public long Length
			{
				get { return _content.Value.Length; }
			}

			public string PhysicalPath
			{
				get { return null; }
			}

			public string Name
			{
				get { return _fileInfo.Name; }
			}

			public bool IsDirectory
			{
				get { return _fileInfo.IsDirectory; }
			}

#if OWIN
			public DateTime LastModified
			{
				get { return _fileInfo.LastModified; }
			}
#else
			public DateTimeOffset LastModified
			{
				get { return _fileInfo.LastModified; }
			}

			public void WriteContent(byte[] content)
			{
				_fileInfo.WriteContent(content);
			}

			public void Delete()
			{
				_fileInfo.Delete();
			}

			public bool Exists
			{
				get { return _fileInfo.Exists; }
			}

			public bool IsReadOnly
			{
				get { return _fileInfo.IsReadOnly; }
			}
#endif
		}
	}
}