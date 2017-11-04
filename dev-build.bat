@echo off

for %%s in (
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
) do (
    if exist %%s (
        echo %%s build.proj
        %%s build.proj
        goto :done
    )
)

:notfound
echo Could not find MSBuild.exe. Make sure Visual Studio 2017 is installed and try again.

:done
pause
