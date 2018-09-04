/*
    ============================================================================

    Namespace:          WizardWrx.AnyCSV

    Class Name:         Parser

	Base Class Name:	CSVParseEngine

    File Name:          Parser.cs

    Synopsis:           This class defines the most robust CSV string parser
                        that I can conceive, based on 17 years of experience
                        writing CSV parsers, leading to the discovery of a use
                        case that breaks parsers that are considered unbreakable
                        by most, and are, with respect to fairly well formed CSV
                        text.

    Remarks:            This is the case that broke them all.

                            CN=RapidSSL CA, O="GeoTrust, Inc.", C=US

                        I didn't make this up; I discovered it "in the wild," in
                        the collection of digital certificates installed on my
                        development machine.

                        For what it's worth, this one-class library uses only
                        core Base Class Library features, and needs only one
                        reference, to System. Although it defines and uses a few
                        constants that duplicate constants defined in my core
                        class library, WizardWrx.DLLServices.dll, I avoided any
                        reference to them, so that this library is completely
                        freestanding.

	Reference:			"Issues with building a project with "Register for COM interop" for a 64-bit assembly"
						https://support.microsoft.com/en-us/help/956933/issues-with-building-a-project-with-register-for-com-interop-for-a-64
						Retrieved 2017/08/05 07:28:24

	License:            Copyright (C) 2014-2018, David A. Gray.
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

    Created:            Saturday, 05 July 2014 through Monday, 07 July 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Description
    ---------- ------- ------ --------------------------------------------------
    2014/07/07 3.0     DAG    Initial implementation.

    2016/06/10 3.1     DAG    Embed my three-clause BSD license, and improve the
                              internal documentation.

    2016/10/01 4.0     DAG    Expose this class to COM, correct typographical
                              errors in the documentation and comments, and
                              clarify a handful of technical points in the help.

    2017/08/05 5.0     DAG    Change CPU architecture from x86 to MSIL, and
                              record the COM registration in the 64 bit Registry
                              as described in the reference cited above.

    2018/01/03 7.0     DAG    Sign the assembly with a strong name, so that it
                              can go into a Global Assembly Cache. Since there
                              are no other changes, there is no debug build.

    2018/09/03 7.0     DAG    Eliminate unused using directives.
    ============================================================================
*/


using System.Runtime.InteropServices;


namespace WizardWrx.AnyCSV
{
    /// <summary>
    /// This class defines the most robust CSV string parser that I can
    /// conceive, based on 17 years of experience writing CSV parsers, leading
    /// to the discovery today of a use case that breaks parsers that were
    /// thought to be bulletproof.
	/// 
	/// Like its parent, RobustDelimitedStringParser, this class relies upon a
	/// static method on its base class, CSVParseEngine, to do the work.
    /// </summary>
	[ComVisible ( false ) ]
	public class Parser : CSVParseEngine
    {
        #region Constructors
        /// <summary>
        /// The default constructor creates a Parser that uses its namesake, the
        /// comma, as its delimiter, protects commas that occur within double
        /// quotation marks, discards double quotation marks that surround whole
        /// fields, and preserves leading and trailing white space.
        /// </summary>
		public Parser ( ) { }   // The default constructor is 1 of 9.


        /// <summary>
        /// Use this constructor to override the default delimiter (comma) by
        /// passing your own character.
        /// </summary>
        /// <param name="pchrDelimiter">
        /// Specify the character to treat as the delimiter. Any character that
        /// can occur in a text file is valid.
        /// </param>
		public Parser ( char pchrDelimiter )
        {
            _chrDelimiter = pchrDelimiter;
            _enmDelimiter = DelimiterEnumFromChar ( pchrDelimiter );
        }   // Constructor 2 of 9


        /// <summary>
        /// Use this constructor to override the default delimiter (comma) by
        /// selecting a character from an enumeration.
        /// </summary>
        /// <param name="penmDelimiter">
        /// Use any member of the DelimiterChar enumeration except DelimiterChar.Other
        /// to specify your chosen delimiter.
        /// </param>
		public Parser ( DelimiterChar penmDelimiter )
        {
            _enmDelimiter = penmDelimiter;
            _chrDelimiter = DelimiterCharFromEnum ( penmDelimiter );
        }   // Constructor 3 of 9


        /// <summary>
        /// Use this constructor to override the default protector of delimiters
        /// (double quote) by selecting a character from an enumeration.
        /// </summary>
        /// <param name="penmProtector">
        /// Use any member of the GuardChar enumeration except GuardChar.Other
        /// to specify your chosen character to protect delimiters.
        /// </param>
		public Parser ( GuardChar penmProtector )
        {
            _enmGuard = penmProtector;
            _chrGuard = GuardCharFromEnum ( penmProtector );
        }   // Constructor 4 of 9


        /// <summary>
        /// Use this constructor to override the default delimiter (comma) and
        /// the protector of delimiters by passing your own character for each.
        /// See Remarks.
        /// </summary>
        /// <param name="pchrDelimiter">
        /// Specify the character to treat as the delimiter. Any character that
        /// can occur in a text file is valid.
        /// </param>
        /// <param name="pchrProtector">
        /// Specify the character to treat as the protector of delimiters. Any
        /// character that can occur in a text file is valid.
        /// </param>
        /// <remarks>
        /// Since both delimiters are of the same type, this constructor is the
        /// only avenue to specify a nonstandard delimiter AND a nonstandard
        /// protector. (It could be done, but not without fabricated types.) For
        /// most applications, the enumerated types are adequate, and safer.
        /// </remarks>
		public Parser (
            char pchrDelimiter ,
            char pchrProtector )
        {
            _chrDelimiter = pchrDelimiter;
            _chrGuard = pchrProtector;

            _enmDelimiter = DelimiterEnumFromChar ( pchrDelimiter );
            _enmGuard = GuardEnumFromChar ( pchrProtector );
        }   // Constructor 5 of 9


