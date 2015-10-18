---
layout: docs
title: Code Structure
---

This document outlines the structure of ReactJS.NET's code internally. If you
are just a regular user of ReactJS.NET, you don't have to read it.

Assemblies
----------
ReactJS.NET is split into several different dependencies (React.Core, React.Web,
etc.). Ideally, the main "React.Core" assembly should have as few dependencies as
possible. Dependencies with third-party libraries should be kept in separate
assemblies unless they're required by core ReactJS.NET functionality (such as
JavaScript engines)

Interfaces and Dependency Injection
-----------------------------------
ReactJS.NET uses TinyIoC for dependency injection. You shouldn't directly
reference the concrete implementation of any class unless explicitly extending
it. The core ReactJS.NET consists of the following interfaces:

 * **IReactEnvironment** - The core React environment. Handles loading of all
   the JavaScript files, converting JSX to JavaScript, and actually rendering
   components
 * **IJavaScriptEngineFactory** - Handles creation of JavaScript engines. Since
   the Internet Explorer engine is not thread-safe, a new engine will be created
   per-thread

To obtain an implementation of any of these interfaces in your own code, use the
dependency resolver:

```csharp
var environment = React.AssemblyRegistration.Container.Resolve<IReactEnvironment>();
```

Ideally this should be done as little as possible. Internally, ReactJS.NET code
should use constructor injection except at the very root entry points.
