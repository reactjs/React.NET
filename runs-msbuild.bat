@echo off

set BUILDTYPE="%~1"
set ACTION="%~2"

for %%s in (
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
    "%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin\MSBuild.exe"
) do (
    if exist %%s (
        echo %%s build.proj %ACTION% %BUILDTYPE%
        %%s build.proj %ACTION% %BUILDTYPE%
        goto :done
    )
)

:notfound
echo Could not find MSBuild.exe. Make sure Visual Studio 2019 is installed and try again.

:done
pause
