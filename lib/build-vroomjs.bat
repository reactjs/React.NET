@echo off
"%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe" VroomJs\VroomJs\VroomJs.csproj /p:BuildProjectReferences=false /p:Configuration=Release /p:OutputPath=..\..\ /p:SignAssembly=true /p:AssemblyOriginatorKeyFile=..\..\..\src\Key.snk
pause