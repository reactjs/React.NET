@echo off
"%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe" build.proj  /p:BuildType=Dev
pause