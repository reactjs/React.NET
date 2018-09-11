using System;
using System.IO;

namespace React.Tests.Common
{
	public class PhysicalFileSystem : FileSystemBase
	{
		private string _baseDirPath;


		public PhysicalFileSystem()
			: this(AppDomain.CurrentDomain.BaseDirectory)
		{ }

		public PhysicalFileSystem(string baseDirPath)
		{
			_baseDirPath = baseDirPath;
		}


		public override string MapPath(string relativePath)
		{
			if (Path.IsPathRooted(relativePath))
			{
				return relativePath;
			}

			return Path.GetFullPath(Path.Combine(_baseDirPath, relativePath));
		}
	}
}
