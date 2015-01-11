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

using Microsoft.Owin.FileSystems;

namespace React.Owin
{
    /// <summary>
    /// Owin file system that serves transformed JSX files.
    /// </summary>
    internal class JsxFileSystem : Microsoft.Owin.FileSystems.IFileSystem
    {
        private readonly IJsxTransformer _transformer;
        private readonly Microsoft.Owin.FileSystems.IFileSystem _physicalFileSystem;
        private readonly string[] _extenstions;

        public JsxFileSystem(IJsxTransformer transformer, string root, IEnumerable<string> extenstions)
            : this(transformer, new PhysicalFileSystem(root), extenstions)
        {            
        }

        public JsxFileSystem(IJsxTransformer transformer, Microsoft.Owin.FileSystems.IFileSystem fileSystem, IEnumerable<string> extenstions)
        {
            _transformer = transformer;
            _physicalFileSystem = fileSystem;

            // Make sure the extensions start with dot
            _extenstions = extenstions.Select(extenstion => extenstion.StartsWith(".") ? extenstion : "." + extenstion).ToArray();
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            IFileInfo internalFileInfo;
            fileInfo = null;

            if (!_physicalFileSystem.TryGetFileInfo(subpath, out internalFileInfo))
                return false;

            if (internalFileInfo.IsDirectory || !_extenstions.Any(internalFileInfo.Name.EndsWith))
                return false;

            fileInfo = new JsxFileInfo(_transformer, internalFileInfo);
            return true;
        }

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            return _physicalFileSystem.TryGetDirectoryContents(subpath, out contents);
        }

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
                    () =>
                    {
                        return Encoding.UTF8.GetBytes(_jsxTransformer.TransformJsxFile(fileInfo.PhysicalPath));
                    });
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
                get { return _fileInfo.PhysicalPath; }
            }

            public string Name
            {
                get { return _fileInfo.Name; }
            }

            public DateTime LastModified
            {
                get { return _fileInfo.LastModified; }
            }

            public bool IsDirectory
            {
                get { return _fileInfo.IsDirectory; }
            }
        }
    }
}