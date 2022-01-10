@echo off

setlocal

set BINARYDIR=.\FileWiper\bin\
set OBJECTDIR=.\FileWiper\obj\

if exist "%BINARYDIR%" (
	echo Remove directory "%BINARYDIR%".
	rmdir "%BINARYDIR%" /s /q
)

if exist "%OBJECTDIR%" (
	echo Remove directory "%OBJECTDIR%".
	rmdir "%OBJECTDIR%" /s /q
)

rem Remove solution's settings file.
rem echo Remove all SUO files.
rem del *.suo /q /a:h 2>&1> nul

endlocal
if "%1" == "nobreak" goto END_BATCH
pause
:END_BATCH
rem EOF