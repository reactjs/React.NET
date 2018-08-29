#if NET461
using System;
using System.IO;

namespace React.Tests.Integration
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

			return Path.Combine(_baseDirPath, relativePath);
		}
	}
}
#endif
