/*
    ============================================================================

    Namespace:          WizardWrx.AnyCSV

    Class Name:         Parser

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

	License:            Copyright (C) 2014-2016, David A. Gray.
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
    ============================================================================
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WizardWrx.AnyCSV
{
    /// <summary>
    /// This class defines the most robust CSV string parser that I can
    /// conceive, based on 17 years of experience writing CSV parsers, leading
    /// to the discovery today of a use case that breaks parsers that were
    /// thought to be bulletproof.
    /// </summary>
    public class Parser
    {
        #region Public Constants and Enumerations
        /// <summary>
        /// Use this symbolic constant to construct a Parser instance that uses
        /// a comma (',') as its delimiter, or to specify one to the static
        /// Parse method.
        ///
		/// The equivalent DelimiterChar member is Comma, and its integral value
		/// is 0x2c (44 decimal).
        /// </summary>
        public const char COMMA = ',';


        /// <summary>
        /// Use this symbolic constant to construct a Parser instance that uses
        /// a tab ('\t") as its delimiter, or to specify one to the static Parse
        /// method.
        ///
        /// The equivalent DelimiterChar member is Tab, and its integral value
		/// is 9.
        /// </summary>
        public const char TAB = '\t';


        /// <summary>
        /// Use this symbolic constant to construct a Parser instance that uses
        /// a vertical bar ('|'), also known as the Pipe character, vertical 
		/// slash, bar, obelisk, and various other names, as its delimiter, or
		/// to specify one to the static Parse method.
        ///
        /// The equivalent DelimiterChar member is VerticalBar, and its integral
		/// value is 0x7C (124 decimal).
        /// </summary>
        public const char VERTICAL_BAR = '|';


        /// <summary>
        /// Use this symbolic constant to construct a Parser instance that uses
        /// a carat ('^') as its delimiter, or to specify one to the static
        /// Parse method.
        ///
		/// The equivalent DelimiterChar member is Carat, and its integral
		/// value is 0x5e (94 decimal).
        /// </summary>
        public const char CARAT = '^';


        /// <summary>
        /// Use this symbolic constant to construct a Parser instance that uses
        /// a space (' ') as its delimiter, or to specify one to the static
        /// Parse method.
        ///
		/// The equivalent DelimiterChar member is Space, and its integral
		/// value is 0x20 (32 decimal).
        /// </summary>
        public const char SPACE = ' ';


        /// <summary>
        /// Protect delimiters enclosed in double quotation marks.
        ///
		/// The equivalent GuardChar member is DoubleQuote, and its integral
		/// value is 0x22 (34 decimal).
        /// </summary>
        public const char DOUBLE_QUOTE = '\x0022';			// Protect enclosed delimiter characters


        /// <summary>
        /// Protect delimiters enclosed in single quotation marks.
        ///
		/// The equivalent GuardChar member is SingleQuote, and its integral
		/// value is 0x27 (39 decimal).
        /// </summary>
        public const char SINGLE_QUOTE = '\x0027';			// Protect enclosed delimiter characters



        /// Protect delimiters enclosed in backwards quotation marks, commonly
        /// called back-ticks.
        ///
		/// The equivalent GuardChar member is BackQuote, and its integral
		/// value is 0x60 (96 decimal).
        public const char BACK_QUOTE = '\x0060';			// Protect enclosed delimiter characters


        /// <summary>
        /// The DelimiterChar enumeration simplifies specifying any of the commonly
        /// used field delimiter characters.
        /// </summary>
        public enum DelimiterChar
        {
            /// <summary>
            /// Specify a comma as the delimiter.
            ///
            /// The equivalent character constant is COMMA.
            /// </summary>
            Comma,

            /// <summary>
            /// Specify a tab as the delimiter.
            ///
            /// The equivalent character constant is TAB.
            /// </summary>
            Tab,

            /// <summary>
            /// Specify a vertical bar ('|'), commonly called the pipe symbol,
            /// as the delimiter.
            ///
            /// The equivalent character constant is VERTICAL_BAR.
            /// </summary>
            VerticalBar,

            /// <summary>
            /// Specify a CARAT (^) as the delimiter.
            ///
            /// The equivalent character constant is CARAT.
            /// </summary>
            Carat,

            /// <summary>
            /// Specify a SPACE (' ') as the delimiter.
            ///
            /// The equivalent character constant is SPACE.
            /// </summary>
            Space,

            /// <summary>
            /// Infrastructure: The delimiter is something besides the
            /// enumerated choices.
            ///
            /// You cannot specify this type as input to the constructor. Use
            /// the overload that takes a character.
            /// </summary>
            Other,
        }   // public enum DelimiterChar


        /// <summary>
        /// The GuardChar enumeration simplifies specifying any of the
        /// commonly used field delimiter protector characters.
        /// </summary>
        public enum GuardChar
        {
            /// <summary>
            /// Specify a double quotation as the protector of delimiters.
            ///
            /// The equivalent character constant is DOUBLE_QUOTE.
            /// </summary>
            DoubleQuote,

            /// <summary>
            /// Specify a double quotation as the protector of delimiters.
            ///
            /// The equivalent character constant is SINGLE_QUOTE.
            /// </summary>
            SingleQuote,

            /// <summary>
            /// Specify a double quotation as the protector of delimiters.
            ///
            /// The equivalent character constant is SINGLE_QUOTE.
            /// </summary>
            BackQuote,

            /// <summary>
            /// Infrastructure: The delimiter is something besides the
            /// enumerated choices.
            ///
            /// You cannot specify this type as input to the constructor. Use
            /// the overload that takes a character.
            /// </summary>
            Other,
        }   // GuardChar


        /// <summary>
        /// Indicate whether to keep or discard field guard characters. Guards
        /// that simply appear in the body of a field are always preserved.
        /// </summary>
        public enum GuardDisposition
        {
            /// <summary>
            /// Keep guards that surround a whole field.
            /// </summary>
            Keep ,

            /// <summary>
            /// Strip (discard) guards that surround a whole field.
            /// </summary>
            Strip ,
        }   // GuardDisposition


        /// <summary>
        /// Indicate whether to trim leading and trailing space from fields.
        /// By default, leading and trailing white space is left. The other
        /// three options are self explanatory.
        /// </summary>
        public enum TrimWhiteSpace
        {
            /// <summary>
            /// Leave leading and trailing white space. Assume that its presence
            /// is meaningful. This is the default behavior.
            /// </summary>
            Leave ,

            /// <summary>
            /// Trim leading white space. This is designed specifically for use
            /// with Issuer and Subject fields of X.509 digital certificates.
            /// </summary>
            TrimLeading ,

            /// <summary>
            /// Trim trailing white space. This option is especially useful with
            /// CSV files generated by Microsoft Excel, which often have long
            /// runs of meaningless white space, especially when a worksheet has
            /// blank rows or columns in its UsedRange.
            /// </summary>
            TrimTrailing ,

            /// <summary>
            /// Given that TrimLeading and TrimTrailing are required use cases,
            /// trimming both ends is essentially free.
            ///
            /// This flag is implemented such that it can be logically processed
            /// as TrimLeading | TrimTrailing.
            /// </summary>
            TrimBoth ,
        }   // TrimWhiteSpace


        /// <summary>
        /// This property governs the static Parse methods that omit a delimiter
        /// argument.
        /// </summary>
        public const char DEFAULT_DELIMITER = COMMA;


        /// <summary>
        /// This constant governs the static Parse methods that omit a delimiter
        /// protector (guard) character.
        /// </summary>
        public const char DEFAULT_DELIMITER_GUARD = DOUBLE_QUOTE;


        /// <summary>
        /// This constant governs the static Parse methods that leave the
        /// disposition of guard characters unspecified.
        /// </summary>
        public const GuardDisposition DEFAULT_GUARD_DISPOSITION = GuardDisposition.Strip;


        /// <summary>
        /// This constant governs the static Parse methods that leave the
        /// treatment of white space unspecified. The default is the most
        /// conservative treatment.
        /// </summary>
        public const TrimWhiteSpace DEFAULT_WHITESPACE_TREATMENT = TrimWhiteSpace.Leave;
        #endregion  // Public Constants and Enumerations


        #region Private Structures, Constants, and Enumerations
        enum GuardState
        {
            Unguarded,          // There are currently no unmatched quotes.
            FieldIsGuarded,     // The first character of the current field is a guard character.
            SegmentIsGuarded,   // A guard character was found in the middle of a field.
        }   // GuardState

        struct DelimiterMap
        {
            public DelimiterChar DelimiterEnum;
            public char DelimiterCharacter;
            public string DelimiterDisplay;
        };  // struct DelimiterMap

        struct GuardMap
        {
            public GuardChar GuardEnum;
            public char GuardCharacter;
            public string GuardDisplay;
        };  // struct GuardMap

        const int ARRAY_BASE = 0;
        const int ARRAY_INDEX_TO_ORDINAL = 1;
        const int EMPTY = 0;
        const int SUBSTRING_2ND_CHAR = 1;
        const int SUBSTRING_1ST_AND_LAST = 2;
        const int DEFINED_PROTECTORS = 3;
        const int DEFINED_DELIMITERS = 5;

        const int LAST_PROTECTOR_INDEX = DEFINED_PROTECTORS - ARRAY_INDEX_TO_ORDINAL;
        const int LAST_DELIMITER_INDEX = DEFINED_DELIMITERS - ARRAY_INDEX_TO_ORDINAL;
        #endregion  // Private Structures, Constants, and Enumerations


        #region Private Static Storage and Initializer
        static object s_objSyncLock = new object ( );       // Use this object to synchronize access by multiple threads.
        static readonly DelimiterMap [ ] s_aDelimiterMap;
        static readonly GuardMap [ ] s_aGuardrMap;

        static Parser ( )
        {
            //  ----------------------------------------------------------------
            //  Create both lookup tables.
            //  ----------------------------------------------------------------

            s_aDelimiterMap = new DelimiterMap [ DEFINED_DELIMITERS ];
            s_aGuardrMap = new GuardMap [ DEFINED_PROTECTORS ];

            //  ----------------------------------------------------------------
            //  Load the delimiter mapping table.
            //  ----------------------------------------------------------------

            s_aDelimiterMap [ 0 ].DelimiterEnum = DelimiterChar.Comma;
            s_aDelimiterMap [ 0 ].DelimiterCharacter = COMMA;
            s_aDelimiterMap [ 0 ].DelimiterDisplay = Properties.Resources.DLM_DSP_COMMA;

            s_aDelimiterMap [ 1 ].DelimiterEnum = DelimiterChar.Tab;
            s_aDelimiterMap [ 1 ].DelimiterCharacter = TAB;
            s_aDelimiterMap [ 1 ].DelimiterDisplay = Properties.Resources.DLM_DSP_TAB;

            s_aDelimiterMap [ 2 ].DelimiterEnum = DelimiterChar.VerticalBar;
            s_aDelimiterMap [ 2 ].DelimiterCharacter = VERTICAL_BAR;
            s_aDelimiterMap [ 2 ].DelimiterDisplay = Properties.Resources.DLM_DSP_VERTICAL_BAR;

            s_aDelimiterMap [ 3 ].DelimiterEnum = DelimiterChar.Carat;
            s_aDelimiterMap [ 3 ].DelimiterCharacter = CARAT;
            s_aDelimiterMap [ 3 ].DelimiterDisplay = Properties.Resources.DLM_DSP_CARAT;

            s_aDelimiterMap [ 4 ].DelimiterEnum = DelimiterChar.Space;
            s_aDelimiterMap [ 4 ].DelimiterCharacter = SPACE;
            s_aDelimiterMap [ 4 ].DelimiterDisplay = Properties.Resources.DLM_DSP_SPACE;

            //  ----------------------------------------------------------------
            //  Load the delimiter protector mapping table.
            //  ----------------------------------------------------------------

            s_aGuardrMap [ 0 ].GuardEnum = GuardChar.DoubleQuote;
            s_aGuardrMap [ 0 ].GuardCharacter = DOUBLE_QUOTE;
            s_aGuardrMap [ 0 ].GuardDisplay = Properties.Resources.GRD_DSP_DBL_QUOTE;

            s_aGuardrMap [ 1 ].GuardEnum = GuardChar.SingleQuote;
            s_aGuardrMap [ 1 ].GuardCharacter = SINGLE_QUOTE;
            s_aGuardrMap [ 1 ].GuardDisplay = Properties.Resources.GRD_DSP_SGL_QUOTE;

            s_aGuardrMap [ 2 ].GuardEnum = GuardChar.BackQuote;
            s_aGuardrMap [ 2 ].GuardCharacter = BACK_QUOTE;
            s_aGuardrMap [ 2 ].GuardDisplay = Properties.Resources.GRD_DSP_BCK_QUOTE;
        }   // static Parser initializer
        #endregion  // Private Static Storage


        #region Instance Storage
        char _chrDelimiter = DEFAULT_DELIMITER;
        char _chrGuard = DEFAULT_DELIMITER_GUARD;
        DelimiterChar _enmDelimiter = DelimiterEnumFromChar ( DEFAULT_DELIMITER );
        GuardChar _enmGuard = GuardEnumFromChar ( DEFAULT_DELIMITER_GUARD );
        GuardDisposition _enmGuardDisposition = DEFAULT_GUARD_DISPOSITION;
        TrimWhiteSpace _enmTrimWhiteSpace = DEFAULT_WHITESPACE_TREATMENT;
        bool _fSettingsLocked = false;
        #endregion  // Instance Storage


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
        /// characters from a pair of emumerations that offer two mutually
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
        /// characters from a pair of emumerations that offer two mutually
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
        /// characters from a pair of emumerations that offer two mutually
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
        /// characters from a pair of emumerations that offer two mutually
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


        #region Public Properties
        /// <summary>
        /// Get or set the character currently specified as the delimiter
        /// character.
        ///
        /// Before a value is accepted, it is compared against the current
        /// delimiter guard character. If the two characters are different, the
        /// new value is accepted and saved. Otherwise, an
        /// InvalidOperationException exception is thrown.
        ///
        /// To ensure consistent behavior in a loop, once the LockSettings or
        /// Parse method on an instance is called, the property values are fixed
        /// permanently (for the remaining lifetime of the instance).
        /// </summary>
        /// <remarks>
        /// The setter method is thread-safe.
        /// </remarks>
        public char FieldDelimiter
        {
            get { return _chrDelimiter; }
            set
            {
                if ( _fSettingsLocked )
                    throw new InvalidOperationException (
                        Properties.Resources.ERRMSG_SETTINGS_ARE_LOCKED );
                else
                    lock ( s_objSyncLock )
                        if (  value != _chrGuard )
                            _chrDelimiter = value;
                        else
                            throw new InvalidOperationException (
                                ShowDelimiterAndGuardChars (
                                value ,
                                _chrGuard ) );
            }   // FieldDelimiter setter method
        }   // public char FieldDelimiter


        /// <summary>
        /// Get or set the character currently specified as the delimiter guard
        /// character.
        ///
        /// Before a value is accepted, it is compared against the current
        /// delimiter character. If the two characters are different, the new
        /// value is accepted and saved. Otherwise, an InvalidOperationException
        /// exception is thrown.
        ///
        /// To ensure consistent behavior in a loop, once the LockSettings or
        /// Parse method on an instance is called, the property values are fixed
        /// permanently (for the remaining lifetime of the instance).
        /// </summary>
        /// <remarks>
        /// The setter method is thread-safe.
        /// </remarks>
        public char DelimiterGuard
        {
            get { return _chrGuard; }
            set
            {
                if ( _fSettingsLocked )
                    throw new InvalidOperationException (
                        Properties.Resources.ERRMSG_SETTINGS_ARE_LOCKED );
                else
                    lock ( s_objSyncLock )
                        if ( value != _chrDelimiter )
                            _chrGuard = value;
                        else
                            throw new InvalidOperationException (
                                ShowDelimiterAndGuardChars (
                                _chrDelimiter ,
                                value ) );
            }   // DelimiterGuard setter method
        }   // public char DelimiterGuard property


        /// <summary>
        /// Get or set the flag implemented by the GuardDisposition enumeration
        /// that indicates whether delimiter guard characters surrounding a
        /// whole filed are discarded (default) or kept.
        ///
        /// To ensure consistent behavior in a loop, once the LockSettings or
        /// Parse method on an instance is called, the property values are fixed
        /// permanently (for the remaining lifetime of the instance).
        /// </summary>
        /// <remarks>
        /// The setter method is thread-safe.
        /// </remarks>
        public GuardDisposition GuardCharDisposition
        {
            get { return _enmGuardDisposition; }
            set
            {
                if ( _fSettingsLocked )
                    throw new InvalidOperationException (
                        Properties.Resources.ERRMSG_SETTINGS_ARE_LOCKED );
                else
                    lock ( s_objSyncLock )
                        _enmGuardDisposition = value;
            }   // GuardDisposition setter method
        }   // public GuardDisposition GuardCharDisposition


        /// <summary>
        /// Get or set the flag implemented by the TrimWhiteSpace enumeration
        /// that indicates whether leading and trailing white space is kept
        /// (default) or discarded if at the beginning, end, or both.
        ///
        /// To ensure consistent behavior in a loop, once the LockSettings or
        /// Parse method on an instance is called, the property values are fixed
        /// permanently (for the remaining lifetime of the instance).
        /// </summary>
        /// <remarks>
        /// The setter method is thread-safe.
        /// </remarks>
        public TrimWhiteSpace WhiteSpaceDisposition
        {
            get { return _enmTrimWhiteSpace; }
            set
            {
                if ( _fSettingsLocked )
                    throw new InvalidOperationException (
                        Properties.Resources.ERRMSG_SETTINGS_ARE_LOCKED );
                else
                    lock ( s_objSyncLock )
                        _enmTrimWhiteSpace = value;
            }   // TrimWhiteSpace setter method
        }   // public TrimWhiteSpace WhiteSpaceDisposition


        /// <summary>
        /// This read only flag indicates whether the the other settings are
        /// locked, and cannot henceforth be changed, for the remaining lifetime
        /// of the instance.
        ///
        /// There are two ways for settings to become locked.
        ///
        /// 1) Call the instance Parse method.
        ///
        /// 2) Call the LockSettings method.
        /// </summary>
        public bool SettingsLocked { get { return _fSettingsLocked; } }
        #endregion  // Public Properties


        #region Public Methods
        /// <summary>
        /// Lock the properties against changes.
        ///
        /// IMPORTANT: Once this method is called on an instance, subsequent
        /// attempts to set any of its properties are punished, without a trial,
        /// by an InvalidOperationException exception.
        /// </summary>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public void LockSettings ( )
        {
            lock ( s_objSyncLock )
                _fSettingsLocked = true;
        }   // public void LockSettings


        /// <summary>
        /// Use the properties set on the current Parser instance to parse any
        /// valid CSV string.
        /// </summary>
        /// <param name="pstrAnyCSV">
        /// The string may be any type of well formed CSV string. See Remoarks
        /// on the like named static method.
        /// </param>
        /// <returns>
        /// The return value is the array of fields parsed from the string.
        /// </returns>
        /// <remarks>
        /// This method is thread-safe.
        /// </remarks>
        public string [ ] Parse ( string pstrAnyCSV )
        {
            LockSettings ( );
            return Parser.Parse (
                pstrAnyCSV ,
                _chrDelimiter ,
                _chrGuard ,
                _enmGuardDisposition ,
                _enmTrimWhiteSpace );
        }   // public string [ ] Parse


        /// <summary>
        /// Call this method to parse any CSV string without working from an
        /// instance of the Parser class, specifying both the field delimiter
        /// and a separate character to protect delimiters embedded in fields.
        /// </summary>
        /// <param name="pstrAnyCSV">
        /// The string may be any type of well formed CSV string. See Remoarks.
        /// </param>
        /// <param name="pchrDelimiter">
        /// Specify the character to treat as the delimiter. Any character that
        /// can occur in a text file is valid.
        /// </param>
        /// <param name="pchrProtector">
        /// Specify the character to treat as the protector of delimiters. Any
        /// character that can occur in a text file is valid.
        /// </param>
        /// <param name="penmGuardDisposition">
        /// Specify whether guard characters that surround a whole field should
        /// be stripped (default) or kept.
        /// </param>
        /// <param name="penmTrimWhiteSpace">
        /// Specify whether leading or trailing white space should be trimmed
        /// from a field.
        /// </param>
        /// <returns>
        /// The return value is the array of fields parsed from the string.
        /// </returns>
        /// <remarks>
        /// Rather than wastefully coding the same routine twice, parsing for
        /// both instances of the Parser class and stand-alone parse requests
        /// that accept defaults for one or both of the character parameters is
        /// handled by this static method. This is easily accomplished by having
        /// the instance method pass in its current property values, along with
        /// the string, which always accompanies the request.
        /// </remarks>
        public static string [ ] Parse (
            string pstrAnyCSV ,
            char pchrDelimiter ,
            char pchrProtector ,
            GuardDisposition penmGuardDisposition ,
            TrimWhiteSpace penmTrimWhiteSpace )
        {
            List<string> lstSubstrings = new List<string> ( );

            if ( string.IsNullOrEmpty ( pstrAnyCSV ) )
            {
                lstSubstrings.Add ( string.Empty );
            }   // TRUE (degenerate case) block, if ( string.IsNullOrEmpty ( pstrAnyCSV ) )
            else
            {   // String contains something. Start scanning for delimiters and guards.
                bool fInProgress = false;                           // Initialize both flags.
                bool fProtectDelimiters = false;

                int intTotalChars = pstrAnyCSV.Length;              // Save lots of trips into the Length property of the string.
                StringBuilder sbCurrentField = new StringBuilder ( intTotalChars ); // The worst case is a string devoid of delimiters.

                for ( int intCurrChar = ARRAY_BASE ;
                          intCurrChar < intTotalChars ;
                          intCurrChar++ )
                {
                    //  --------------------------------------------------------
                    //  Process delimiters first. A test for guarded delimiters
                    //  preserves any that are.
                    //  --------------------------------------------------------

                    if ( pstrAnyCSV [ intCurrChar ] == pchrDelimiter )
                    {
                        if ( fInProgress )
                        {
                            if ( fProtectDelimiters )
                            {
                                sbCurrentField.Append ( pstrAnyCSV [ intCurrChar ] );
                            }
                            else
                            {
                                lstSubstrings.Add ( TransformAsDirectoed (
                                    sbCurrentField ,
                                    pchrProtector ,
                                    penmGuardDisposition ,
                                    penmTrimWhiteSpace ) );
                                sbCurrentField.Remove (
                                    ARRAY_BASE ,
                                    sbCurrentField.Length );
                                fInProgress = false;                    // Reset both state flags.
                                fProtectDelimiters = false;
                            }
                        }   // TRUE (normal case) block, if ( fInProgress )
                        else
                        {   // The field is empty. Add the empty string, and ignore the empty StringBuilder.
                            lstSubstrings.Add ( string.Empty );
                        }   // FALSE (degenerate case) block, if ( fInProgress )
                    }   // TRUE block, if ( pstrAnyCSV [ intCurrChar ] == pchrDelimiter )

                    //  --------------------------------------------------------
                    //  Guard characters get their own block.
                    //  --------------------------------------------------------

                    else if ( pstrAnyCSV [ intCurrChar ] == pchrProtector )
                    {   // Keep all quotes. We'll deal with it at the end.
                        sbCurrentField.Append ( pstrAnyCSV [ intCurrChar ] );
                        fProtectDelimiters = !fProtectDelimiters;   // Toggle the flag.
                    }   // else if ( pstrAnyCSV [ intCurrChar ] == pchrDelimiter )
                    else
                    {   // If neither a delimiter, nor a guard it is, keep it, and change the FieldState flag.
                        fInProgress = true;
                        sbCurrentField.Append ( pstrAnyCSV [ intCurrChar ] );
                    }   // if ( pstrAnyCSV [ intCurrChar ] == pchrDelimiter ) and else if ( pstrAnyCSV [ intCurrChar ] == pchrDelimiter ) are both false
                }   // for ( int intCurrChar=ARRAY_BASE; intCurrChar < intTotalChars; intCurrChar++ )

                if ( fInProgress )
                {
                    lstSubstrings.Add ( TransformAsDirectoed (
                        sbCurrentField ,
                        pchrProtector ,
                        penmGuardDisposition ,
                        penmTrimWhiteSpace ) );
                }   // if ( fInProgress )
                else if ( pstrAnyCSV [ intTotalChars - ARRAY_INDEX_TO_ORDINAL ] == pchrDelimiter )
                {
                    lstSubstrings.Add ( string.Empty );
                }   // else if ( pstrAnyCSV [ intTotalChars - ARRAY_INDEX_TO_ORDINAL ] == pchrDelimiter )
            }   // FALSE (expected case) block, if ( string.IsNullOrEmpty ( pstrAnyCSV ) )

            return lstSubstrings.ToArray ( );
        }   // public static string [ ] Parse (1 of 4)


        /// <summary>
        /// Call this method to parse any CSV string without working from an
        /// instance of the Parser class, specifying both the field delimiter
        /// and a separate character to protect delimiters embedded in fields.
        /// </summary>
        /// <param name="pstrAnyCSV">
        /// The string may be any type of well formed CSV string. See Remoarks.
        /// </param>
        /// <param name="pchrDelimiter">
        /// Specify the character to treat as the delimiter. Any character that
        /// can occur in a text file is valid.
        /// </param>
        /// <param name="pchrProtector">
        /// Specify the character to treat as the protector of delimiters. Any
        /// character that can occur in a text file is valid.
        /// </param>
        /// <param name="penmGuardDisposition">
        /// Specify whether guard characters that surround a whole field should
        /// be stripped (default) or kept.
        /// </param>
        /// <returns>
        /// The return value is the array of fields parsed from the string.
        /// </returns>
        /// <remarks>
        /// </remarks>
        public static string [ ] Parse (
            string pstrAnyCSV ,
            char pchrDelimiter ,
            char pchrProtector ,
            GuardDisposition penmGuardDisposition )
        {
            return Parser.Parse (
                pstrAnyCSV ,
                pchrDelimiter ,
                pchrProtector ,
                penmGuardDisposition ,
                DEFAULT_WHITESPACE_TREATMENT );
        }   // public static string [ ] Parse (2 of 4)


        /// <summary>
        /// Call this method to parse any CSV string without working from an
        /// instance of the Parser class, specifying both the field delimiter
        /// and a separate character to protect delimiters embedded in fields.
        /// </summary>
        /// <param name="pstrAnyCSV">
        /// The string may be any type of well formed CSV string. See Remoarks.
        /// </param>
        /// <param name="pchrDelimiter">
        /// Specify the character to treat as the delimiter. Any character that
        /// can occur in a text file is valid.
        /// </param>
        /// <param name="pchrProtector">
        /// Specify the character to treat as the protector of delimiters. Any
        /// character that can occur in a text file is valid.
        /// </param>
        /// <returns>
        /// The return value is the array of fields parsed from the string.
        /// </returns>
        /// <remarks>
        /// Rather than wastefully coding the same routine twice, parsing for
        /// both instances of the Parser class and stand-alone parse requests
        /// that accept defaults for one or both of the character parameters is
        /// handled by this static method. This is easily accomplished by having
        /// the instance method pass in its current property values, along with
        /// the string, which always accompanies the request.
        /// </remarks>
        public static string [ ] Parse (
            string pstrAnyCSV ,
            char pchrDelimiter ,
            char pchrProtector )
        {
            return Parser.Parse (
                pstrAnyCSV ,
                pchrDelimiter ,
                pchrProtector ,
                DEFAULT_GUARD_DISPOSITION ,
                DEFAULT_WHITESPACE_TREATMENT );
        }   // public static string [ ] Parse (3 of 4)


        /// <summary>
        /// Call this method to parse any CSV string without working from an
        /// instance of the Parser class, specifying the field delimiter, and
        /// using the default delimiter guard character, the double quotation
        /// mark.
        /// </summary>
        /// <param name="pstrAnyCSV">
        /// The string may be any type of well formed CSV string. See Remoarks.
        /// </param>
        /// <param name="pchrDelimiter">
        /// Specify the character to treat as the delimiter. Any character that
        /// can occur in a text file is valid.
        /// </param>
        /// <returns>
        /// The return value is the array of fields parsed from the string.
        /// </returns>
        public static string [ ] Parse (
            string pstrAnyCSV ,
            char pchrDelimiter )
        {
            return Parser.Parse (
                pstrAnyCSV ,
                pchrDelimiter ,
                DEFAULT_DELIMITER_GUARD ,
                DEFAULT_GUARD_DISPOSITION ,
                DEFAULT_WHITESPACE_TREATMENT );
        }   // public static string [ ] Parse (4 of 4)


        /// <summary>
        /// Call this method to parse any CSV string without working from an
        /// instance of the Parser class, using the default delimiter character
        /// and  the default delimiter guard character, the double quotation
        /// mark. See Remarks.
        /// </summary>
        /// <param name="pstrAnyCSV">
        /// The string may be any type of well formed CSV string. See Remoarks.
        /// </param>
        /// <returns>
        /// The return value is the array of fields parsed from the string.
        /// </returns>
        /// <remarks>
        /// This method gets its own name, to distinguish it from a similarly
        /// named instance method that has the same signature. This is just as
        /// well, because its distictive name clarifies that its use is limited
        /// to true Comma Separated Values (CSV) strings.
        /// </remarks>
        public static string [ ] StandardCSVParse ( string pstrAnyCSV )
        {
            return Parser.Parse (
                pstrAnyCSV ,
                DEFAULT_DELIMITER ,
                DEFAULT_DELIMITER_GUARD ,
                DEFAULT_GUARD_DISPOSITION ,
                DEFAULT_WHITESPACE_TREATMENT );
        }   // public static string [ ] StandardCSVParse
        #endregion  // Public Methods


        #region Regular Private Static Methods
        private static char DelimiterCharFromEnum ( DelimiterChar penmDelimiter )
        {
            foreach ( DelimiterMap dm in s_aDelimiterMap )
                if ( dm.DelimiterEnum == penmDelimiter )
                    return dm.DelimiterCharacter;

            throw new InvalidEnumArgumentException (
                "penmDelimiter" ,
                ( int ) penmDelimiter ,
                typeof ( DelimiterChar ) );
        }   // private static char DelimiterCharFromEnum


        private static char GuardCharFromEnum ( GuardChar penmGuardChar )
        {
            foreach ( GuardMap pm in s_aGuardrMap )
                if ( pm.GuardEnum == penmGuardChar )
                    return pm.GuardCharacter;

            throw new InvalidEnumArgumentException (
                "penmGuardChar" ,
                ( int ) penmGuardChar ,
                typeof ( GuardChar ) );
        }   // private static char ProtectorCharFromEnum


        private static DelimiterChar DelimiterEnumFromChar ( char pchrDelimiter )
        {
            foreach ( DelimiterMap dm in s_aDelimiterMap )
                if ( dm.DelimiterCharacter == pchrDelimiter )
                    return dm.DelimiterEnum;

            return DelimiterChar.Other;
        }   // private static DelimiterChar DelimiterEnumFromChar


        private static GuardChar GuardEnumFromChar ( char pchrProtector )
        {
            foreach ( GuardMap pm in s_aGuardrMap )
                if ( pm.GuardCharacter == pchrProtector )
                    return pm.GuardEnum;

            return GuardChar.Other;
        }   // private static GuardChar GuardEnumFromChar


        private static string ShowDelimiterAndGuardChars (
            char pchrDelimiter ,
            char pchrGuard )
        {
            const string HEX_VALUE_2_DIGITS = @"x2";

            //  ----------------------------------------------------------------
            //  Construct a message that leaves no ambiguity about the character
            //  chosen as the field delimiter.
            //  ----------------------------------------------------------------

            string strDelimiterDisplayText = null;

            foreach ( DelimiterMap dm in s_aDelimiterMap )
                if ( dm.DelimiterCharacter == pchrDelimiter )
                    strDelimiterDisplayText = dm.DelimiterDisplay;

            if ( string.IsNullOrEmpty ( strDelimiterDisplayText ) )
                strDelimiterDisplayText = string.Format (
                    Properties.Resources.DLM_DSP_OTHER ,
                    pchrDelimiter ,
                    ( ( int ) pchrDelimiter ).ToString ( HEX_VALUE_2_DIGITS ) );

            //  ----------------------------------------------------------------
            //  Construct a message that leaves no ambiguity about the character
            //  chosen as the field delimiter guard.
            //  ----------------------------------------------------------------

            string strGuardDisplayText = null;

            foreach ( GuardMap dm in s_aGuardrMap )
                if ( dm.GuardCharacter == pchrGuard )
                    strGuardDisplayText = dm.GuardDisplay;

            if ( string.IsNullOrEmpty ( strGuardDisplayText ) )
                strGuardDisplayText = string.Format (
                    Properties.Resources.DLM_DSP_OTHER ,
                    pchrGuard ,
                    ( ( int ) pchrGuard ).ToString ( HEX_VALUE_2_DIGITS ) );

            //  ----------------------------------------------------------------
            //  Package both messages into a message that will be handed off to
            //  the InvalidOperationException Exception object that it about to
            //  spring into existence and wreak havoc on the unsuspecting user.
            //  ----------------------------------------------------------------

            return string.Format (
                Properties.Resources.ERRMSG_SAME_CHAR_AS_DLM_AND_GUARD ,
                strDelimiterDisplayText ,
                strGuardDisplayText ,
                Environment.NewLine );
        }   // private static string ShowDelimiterAndGuardChars


        private static string TransformAsDirectoed (
            StringBuilder psbCurrentField ,
            char pchrProtector ,
            GuardDisposition penmGuardDisposition ,
            TrimWhiteSpace penmTrimWhiteSpace )
        {
            int intFldLen = psbCurrentField.Length;

            if ( penmGuardDisposition == GuardDisposition.Keep )
                return TransformWhiteSpace (
                    psbCurrentField.ToString ( ) ,
                    penmTrimWhiteSpace );
            else if ( psbCurrentField [ ARRAY_BASE ] == pchrProtector
                 && psbCurrentField [ intFldLen - ARRAY_INDEX_TO_ORDINAL ] == pchrProtector )
                return TransformWhiteSpace (
                    psbCurrentField.ToString (
                        SUBSTRING_2ND_CHAR ,
                        intFldLen - SUBSTRING_1ST_AND_LAST ) ,
                        penmTrimWhiteSpace );
            else
                return TransformWhiteSpace (
                    psbCurrentField.ToString ( ) ,
                    penmTrimWhiteSpace );
        }   // private static string TransformAsDirectoed


        private static string TransformWhiteSpace (
            string pstrAlmostReady ,
            TrimWhiteSpace penmTrimWhiteSpace )
        {
            switch ( penmTrimWhiteSpace )
            {
                case TrimWhiteSpace.Leave:
                    return pstrAlmostReady;
                case TrimWhiteSpace.TrimLeading:
                    return pstrAlmostReady.TrimStart ( null );
                case TrimWhiteSpace.TrimTrailing:
                    return pstrAlmostReady.TrimEnd ( null );
                case TrimWhiteSpace.TrimBoth:
                    return pstrAlmostReady.Trim ( );
                default:
                    throw new InvalidEnumArgumentException (
                        "penmTrimWhiteSpace" ,
                        ( int ) penmTrimWhiteSpace ,
                        typeof ( TrimWhiteSpace ) );
            }   // switch ( penmTrimWhiteSpace )
        }   // private static string TransformWhiteSpace
        #endregion  // Regular Private Static Methods
    }   // public class Parser
}   // namespace WizardWrx.AnyCSV