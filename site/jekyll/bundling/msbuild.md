---
layout: docs
title: MSBuild (ASP.NET 4.x)
---

> **Note:**
>
> This guide applies only to ASP.NET 4.x. Please consider using [webpack](/bundling/webpack.html) if possible.

Just want to see the code? Check out the [sample project](https://github.com/reactjs/React.NET/tree/main/src/React.Sample.Mvc4).

ReactJS.NET includes an MSBuild task for compiling JSX into JavaScript. This is
handy to improve the start time of your application, especially if you have a
large number of JSX files.

To use it, first reference the `TransformBabel` task, and then call it wherever
you like:

```xml
<UsingTask
	AssemblyFile="tools\React\React.MSBuild.dll"
	TaskName="TransformBabel"
/>
<Target Name="TransformBabel">
	<TransformBabel SourceDir="$(MSBuildProjectDirectory)" TargetDir="" />
</Target>
```

To get started easily, you can install the [React.MSBuild](https://www.nuget.org/packages/React.MSBuild/) NuGet package which will
automatically modify your web application's `.csproj` file to reference the task
and run it after every site compilation. To customise the process (for example,
to only compile the JSX files for release builds), modify the `TransformBabel`
build target that was added to the csproj file.

The NuGet package is good for getting started quickly, but it has some
limitations. The package needs to add a reference to `React.MSBuild.dll`, even
though this assembly is only used at build time and not actually used at
runtime. Instead of using the NuGet package, you can just manually copy all the
assembly files into a folder (such as `tools\React`) and just reference the task
manually.
