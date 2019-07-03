/*
    ============================================================================

    Module Name:        Program.cs

    Namespace Name:     ConsoleUtility1

    Class Name:         Program

    Synopsis:           This command line utility is the test program for my new
                        AnyCSV parsing engine.

    Remarks:            This class module implements the Program class, which is
                        composed exclusively of the static void Main method,
                        which is functionally equivalent to the main() routine
                        of a standard C program.

						This assembly leverages routines imported from several
                        of my core helper class libraries, for which I make no
                        apology. Since it is a unit test program, and should not
						go into a production environment, I am less concerned
                        about dependencies, and will happily trade them for a
                        robust program that goes together quickly.

                        WizardWrx.AnyCSV.dll, on the other hand, is completely
                        independent; its only reference is to system, and it can
                        run against any version of the Microsoft .NET Framework
                        above 1.1 (Does anybody still use that version?).

	License:            Copyright (C) 2014-2019, David A. Gray.
						All rights reserved.

                        Redistribution and use in source and binary forms, with
                        or without modification, are permitted provided that the
                        following conditions are met:

                        *   Redistributions of source code must retain the above
                            copyright notice, this list of conditions and the
                            following disclaimer.

                        *   Redistributions in binary form must reproduce the
                            above copyright notice, this list of conditions and
                            the following disclaimer in the documentation and/or
                            other materials provided with the distribution.

                        *   Neither the name of David A. Gray, nor the names of
                            his contributors may be used to endorse or promote
                            products derived from this software without specific
                            prior written permission.

                        THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
                        CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
                        WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
                        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
                        PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
                        David A. Gray BE LIABLE FOR ANY DIRECT, INDIRECT,
                        INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
                        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
                        SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
                        PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
                        ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
                        LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
                        ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
                        IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    Created:            Saturday, 05 July 2014 - Monday, 07 July 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Synopsis
    ---------- ------- ------ -------------------------------------------------
    2014/07/07 3.0     DAG    This is the first version.

	2016/06/10 3.1     DAG    Embed my three-clause BSD license, and sever all
                              ties with the deprecated class libraries and the
                              CodeProject library that AnyCSV superseded.

    2016/09/18 4.0     DAG    The original Parser class exposes many more
                              constructors and overloaded Parse methods than can
                              be exposed to COM, and this remains its unit test.

    2017/08/11 5.0     DAG    Adapt to use the new constellation of helper
                              libraries in place of WizardWrx.DllServices2.dll,
                              which was a monolith.

    2019/07/01 7.1     DAG    Using the enumerations, create an instance that is
                              functionally identical to the first static method
                              scenario, use it to parse all strings, and use its
                              ToString method to report its properties 
                              immediately post-construction and immediately
                              after first use.
    ============================================================================
*/


using System;
using System.Text;

using WizardWrx;
using WizardWrx.ConsoleAppAids3;
using WizardWrx.Core;
using WizardWrx.DLLConfigurationManager;


namespace AnyCSVTestStand
{
    class Program
    {
        enum OutputFormat
        {
            Verbose = 0 ,									// Tell all
            Terse = 1 ,										// Just the facts
            None = 2 ,										// Silence!
            Quiet = 2 ,										// Equivalent to None
            V = 0 ,											// Equivalent to Verbose
            T = 1 ,											// Equivalent to Terse
            N = 2 ,											// Equivalent to None
            Q = 2											// Equivalent to None
        };  // OutputFormat

        const int ERR_RUNTIME = 1;
        const int ERR_NEED_INPUT_FILENAME = 2;

        const bool IGNORE_BOM = false;

        static readonly string [ ] s_astrErrorMessages =
        {
            Properties.Resources.ERRMSG_SUCCESS ,           // ERROR_SUCCESS
            Properties.Resources.ERRMSG_RUNTIME,            // ERR_RUNTIME
            Properties.Resources.ERRMSG_NEED_INPUT_FILENAME	// ERR_NEED_INPUT_FILENAME
        };  // s_astrErrorMessages

        static ConsoleAppStateManager s_theApp;

        const char SW_OUTPUT = 'o';                         // Argument specifies one of three supported formats for the STDOUT display.

        static readonly char [ ] s_achrValidSwitches =
        {
            SW_OUTPUT ,
        };  // static char [ ] s_achrValidSwitches

