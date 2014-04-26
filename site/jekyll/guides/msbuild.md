---
layout: docs
title: MSBuild
---

**New in ReactJS.NET 0.2**

ReactJS.NET includes an MSBuild task for compiling JSX into JavaScript. This is
handy to improve the start time of your application, especially if you have a
large number of JSX files.

To use it, first reference the `TransformJsx` task, and then call it wherever
you like:

```xml
<UsingTask
	AssemblyFile="tools\React\React.MSBuild.dll"
	TaskName="TransformJsx"
/>
<Target Name="TransformJsx">
	<TransformJsx SourceDir="$(MSBuildProjectDirectory)" TargetDir="" />
</Target>
```

To get started easily, you can install the [React.MSBuild]
(https://www.nuget.org/packages/React.MSBuild/) NuGet package which will
automatically modify your web application's `.csproj` file to reference the task
and run it after every site compilation. To customise the process (for example,
to only compile the JSX files for release builds), modify the `TransformJsx`
build target that was added to the csproj file.

The NuGet package is good for getting started quickly, but it has some
limitations. The package needs to add a reference to `React.MSBuild.dll`, even
though this assembly is only used at build time and not actually used at
runtime. Instead of using the NuGet package, you can just manually copy all the
assembly files into a folder (such as `tools\React`) and just reference the task
manually.
