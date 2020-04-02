﻿/*
 * Copyright (c) Facebook, Inc. and its affiliates.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;
using System.Runtime.InteropServices;

namespace React
{
	/// <summary>
	/// Utility functions for handling system environmental differences
	/// </summary>
	public static class SystemEnvironmentUtils
	{
#if NET40
		[DllImport("libc")]
		private static extern int uname(IntPtr buf);
#endif

		/// <summary>
		/// Determines whether the application is running on Mac OS.
		/// Based off Mono's XplatUI.cs, licensed under LGPL.
		/// </summary>
		/// <returns><c>true</c> if running on Mac OS</returns>
		public static bool IsRunningOnMac()
		{
			return _isRunningOnMac.Value;
		}

		private readonly static Lazy<bool> _isRunningOnMac = new Lazy<bool>(() =>
		{
#if NET40
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				return false;
			}

			var buf = IntPtr.Zero;
			try
			{
				buf = Marshal.AllocHGlobal(8192);
				if (uname(buf) == 0)
				{
					string os = Marshal.PtrToStringAnsi(buf);
					if (os == "Darwin")
						return true;
				}
			}
			catch
			{
				// YOLO
			}
			finally
			{
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal(buf);
			}
			return false;
#else
			// This code was specifically to work around a bug in Mono. If running under .NET Core, we
			// don't need to worry.
			return false;
#endif
		});
	}
}
