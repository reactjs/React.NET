To use ReactJS.NET on Linux or Mac OS X, you need to compile VroomJs and V8. This can be
accomplished using the following shell commands:

# Get a supported version of V8
cd /usr/local/src/
git clone https://github.com/v8/v8.git
cd v8
git checkout 3.15

# Build V8
make werror=no library=shared x64.release

# Get ReactJS.NET's version of libvroomjs
cd /usr/local/src/
git clone https://github.com/reactjs/react.net.git
cd react.net
git submodule update --init
cd lib/VroomJs/libvroomjs/

# Build libvroomjs
g++ jsengine.cpp managedref.cpp bridge.cpp -o libvroomjs.so -shared -L /usr/local/src/v8/out/x64.release/lib.target/ -I /usr/local/src/v8/include/ -fPIC -Wl,--no-as-needed -lv8 -g
cp libvroomjs.so /usr/local/lib/
ldconfig

If VroomJs fails to load, run Mono with the `MONO_LOG_LEVEL=debug` environment variable to get 
more useful debugging information. Often, this occurs when Mono is unable to locate V8 (ie. it's 
not in /usr/lib/ or /usr/local/lib/)

For more information, please see the ReactJS.NET website at http://reactjs.net/