---
title: "Use React and JSX in ASP.NET MVC"
layout: post
author: Daniel Lo Nigro
---
*Cross-posted from [the official React blog](https://reactjs.org/blog/2014/04/04/reactnet.html).*
____

Today we're happy to announce the initial release of
[ReactJS.NET](http://reactjs.net/), which makes it easier to use React and JSX
in .NET applications, focusing specifically on ASP.NET MVC web applications.
It has several purposes:

 - On-the-fly JSX to JavaScript compilation. Simply reference JSX files and they
   will be compiled and cached server-side.

   ```html
   <script src="@Url.Content("/Scripts/HelloWorld.jsx")"></script>
   ```
 - JSX to JavaScript compilation via popular minification/combination libraries
   (Cassette and ASP.NET Bundling and Minification). This is suggested for
   production websites.
 - Server-side component rendering to make your initial render super fast.

Even though we are focusing on ASP.NET MVC, ReactJS.NET can also be used in
Web Forms applications as well as non-web applications (for example, in build
scripts). ReactJS.NET currently only works on Microsoft .NET but we are working
on support for Linux and Mac OS X via Mono as well.

Installation
------------
ReactJS.NET is packaged in NuGet. Simply run `Install-Package React.Mvc4` in the
package manager console or search NuGet for "React" to install it.
[See the documentation](http://reactjs.net/docs) for more information. The
GitHub project contains
[a sample website](https://github.com/reactjs/React.NET/tree/master/src/React.Sample.Mvc4)
demonstrating all of the features.

Let us know what you think, and feel free to send through any feedback and
report bugs [on GitHub](https://github.com/reactjs/React.NET).
