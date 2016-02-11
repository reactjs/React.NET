/*
 *  Copyright (c) 2014-Present, Facebook, Inc.
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
using Microsoft.Extensions.Primitives;
using IOwinFileSystem = Microsoft.AspNet.FileProviders.IFileProvider;
using PhysicalFileSystem = Microsoft.AspNet.FileProviders.PhysicalFileProvider;
#endif

#if OWIN
namespace React.Owin
#else
namespace React.AspNet
#endif
{
	/// <summary>
	/// File system that serves transformed JavaScript files.
	/// </summary>
	public class BabelFileSystem : IOwinFileSystem
	{
		private readonly IBabel _transformer;
		private readonly IOwinFileSystem _physicalFileSystem;
		private readonly string[] _extensions;

		/// <summary>
		/// Creates a new instance of the BabelFileSystem.
		/// </summary>
		/// <param name="transformer">Babel transformer used to compile files</param>
		/// <param name="root">The root directory</param>
		/// <param name="extensions">Extensions of files that will be treated as JavaScript files</param>
		public BabelFileSystem(IBabel transformer, string root, IEnumerable<string> extensions)
			: this(transformer, new PhysicalFileSystem(root), extensions)
		{            
		}

		/// <summary>
		/// Creates a new instance of the BabelFileSystem.
		/// </summary>
		/// <param name="transformer">Babel transformer used to compile files</param>
		/// <param name="fileSystem">File system used to look up files</param>
		/// <param name="extensions">Extensions of files that will be treated as JavaScript files</param>
		public BabelFileSystem(IBabel transformer, IOwinFileSystem fileSystem, IEnumerable<string> extensions)
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
		/// Locate a JavaScript file at the given path. 
		/// </summary>
		/// <param name="subpath">The path that identifies the file</param>
		/// <param name="fileInfo">The discovered file if any</param>
		/// <returns>
		/// True if a JavaScript file was located at the given path
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

			fileInfo = new BabelFileInfo(_transformer, internalFileInfo);
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
		/// Locate a file at the given path. 
		/// </summary>
		/// <param name="subpath">The path that identifies the file</param>
		/// <returns>The discovered file if any</returns>
		public IFileInfo GetFileInfo(string subpath)
		{
			var internalFileInfo = _physicalFileSystem.GetFileInfo(subpath);
			return new BabelFileInfo(_transformer, internalFileInfo);
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
		/// Creates a <see cref="T:Microsoft.Framework.Primitives.IChangeToken"/> for the 
		/// specified <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter">
		/// Filter string used to determine what files or folders to monitor. 
		/// Example: **/*.cs, *.*, subFolder/**/*.cshtml.</param>
		/// <returns>
		/// An <see cref="T:Microsoft.Framework.Primitives.IChangeToken"/> that is notified
		/// when a file matching <paramref name="filter"/> is added, modified or deleted.
		/// </returns>
		public IChangeToken Watch(string filter)
		{
			return _physicalFileSystem.Watch(filter);
		}
#endif

		private class BabelFileInfo : IFileInfo
		{
			private readonly IBabel _babel;
			private readonly IFileInfo _fileInfo;
			private readonly Lazy<byte[]> _content;

			public BabelFileInfo(IBabel babel, IFileInfo fileInfo)
			{
				_babel = babel;
				_fileInfo = fileInfo;

				_content = new Lazy<byte[]>(
					() => Encoding.UTF8.GetBytes(_babel.TransformFile(fileInfo.PhysicalPath))
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

			public bool Exists
			{
				get { return _fileInfo.Exists; }
			}
#endif
		}
	}
}
