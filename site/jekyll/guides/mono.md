---
layout: docs
title: Linux (Mono)
---

**New in ReactJS.NET 1.0**

ReactJS.NET 1.0 includes full support for Mono via Google's [V8 JavaScript engine](https://code.google.com/p/v8/), the same engine used by Google Chrome and Node.js. To use ReactJS.NET with Mono, you need to compile V8 and VroomJs (a .NET wrapper around V8). This can be accomplished by running the following shell commands on your Linux or Mac OS X machine:

```sh
# Get a supported version of V8
cd /usr/local/src/
git clone https://github.com/v8/v8.git v8-3.17
cd v8-3.17
git checkout 3.17

# Build V8
make dependencies
make native werror=no library=shared soname_version=3.17.16.2 -j4
cp out/native/lib.target/libv8.so.3.17.16.2 /usr/local/lib/

# Get ReactJS.NET's version of libvroomjs
cd /usr/local/src/
git clone https://github.com/reactjs/react.net.git
cd react.net
git submodule update --init
cd lib/VroomJs/libvroomjs/

# Build libvroomjs
g++ jscontext.cpp jsengine.cpp managedref.cpp bridge.cpp jsscript.cpp -o libVroomJsNative.so -shared -L /usr/local/src/v8-3.17/out/native/lib.target/ -I /usr/local/src/v8-3.17/include/ -fPIC -Wl,--no-as-needed -l:/usr/local/lib/libv8.so.3.17.16.2
cp libVroomJsNative.so /usr/local/lib/
ldconfig
```

Once this has been completed, install the **React.JavaScriptEngine.VroomJs** package to your website.

If VroomJs fails to load, you will see an exception when your application is started. If this happens, run Mono with the `MONO_LOG_LEVEL=debug` environment variable to get more useful debugging information. Often, this occurs when Mono is unable to locate V8 (ie. it's not in /usr/lib/ or /usr/local/lib/)
