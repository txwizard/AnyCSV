@echo off
echo %0 Begin

:: 2012/08/25 DAG Replace hard coded paths with environment variables.
:: 2012/08/25 DAG Derive this script from Archive_SharedUtl2.CMD.
:: 2014/07/07 DAG Derive this script from Archive_ConsoleAppAids.CMD

echo.
echo ------------------------------------------------------------------------
echo Project Assembly = %USERPROFILE%\My Documents\Visual Studio 2010\Projects\WizardWrx_Libs\AnyCSV\AnyCSV\bin\Release\WizardWrx.AnyCSV.dll
echo Backup Directory = C:\COMMON_DATA\Assorted_Zips\program_source\Visual Studio 2010
echo Backup Password  = Default
echo ------------------------------------------------------------------------
echo.

"%USERPROFILE%\My Documents\Visual Studio 2010\Projects\Tools_for_Programmers\WWSourceCodeArchiver\WWSourceCodeArchiver\bin\Release\WWSourceCodeArchiver.exe" "-p%USERPROFILE%\My Documents\Visual Studio 2010\Projects\WizardWrx_Libs\AnyCSV\AnyCSV\bin\Release\WizardWrx.AnyCSV.dll" "-tC:\COMMON_DATA\Assorted_Zips\program_source\Visual Studio 2010"

wwpause.exe
echo %0 Done!