        /// <summary>
        /// Use this constructor to override the default delimiter (comma) and
        /// the protector of delimiters (double quotation marks) by selecting
        /// characters from a pair of enumerations that offer two mutually
        /// exclusive lists of characters.
        /// </summary>
        /// <param name="penmDelimiter">
        /// Use any member of the DelimiterChar enumeration except DelimiterChar.Other
        /// to specify your chosen delimiter.
        /// </param>
        /// <param name="penmProtector">
        /// Use any member of the GuardChar enumeration except GuardChar.Other
        /// to specify your chosen character to protect delimiters.
        /// </param>
		public Parser (
            DelimiterChar penmDelimiter ,
            GuardChar penmProtector )
        {
            _enmDelimiter = penmDelimiter;
            _enmGuard = penmProtector;

            _chrDelimiter = DelimiterCharFromEnum ( penmDelimiter );
            _chrGuard = GuardCharFromEnum ( penmProtector );
        }   // Constructor 6 of 9


        /// <summary>
        /// Use this constructor to override the default delimiter (comma) and
        /// the protector of delimiters (double quotation marks) by selecting
        /// characters from a pair of enumerations that offer two mutually
        /// exclusive lists of characters.
        ///
        /// In addition, this constructor can override the default disposition
        /// of guard characters that surround a whole field. By default, these
        /// guard characters are stripped; this constructor allows you to keep
        /// them.
        /// </summary>
        /// <param name="penmDelimiter">
        /// Use any member of the DelimiterChar enumeration except
        /// DelimiterChar.Other to specify your chosen delimiter.
        /// </param>
        /// <param name="penmProtector">
        /// Use any member of the GuardChar enumeration except GuardChar.Other
        /// to specify your chosen character to protect delimiters.
        /// </param>
        /// <param name="penmGuardDisposition">
        /// Specify whether guard characters that surround a whole field should
        /// be stripped (default) or kept.
        /// </param>
		public Parser (
            DelimiterChar penmDelimiter ,
            GuardChar penmProtector ,
            GuardDisposition penmGuardDisposition )
        {
            _enmDelimiter = penmDelimiter;
            _enmGuard = penmProtector;
            _enmGuardDisposition = penmGuardDisposition;

            _chrDelimiter = DelimiterCharFromEnum ( penmDelimiter );
            _chrGuard = GuardCharFromEnum ( penmProtector );
        }   // Constructor 7 of 9


        /// <summary>
        /// Use this constructor to override the default delimiter (comma) and
        /// the protector of delimiters (double quotation marks) by selecting
        /// characters from a pair of enumerations that offer two mutually
        /// exclusive lists of characters.
        ///
        /// In addition, this constructor can override the default disposition
        /// of guard characters that surround a whole field. By default, these
        /// guard characters are stripped; this constructor allows you to keep
        /// them.
        /// </summary>
        /// <param name="penmDelimiter">
        /// Use any member of the DelimiterChar enumeration except
        /// DelimiterChar.Other to specify your chosen delimiter.
        /// </param>
        /// <param name="penmProtector">
        /// Use any member of the GuardChar enumeration except GuardChar.Other
        /// to specify your chosen character to protect delimiters.
        /// </param>
        /// <param name="penmTrimWhiteSpace">
        /// Specify whether leading or trailing white space should be trimmed
        /// from a field.
        /// </param>
		public Parser (
            DelimiterChar penmDelimiter ,
            GuardChar penmProtector ,
            TrimWhiteSpace penmTrimWhiteSpace )
        {
            _enmDelimiter = penmDelimiter;
            _enmGuard = penmProtector;
            _enmTrimWhiteSpace = penmTrimWhiteSpace;

            _chrDelimiter = DelimiterCharFromEnum ( penmDelimiter );
            _chrGuard = GuardCharFromEnum ( penmProtector );
        }   // Constructor 8 of 9


        /// <summary>
        /// Use this constructor to override the default delimiter (comma) and
        /// the protector of delimiters (double quotation marks) by selecting
        /// characters from a pair of enumerations that offer two mutually
        /// exclusive lists of characters.
        ///
        /// In addition, this constructor can override the default disposition
        /// of guard characters that surround a whole field. By default, these
        /// guard characters are stripped; this constructor allows you to keep
        /// them.
        /// </summary>
        /// <param name="penmDelimiter">
        /// Use any member of the DelimiterChar enumeration except
        /// DelimiterChar.Other to specify your chosen delimiter.
        /// </param>
        /// <param name="penmProtector">
        /// Use any member of the GuardChar enumeration except GuardChar.Other
        /// to specify your chosen character to protect delimiters.
        /// </param>
        /// <param name="penmGuardDisposition">
        /// Specify whether guard characters that surround a whole field should
        /// be stripped (default) or kept.
        /// </param>
        /// <param name="penmTrimWhiteSpace">
        /// Specify whether leading or trailing white space should be trimmed
        /// from a field.
        /// </param>
		public Parser (
            DelimiterChar penmDelimiter ,
            GuardChar penmProtector ,
            GuardDisposition penmGuardDisposition ,
            TrimWhiteSpace penmTrimWhiteSpace )
        {
            _enmDelimiter = penmDelimiter;
            _enmGuard = penmProtector;
            _enmGuardDisposition = penmGuardDisposition;

            _chrDelimiter = DelimiterCharFromEnum ( penmDelimiter );
            _chrGuard = GuardCharFromEnum ( penmProtector );
        }   // Constructor 9 of 9
        #endregion  // Constructors TrimWhiteSpace
    }   // public class Parser
}   // namespace WizardWrx.AnyCSV