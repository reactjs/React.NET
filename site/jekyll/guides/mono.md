---
layout: docs
title: Linux (Mono)
---

While Mono is supported, we strongly recommend using .NET Core instead.

ReactJS.NET includes full support for Mono via Google's [V8 JavaScript engine](https://code.google.com/p/v8/), the same engine used by Google Chrome and Node.js. To use ReactJS.NET with Mono, follow the documentation on the [JavaScriptEngineSwitcher repo](https://github.com/Taritsyn/JavaScriptEngineSwitcher/wiki/JS-Engine-Switcher:-Vroom) to build Vroom, and then register the JS Engine as the default in `Startup.cs`. For example on how to do this, see the [ChakraCore guide](/guides/chakracore.html).

If VroomJs fails to load, you will see an exception when your application is started. If this happens, run Mono with the `MONO_LOG_LEVEL=debug` environment variable to get more useful debugging information. Often, this occurs when Mono is unable to locate V8 (ie. it's not in /usr/lib/ or /usr/local/lib/)