        static void Main ( string [ ] args )
        {

            s_theApp = ConsoleAppStateManager.GetTheSingleInstance ( );

            CmdLneArgsBasic cmdArgs = new CmdLneArgsBasic (
                s_achrValidSwitches ,
                CmdLneArgsBasic.ArgMatching.CaseInsensitive )
            {
                AllowEmptyStringAsDefault = CmdLneArgsBasic.BLANK_AS_DEFAULT_ALLOWED
            };

            //  ----------------------------------------------------------------
            //  The default value of the AppSubsystem property is GUI, which
            //  disables output to the console. Since ReportException returns
            //  the message that would have been written, you still have the
            //  option of displaying or discarding it. If EventLoggingState is
            //  set to Enabled, the message is written into the Application
            //  Event log, where it is preserved until the event log record is
            //  purged by the aging rules or some other method.
            //  ----------------------------------------------------------------

            s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags =
                s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags
                | ExceptionLogger.OutputOptions.EventLog
                | ExceptionLogger.OutputOptions.Stack
                | ExceptionLogger.OutputOptions.StandardError;
			s_theApp.BaseStateManager.LoadErrorMessageTable ( s_astrErrorMessages );

            string strDeferredMessage = null;

            OutputFormat enmOutputFormat = SetOutputFormat (
                cmdArgs ,
                ref strDeferredMessage );

            if ( enmOutputFormat != OutputFormat.None )
            {   // Unless output is suppressed, display the standard BOJ message.
                s_theApp.DisplayBOJMessage ( );
            }   // if ( enmOutputFormat != OutputFormat.None )

            if ( !string.IsNullOrEmpty ( strDeferredMessage ) )
            {   // SetOutputFormat saves its error message, if any, in SetOutputFormat. 
                Console.WriteLine ( strDeferredMessage );
            }   // if ( !string.IsNullOrEmpty ( s_strDeferredMessage ) )

            if ( cmdArgs.PositionalArgsInCmdLine < CmdLneArgsBasic.FIRST_POSITIONAL_ARG )
            {   // Required input is missing. Report and bug out.
                s_theApp.ErrorExit ( ERR_NEED_INPUT_FILENAME );
            }   // if ( cmdArgs.PositionalArgsInCmdLine < CmdLneArgsBasic.FIRST_POSITIONAL_ARG )

            string strTestFileName = cmdArgs.GetArgByPosition ( CmdLneArgsBasic.FIRST_POSITIONAL_ARG );
            Console.WriteLine (
                Properties.Resources.MSG_INPUT_FILENAME ,
				s_theApp.BaseStateManager.InitialWorkingDirectoryName ,
                strTestFileName ,
                Environment.NewLine );

            try
            {
#if ANYCSV
                int intNCases = System.IO.File.ReadAllLines (
                    strTestFileName , 
                    Encoding.ASCII ).Length;
                int intCurrCase = MagicNumbers.ZERO;

                WizardWrx.AnyCSV.Parser ReusableParser = new WizardWrx.AnyCSV.Parser (
                    WizardWrx.AnyCSV.CSVParseEngine.DelimiterChar.Comma ,
                    WizardWrx.AnyCSV.CSVParseEngine.GuardChar.DoubleQuote ,
                    WizardWrx.AnyCSV.CSVParseEngine.GuardDisposition.Strip );
                Console.WriteLine (
                    Properties.Resources.MSG_TOSTRING_CONSTRUCTED_FROM_ENUMS ,          // Format control string
                    Properties.Resources.MSG_TOSTRING_JUST_CONSTRUCTED ,                // Format Item 0: {0}, the ToString method
                    nameof ( ReusableParser ) ,                                         // Format Item 1: the ToString method on {1} reports
                    ReusableParser.ToString ( ) );                                      // Format Item 2: reports as follows: {2}.

                using ( System.IO.StreamReader srTestData = new System.IO.StreamReader (
                        strTestFileName ,
                        Encoding.ASCII ,
                        IGNORE_BOM ,
                        MagicNumbers.CAPACITY_08KB ) )
                {
                    do
                    {
                        string strTestCase = srTestData.ReadLine ( );

                        string [ ] astrInstanceTestOutput = ReusableParser.Parse ( strTestCase );

                        if ( intCurrCase == MagicNumbers.ZERO )
                        {   // On the first iteration, intCurrCase is zero.
                            Console.WriteLine (
                                Properties.Resources.MSG_TOSTRING_CONSTRUCTED_FROM_ENUMS ,          // Format control string
                                Properties.Resources.MSG_TOSTRING_POST_FIRST_USE ,                  // Format Item 0: {0}, the ToString method
                                nameof ( ReusableParser ) ,                                         // Format Item 1: the ToString method on {1} reports
                                ReusableParser.ToString ( ) );                                      // Format Item 2: reports as follows: {2}.
                        }   // if ( intCurrCase == MagicNumbers.ZERO )

                        string [ ] astrStaticTestOutput1 = WizardWrx.AnyCSV.Parser.Parse (
                            strTestCase ,                                                           // Input string
                            WizardWrx.AnyCSV.Parser.COMMA ,                                         // Field delimiter character
                            WizardWrx.AnyCSV.Parser.DOUBLE_QUOTE );                                 // Delimiter guard character

                        //  ----------------------------------------------------
                        //  Identify the case number, and show the input string.
                        //  ----------------------------------------------------

                        intCurrCase++;
                        int intNFields = astrStaticTestOutput1.Length;
                        Console.WriteLine (
                            Properties.Resources.MSG_CASE_LABEL ,
                            new string [ ]
                            {
                                intCurrCase.ToString ( ) ,
                                intNCases.ToString ( ) ,
                                intNFields.ToString ( ) ,
                                strTestCase ,
                                Environment.NewLine } );

                        ReportScenarioOutcome (
                            Properties.Resources.INSTANCE_SCENARIO_1 ,
                            astrInstanceTestOutput ,
                            astrInstanceTestOutput.Length );
                        ReportScenarioOutcome (
                            Properties.Resources.STATIC_SCENARIO_1 ,
                            astrStaticTestOutput1 ,
                            intNFields );

                        string [ ] astrStaticTestOutput2 = WizardWrx.AnyCSV.Parser.Parse (
                            strTestCase ,                                                           // Input string
                            WizardWrx.AnyCSV.Parser.COMMA ,                                         // Field delimiter character
                            WizardWrx.AnyCSV.Parser.DOUBLE_QUOTE ,                                  // Delimiter guard character
                            WizardWrx.AnyCSV.Parser.GuardDisposition.Keep );                        // Override to keep.

                        intNFields = astrStaticTestOutput2.Length;
                        ReportScenarioOutcome (
                            Properties.Resources.STATIC_SCENARIO_2 ,
                            astrStaticTestOutput2 ,
                            intNFields );

                        string [ ] astrStaticTestOutput3 = WizardWrx.AnyCSV.Parser.Parse (
                            strTestCase ,                                                           // Input string
                            WizardWrx.AnyCSV.Parser.COMMA ,                                         // Field delimiter character
                            WizardWrx.AnyCSV.Parser.DOUBLE_QUOTE ,                                  // Delimiter guard character
                            WizardWrx.AnyCSV.Parser.GuardDisposition.Strip ,                        // Override to keep.
                            WizardWrx.AnyCSV.Parser.TrimWhiteSpace.TrimLeading );
                        intNFields = astrStaticTestOutput3.Length;
                        ReportScenarioOutcome (
                            Properties.Resources.STATIC_SCENARIO_3 ,
                            astrStaticTestOutput3 ,
                            intNFields );
                    } while ( !srTestData.EndOfStream );
                }   // using ( System.IO.StreamReader srTestData = new System.IO.StreamReader (
#else
                CsvParser engine = new CsvParser ( );
                string [ ] [ ] astrParsedStrings = engine.Parse ( 
                    new System.IO.StreamReader (
                        strTestFileName ,
                        Encoding.ASCII ,
                        IGNORE_BOM ,
                        StandardConstants.FILE_BUFSIZE_STANDARD_08KB ) );
                int intNCases = astrParsedStrings.Length;

                for ( int intCurrCase = StandardConstants.ARRAY_FIRST_ELEMENT ; intCurrCase < intNCases ; intCurrCase++ )
                {
                    int intNFields = astrParsedStrings [ intCurrCase ].Length;
                    Console.WriteLine (
                        Properties.Resources.MSG_CASE_LABEL ,
                        new string [ ]
                        {
                            OrdinalFromSubscript ( intCurrCase ).ToString(),
                            intNCases.ToString() ,
                            intNFields.ToString() ,
                            Environment.NewLine
                        } );
                    
                    for ( int intCurrField = StandardConstants.ARRAY_FIRST_ELEMENT ;
                              intCurrField < astrParsedStrings [ intCurrCase ].Length ;
                              intCurrField++ )
                    {
                        Console.WriteLine (
                            Properties.Resources.MSG_CASE_DETAIL ,
                            OrdinalFromSubscript ( intCurrField ) ,
                            intNFields ,
                            astrParsedStrings [ intCurrCase ] [ intCurrField ] );
                    }   // for ( int intCurrField = StandardConstants.ARRAY_FIRST_ELEMENT ; intCurrField < astrParsedStrings [ intCurrCase ].Length ; intCurrField++ )
                }   // for ( int intCurrCase = StandardConstants.ARRAY_FIRST_ELEMENT ; intCurrCase < intNCases ; intNCases++ )
#endif
            }
            catch ( Exception exAll )
            {   // The Message string is displayed, but the complete exception goes to the event log.
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exAll );
                Console.WriteLine ( exAll.Message );

                ExitWithError (
                    enmOutputFormat ,
                    ERR_RUNTIME );
            }   // Providing a catch block is enough to cause the program to fall through.

#if DEBUG
            if ( enmOutputFormat == OutputFormat.None )
            {   // Suppress all output.
                s_theApp.NormalExit (  ConsoleAppStateManager.NormalExitAction.Silent );
            }   // TRUE block, if ( enmOutputFormat == OutputFormat.None )
            else
            {   // Display the standard exit banner.
                s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.WaitForOperator );
            }   // FALSE block, if ( enmOutputFormat == OutputFormat.None )
#else
            if ( enmOutputFormat == OutputFormat.None )
            {   // Suppress all output.
                s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.Silent );
            }   // TRUE block, if ( enmOutputFormat == OutputFormat.None )
            else
            {   // Display the standard exit banner.
                s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.ExitImmediately );
            }   // FALSE block, if ( enmOutputFormat == OutputFormat.None )
