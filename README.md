React.NET
=========
React.NET is an experimental library that uses the power of Facebook's
[React](http://facebook.github.io/react/) library to render UI components on 
the server-side with C# as well as on the client. It utilises a JavaScript 
engine to run your component's code server-side. This allows you to reuse
the same logic on the client-side and server-side, and lets you create dynamic
JavaScript applications while keeping search engine optimisation in mind.

It is designed to be cross-platform and work on Linux via Mono as well as 
Microsoft .NET.

Bug reports and feature requests are welcome!

Requirements
============
 * ASP.NET 4.0 or higher
 * ASP.NET MVC 4 (support for other versions will come eventually)
 * [JSON.NET](http://james.newtonking.com/json)
 * A JavaScript engine:
   * [MsieJavaScriptEngine](https://github.com/Taritsyn/MsieJavaScriptEngine) -
     Windows only and requires IE9 or above to be installed on the server
   * [Jint](https://github.com/sebastienros/jint) - Slower but cross-platform

Installation
============
Via released [NuGet package](#)
----------------------------
To be written

Via latest development build
----------------------------
To be written

Manual Installation
-------------------
To be written

Usage
=====
To be written

Changelog
=========

Licence
=======
BSD License for React.NET

Copyright (c) 2014, Facebook, Inc. All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

 * Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.
 * Neither the name Facebook nor the names of its contributors may be used to
   endorse or promote products derived from this software without specific
   prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
