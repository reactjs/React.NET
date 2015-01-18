/*
 *  Copyright (c) 2014-2015, Facebook, Inc.
 *  All rights reserved.
 *
 *  This source code is licensed under the BSD-style license found in the
 *  LICENSE file in the root directory of this source tree. An additional grant 
 *  of patent rights can be found in the PATENTS file in the same directory.
 */

using System;

using Microsoft.Owin.Hosting;

namespace React.Sample.Owin
{
	class Program
	{
		static void Main(string[] args)
		{
			using (WebApp.Start<Startup>("http://localhost:12345"))
			{
				Console.WriteLine("Running on localhost:12345, press enter to quit");
				Console.ReadLine();
			}
		}
	}
}