#endif
        }   // static void Main


        private static void ReportScenarioOutcome (
            string pstrScenarioLabel ,
            string [ ] pastrTestOutput ,
            int pintNFields )
        {
            Console.WriteLine (
                pstrScenarioLabel ,
                Environment.NewLine );

            for ( int intCurrField = ArrayInfo.ARRAY_FIRST_ELEMENT ;
                      intCurrField < pintNFields ;
                      intCurrField++ )
            {
				Console.WriteLine (
					Properties.Resources.MSG_CASE_DETAIL ,
					ArrayInfo.OrdinalFromIndex ( intCurrField ) ,
					pintNFields ,
					pastrTestOutput [ intCurrField ] );
            }   // for ( int intCurrField = StandardConstants.ARRAY_FIRST_ELEMENT ; intCurrField < intNFields ; intCurrField++ )
        }   //private static void ReportScenarioOutcome


        private static void ExitWithError (
            OutputFormat penmOutputFormat ,
            uint puintStatusCode )
        {
            if ( penmOutputFormat == OutputFormat.Quiet )
                s_theApp.NormalExit (
                    puintStatusCode ,
                    ConsoleAppStateManager.NormalExitAction.Silent );
            else
                s_theApp.NormalExit (
                    puintStatusCode ,
                    ConsoleAppStateManager.NormalExitAction.ExitImmediately );
        }   // private static void ExitWithError


        private static OutputFormat SetOutputFormat (
            CmdLneArgsBasic pcmdArgs ,
            ref string pstrDeferredMessage )
        {
            //  ----------------------------------------------------------------
            //  An invalid input value elicits a message similar to the following.
            //
            //      Requested value 'Foolish' was not found.
            //
            //  The simplest way to report an invalid value is by extracting it
            //  from the Message property of the ArgumentException thrown by the
            //  Enum.Parse method.
            //
            //  I happen to have a library routine, ExtractBoundedSubstrings,
            //  which became part of a sealed class, WizardWrx.StringTricks,
            //  exported by class library WizardWrx.SharedUtl2.dll version 2.62,
            //  which came into being exactly two years ago, 2011/11/23.
			//
			//	2016/06/10 - DAG - Though I have retired WizardWrx.SharedUtl2,
			//	                   StringTricks went into WizardWrx.DLLServices,
			//                     as did everything that was worth saving.
            //  ----------------------------------------------------------------

            const bool IGNORE_CASE = true;
            const int NONE = 0;

            OutputFormat renmOutputFormat = OutputFormat.Verbose;

            //  ----------------------------------------------------------------
            //  Enum.Parse needs a try/catch block, because an invalid SW_OUTPUT
            //  value raises an exception that can be gracefully handled without
            //  killing the program.
            //  ----------------------------------------------------------------

            try
            {
                if ( pcmdArgs.ValidSwitchesInCmdLine > NONE )
                {
                    renmOutputFormat = ( OutputFormat ) Enum.Parse (
                        typeof ( OutputFormat ) ,
                        pcmdArgs.GetSwitchByName (
                            SW_OUTPUT ,
                            OutputFormat.Verbose.ToString ( ) ) ,
                        IGNORE_CASE );
                }   // if ( pcmdArgs.ValidSwitchesInCmdLine > NONE )
            }
            catch ( ArgumentException exArg )
            {   // Display of the message is deferred until the BOJ message is printed.
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exArg );
                pstrDeferredMessage = string.Format (
                    Properties.Resources.ERRMSG_INVALID_OUTPUT_FORMAT ,
                    exArg.Message.ExtractBoundedSubstrings ( WizardWrx.SpecialCharacters.SINGLE_QUOTE ) ,
                    renmOutputFormat ,
                    Environment.NewLine );
            }

            return renmOutputFormat;
        }   // private static OutputFormat SetOutputFormat
    }   // class Program
}   // partial namespace AnyCSVTestStand