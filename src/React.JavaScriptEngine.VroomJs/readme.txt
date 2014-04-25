To use ReactJS.NET on Linux or Mac OS X, you need to compile VroomJs and V8. This can be
accomplished using the following shell commands:

# Get a supported version of V8
cd /usr/local/src/
git clone https://github.com/v8/v8.git
cd v8
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
g++ jscontext.cpp jsengine.cpp managedref.cpp bridge.cpp jsscript.cpp -o libVroomJsNative.so -shared -L /usr/local/src/v8/out/x64.release/lib.target/ -I /usr/local/src/v8/include/ -fPIC -Wl,--no-as-needed -l:/usr/local/lib/libv8.so.3.17.16.2
cp libVroomJsNative.so /usr/local/lib/
ldconfig

If VroomJs fails to load, run Mono with the `MONO_LOG_LEVEL=debug` environment variable to get 
more useful debugging information. Often, this occurs when Mono is unable to locate V8 (ie. it's 
not in /usr/lib/ or /usr/local/lib/)

For more information, please see the ReactJS.NET website at http://reactjs.net/
