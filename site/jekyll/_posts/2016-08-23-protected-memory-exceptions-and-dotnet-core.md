---
title: "'Attempted to read or write protected memory' exceptions, and an update on .NET Core support"
layout: post
author: Daniel Lo Nigro
---

Several users have reported received exceptions similar to the following:

```
An unhandled exception of type 'System.AccessViolationException' occurred in MsieJavaScriptEngine.dll
Additional information: Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
```

These errors [appear to be coming from the Internet Explorer JS engine](https://github.com/Taritsyn/MsieJavaScriptEngine/issues/7), which is used as a fallback in case V8 fails to initialise on app startup. If you are seeing this error, the best thing to try is to disable the MSIE engine and determine why V8 is failing to load. This can be done by calling `.SetAllowMsieEngine(false)` in your ReactJS.NET configuration (`ReactConfig.cs` for ASP.NET 4 projects, or in your `Startup.cs` file for ASP.NET Core projects).

For ASP.NET Core projects in particular, JavaScriptEngineSwitcher does not automatically copy the ClearScript DLL files to the output directory on build, which means it's unable to load them at runtime. This can be resolved one of two ways:

If you don't mind some extra DLL files living in your project's root directory, you can copy over the `ClearScript.V8` directory from the NuGet package (at `%UserProfile%\.nuget\packages\JavaScriptEngineSwitcher.V8\1.5.2\content\ClearScript.V8`) to your site's root directory (same directory as its `package.json` and `Startup.cs` files) and then add it to the `buildOptions`/`copyToOutput` section of your `project.json`:

```json{4-7}
"buildOptions": {
  "emitEntryPoint": true,
  "preserveCompilationContext": true,
  "copyToOutput": {
    "include": [
      "ClearScript.V8"
    ]
  }
},
```

On the other hand, if you'd rather not copy DLL files to your site (for example, you don't want to check them into source control), you can add a post-compile step to your `project.json` to copy the files over:

```
"scripts": {
  "postcompile": [
    "xcopy /Y C:\\Users\\Daniel\\.nuget\\packages\\JavaScriptEngineSwitcher.V8\\1.5.2\\content\\ClearScript.V8 %compile:RuntimeOutputDir%\\ClearScript.V8\\*"
  ]
}
```

This is pretty ugly due to the hard-coded path, but at least it works.

Once you've done either of these, build your site and then check its output directory (eg. `bin\Debug\net452\win7-x64`) to ensure the `ClearScript.V8` directory is there. If so, run your site, and you should no longer encounter the "Attempted to read or write protected memory" error.

The good news is that this issue of the DLL files not being automatically copied across [will be resolved](https://github.com/Taritsyn/JavaScriptEngineSwitcher/issues/18) with the upcoming [2.0 release of JavaScriptEngineSwitcher](https://github.com/Taritsyn/JavaScriptEngineSwitcher/releases/tag/v2.0.0-alpha.1), which this project will be switching over to in the near future.

One benefit of JavaScriptEngineSwitcher 2.0 is that it also adds support for .NET Core! Currently, ReactJS.NET only supports ASP.NET Core on the full .NET Framework. If you want to try out .NET Core support today, [Richard Dyer](https://github.com/RichardD012) has an [unofficial fork](https://github.com/reactjs/React.NET/issues/294) with preliminary support for .NET Core.

Please let me know if you still encounter the "protected memory" error even after switching to V8!

— Daniel
