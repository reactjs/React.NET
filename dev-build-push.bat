@echo off
"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" build.proj /t:Package;Push /p:BuildType=Dev
pause
