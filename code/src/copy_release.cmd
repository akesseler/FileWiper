@echo off

setlocal

set INPUTDIR1=.\FileWiper\bin\embattled\release\
set INPUTDIR2=.\FileWiper\bin\simulation\release\
set OUTPUTDIR1=..\Release\Embattled\
set OUTPUTDIR2=..\Release\Simulation\

REM Create output directory (1).

if not exist "%OUTPUTDIR1%" mkdir "%OUTPUTDIR1%"

REM Check and copy files (1).

if not exist "%INPUTDIR1%" (
	echo.
	echo.
	echo ERROR: Input directory ^"%INPUTDIR1%^" does not exist! Rebuild the solution beforehand.
	goto ERROR_INPUT
)

echo.
echo Copy EMBATTLED files...

for %%I in ("%INPUTDIR1%*.*") do (
	echo Copy file ^"%%I^" into ^"%OUTPUTDIR1%^" directory.
	copy "%%I" "%OUTPUTDIR1%" /y > nul
)

REM Create output directory (2).

if not exist "%OUTPUTDIR2%" mkdir "%OUTPUTDIR2%"

REM Check and copy files (2).

if not exist "%INPUTDIR2%" (
	echo.
	echo.
	echo ERROR: Input directory ^"%INPUTDIR2%^" does not exist! Rebuild the solution beforehand.
	goto ERROR_INPUT
)

echo.
echo Copy SIMULATION files...

for %%I in ("%INPUTDIR2%*.*") do (
	echo Copy file ^"%%I^" into ^"%OUTPUTDIR2%^" directory.
	copy "%%I" "%OUTPUTDIR2%" /y > nul
)

REM Cleanup...

endlocal
set ERRORLEVEL=0
goto END_BATCH

:ERROR_INPUT
echo.
endlocal
set ERRORLEVEL=1
goto END_BATCH

:END_BATCH

pause