@echo off
goto SKIPREM

::  ============================================================================
::
::  Name:           AnyCSV_Demo_Release.CMD
::
::  Synopsis:       Use CSV_Test_Strings.TXT to exercise the Release build of
::                  AnyCSVTestStand.exe.
::
::  Arguments:      None
::
::  Dependencies:   AnyCSVTestStand.exe  = Unit test and demonstration program
::                                         for WizardWrx.AnyCSV.dll.
::
::                  CSV_Test_Strings.TXT = Test cases for exercising the DLL.
::
::                  1) AnyCSVTestStand.exe must be installed in the Release
::                     build output directory, ..\AnyCSVTestStand\bin\Release
::                     relative to the directory into which this script is
::                     installed.
::
::                  2) CSV_Test_Strings.TXT must be installed in the directory
::                     into which this script is installed.
::
::  Remarks:        This script works without a terminal pause because the test
::                  program has one built into it.
::
::                  Without an installer that can fix it up, this script is more
::                  portable than a desktop shortcut.
::
::  ----------------------------------------------------------------------------
::  Revision History
::  ----------------------------------------------------------------------------
::
::  Date       By  Synopsis
::  ---------- --- -------------------------------------------------------------
::  2019/08/10 DAG Initial version.
::  ============================================================================

::  ----------------------------------------------------------------------------
::  Since I like to keep the headnote with the routine that it deocuments, I put
::  it below the label that denotes its start, and immediately jump over it, to
::  keep the comments, themselves, out of the execution path. This is the result
::  of hard won knowledtge about the strange things that can hapen when you have
::  inline comments, especially when a stray character finds its way into the
::  comment.
::  ----------------------------------------------------------------------------


::  +--------------------------------------------------------------------------+
::  |                   B e g i n   M a i n   R o u t i n e                    |
::  +--------------------------------------------------------------------------+


:SKIPREM

    goto SKIPRM1

::  ----------------------------------------------------------------------------
::  Display a startup message, then verify that command extensions are enabled.
::
::  NOTE:   On Wednesday, 05 February 2014, I discovered that the test below is
::          broken, but the outcome is more or less acceptable, because it kills
::          the script immediately.
::
::  Moreover, there are circumstances in which %~t0 is not interpolated, either,
::  but it is replaced by the modified date of the executing script under most
::  circumstances, even when the script is called from a secondary shell, even
::  when that shell happens to be COMMAND.COM.
::  ----------------------------------------------------------------------------

:SKIPRM1

    echo %0 version %~t0 Begin
    echo.

    if CMDEXTVERSION 1  goto ARBEITE

    echo.
    echo ----------------------------------------------------
    echo     This script requires command extensions to be enabled. Since they
    echo     are enabled, by default, they have been disabled, directly or by GPO.

    goto ERR_DONE

::  ----------------------------------------------------------------------------
::  Though it is technically legal to allow the command processor to parse these
::  comments, which it should skip, I have learned the hard way that doing so is
::  unwise, because it takes only one illegal character such as an angle bracket
::  in just the right spot to make a mess of your careful work, with output that
::  makes the cause hard to pin down.
::  ----------------------------------------------------------------------------

:ARBEITE

    setlocal
    goto ARBEITE1

:ARBEITE1

    echo Script Directory Name = %~dp0
    echo.
    %~dp0..\AnyCSVTestStand\bin\Release\AnyCSVTestStand.exe %~dp0CSV_Test_Strings.TXT

:REG_DONE

    echo.
    echo %~nx0 Done!
    echo.
    goto :EOF

:ERR_DONE

    echo.
    echo %~nx0 Done with ERROR because Command Extensions are off
    echo.