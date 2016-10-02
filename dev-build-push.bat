@echo off
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" build.proj /t:Package;Push /p:BuildType=Dev
pause