[ReactJS.NET](http://reactjs.net/)
===========
ReactJS.NET is a library that makes it easier to use Facebook's
[React](http://facebook.github.io/react/) and
[JSX](http://facebook.github.io/react/docs/jsx-in-depth.html) from C#.

Features
========
 * On-the-fly [JSX to JavaScript compilation](http://reactjs.net/getting-started/download.html)
 * JSX to JavaScript compilation via popular minification/combination
   libraries:
   * [ASP.NET Bundling and Minification](http://reactjs.net/guides/weboptimizer.html)
   * Cassette
 * [Server-side component rendering](http://reactjs.net/guides/server-side-rendering.html)
   to make your initial render super-fast (experimental!)

Quick Start
===========
Install the package
```
Install-Package React.Mvc4
```

Create JSX files
```javascript
// /Scripts/HelloWorld.jsx
/** @jsx React.DOM */
var HelloWorld = React.createClass({
    render: function () {
        return (
            <div>Hello {this.props.name}</div>
        );
    }
});
```

Reference the JSX files from your HTML
```html
<script src="@Url.Content("~/Scripts/HelloWorld.jsx")"></script>
```

Now you can use the `HelloWorld` component.

For information on more advanced topics (including precompilation and
server-side rendering), check out [the documentation](http://reactjs.net/docs)

Licence
=======
BSD License for ReactJS.NET

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
