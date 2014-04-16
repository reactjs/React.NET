---
layout: docs
title: Linux (Mono)
---

**New in ReactJS.NET 0.2.0**

ReactJS.NET 0.2.0 includes partial Mono support. Server-side component rendering
is supported, but JSX compilation is not yet supported. In order to use JSX
on Linux, you need to precompile all your JSX files on Windows before
deployment. This can be done via the [MSBuild task](/guides/msbuild.html) or via
[Cassette](/guides/cassette.html). Precompilation via the MSBuild task will
create `.generated.js` files which need to be deployed alongside the original
`.jsx` files.
