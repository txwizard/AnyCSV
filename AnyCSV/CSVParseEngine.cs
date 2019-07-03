/*
    ============================================================================

    Namespace:          WizardWrx.AnyCSV

    Class Name:         CSVParseEngine

    File Name:          CSVParseEngine.cs

    Synopsis:           This class defines the managed implementation of my
                        robust CSV parsing algorithm.

	Remarks:			It wouldn't surprise me to discover that the base class
						being abstract trips up COM Interop.

	Reference:			1)	"C++ calling C# COM Interop Error: HRESULT 0x80131509"
							http://stackoverflow.com/questions/9093531/c-calling-c-sharp-com-interop-error-hresult-0x80131509

						2)	"Issues with building a project with "Register for COM interop" for a 64-bit assembly"
							https://support.microsoft.com/en-us/help/956933/issues-with-building-a-project-with-register-for-com-interop-for-a-64
							Retrieved 2017/08/05 07:28:24

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

    Begun:				Saturday, 17 September 2016

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Description
    ---------- ------- ------ --------------------------------------------------
    2016/10/01 4.0     DAG    Move the guts of the Parser class, the original
                              implementation of my CSV parsing algorithm, into
                              this class.

    2017/08/05 5.0     DAG    Change CPU architecture from x86 to MSIL, and
                              record the COM registration in the 64 bit Registry
                              as described in the second reference cited above.

    2018/01/03 7.0     DAG    Sign the assembly with a strong name, so that it
                              can go into a Global Assembly Cache. Since there
                              are no other changes, there is no debug build.

    2018/09/03 7.0     DAG    Amend ToString to display character codes as both
	                          raw characters and decimal values.

    2019/07/03 7.1     DAG    Synchronize the format control string used by the
                              ToString override to align with the format items
                              added in version 7.0, and add the overlooked
                              abstract marking, and correct errors in the XML
                              help text of two GuardChar enumeration members.
    ============================================================================
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;


namespace WizardWrx.AnyCSV
{
	/// <summary>
	/// This abstract class encapsulates the core components of my robust string
	/// parsing algorithm, along with the code and data to support a set of 
	/// commonly used delimiter and guard characters. 
	/// 
	/// RobustDelimitedStringParser, a companion class defined in the same
	/// assembly, is a basic concrete instance of this class, which contains the
	/// bare essentials needed to expose its functionality to COM. The original
	/// class, Parser, extends RobustDelimitedStringParser with overloaded class
	/// constructors and Parse methods.
	/// </summary>
	[ComVisible ( true ) ,
		   Guid ( "EE63E545-0FC1-42F0-9DDD-028A5FFD438F" )]
	//	[ClassInterface ( ClassInterfaceType.None )]			<<-- I think this is another one of these cases in which the sanctioned approach isn't worth the trouble for its alleged benefits.
	[ClassInterface ( ClassInterfaceType.AutoDual )]	//	    <<-- This is how my AssemblyPropertyViewer class is marked.
	public abstract class CSVParseEngine : ICSVParser
	{
        #region Public Constants and Enumerations
		/// <summary>
		/// Protect delimiters enclosed in backwards quotation marks, commonly
		/// called back-ticks.
		///
		/// The equivalent GuardChar member is BackQuote (2), and its integral
		/// value is 0x60 (96 decimal).
		/// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char BACK_QUOTE = '\x0060';			// Protect enclosed delimiter characters that are enclosed in double quotation marks (ASCII code 096 = 0x0060).


		/// <summary>
		/// Use this symbolic constant to construct a Parser instance that uses
		/// a carat ('^') as its delimiter, or to specify one to the static
		/// Parse method.
		///
		/// The equivalent DelimiterChar member is Carat (3), and its integral
		/// value is 0x5e (94 decimal).
		/// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char CARAT = '^';						// Treat the space character (ASCII code 094 = 0x005E) as a field delimiter, unless it falls within a pair of guard characters.


		/// <summary>
		/// Use this symbolic constant to construct a Parser instance that uses
		/// a Carriage Return (CR) character as its delimiter, or to specify one 
		/// to the static Parse method.
		/// 
		/// The equivalent DelimiterChar member is CarriageReturn (5), and its
		/// integral value is 0x0D (013 decimal).
		/// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char CARRIAGE_RETURN = '\r';			// Treat the Carriage Return (ASCII code 013 = 0x000D) as a field delimiter, unless it falls within a pair of guard characters.


		/// <summary>
        /// Use this symbolic constant to construct a Parser instance that uses
        /// a comma (',') as its delimiter, or to specify one to the static
        /// Parse method.
        ///
		/// The equivalent DelimiterChar member is Comma (0), and its integral 
		/// value is 0x2c (44 decimal).
        /// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char COMMA = ',';						// Treat the space character (ASCII code 044 = 0x002C) as a field delimiter, unless it falls within a pair of guard characters.


		/// <summary>
		/// Protect delimiters enclosed in double quotation marks.
		///
		/// The equivalent GuardChar member is DoubleQuote (0), and its integral
		/// value is 0x22 (34 decimal).
		/// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char DOUBLE_QUOTE = '\x0022';			// Protect enclosed delimiter characters that are enclosed in double quotation marks (ASCII code 034 = 0x0022).


		/// <summary>
		/// Use this symbolic constant to construct a Parser instance that uses
		/// a Line Feed (LF) character as its delimiter, or to specify one 
		/// to the static Parse method.
		/// 
		/// The equivalent DelimiterChar member is LineFeed (6), and its
		/// integral value is 0x0A (010 decimal).
		/// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char LINE_FEED = '\n';					// Treat the Line Feed (ASCII code 010 = 0x000A) as a field delimiter, unless it falls within a pair of guard characters.


		/// <summary>
		/// Protect delimiters enclosed in single quotation marks.
		///
		/// The equivalent GuardChar member is SingleQuote (1), and its integral
		/// value is 0x27 (39 decimal).
		/// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char SINGLE_QUOTE = '\x0027';			// Protect enclosed delimiter characters that are enclosed in single quotation marks (ASCII code 039 = 0x0027).


		/// <summary>
		/// Use this symbolic constant to construct a Parser instance that uses
		/// a space (' ') as its delimiter, or to specify one to the static
		/// Parse method.
		///
		/// The equivalent DelimiterChar member is Space (4), and its integral
		/// value is 0x20 (32 decimal).
		/// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char SPACE = '\x0020';					// Treat the space character (ASCII code 032 = 0x0020) as a field delimiter, unless it falls within a pair of guard characters.


		/// <summary>
        /// Use this symbolic constant to construct a Parser instance that uses
        /// a tab ('\t") as its delimiter, or to specify one to the static Parse
        /// method.
        ///
        /// The equivalent DelimiterChar member is Tab (1), and its integral value
		/// is 9.
        /// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char TAB = '\t';						// Treat the space character (ASCII code 009 = 0x0009) as a field delimiter, unless it falls within a pair of guard characters.


		/// <summary>
		/// Use this symbolic constant to construct a Parser instance that uses
		/// a vertical bar ('|'), also known as the Pipe character, vertical 
		/// slash, bar, obelisk, and various other names, as its delimiter, or
		/// to specify one to the static Parse method.
		///
		/// The equivalent DelimiterChar member is VerticalBar (2), and its integral
		/// value is 0x7C (124 decimal).
		/// </summary>
		[ComVisible ( true )]
		[MarshalAs ( UnmanagedType.I1 )]
		public const char VERTICAL_BAR = '|';				// Treat the space character (ASCII code 124 = 0x007C) as a field delimiter, unless it falls within a pair of guard characters.


        /// <summary>
        /// The DelimiterChar enumeration simplifies specifying any of the
		/// commonly used field delimiter characters. 
		/// 
		/// All but the first and last values map to one of the public constants
		/// defined by this class.
        /// </summary>
		[ComVisible ( true ) ,
			   Guid ( "EBC3549F-FE2A-4CB1-9006-0F48427F2A7A" )]
		public enum DelimiterChar
        {
			/// <summary>
			/// This value indicates that the DelimiterChar is uninitialized.
			/// </summary>
			None = 0 ,

			/// <summary>
			/// Specify a CARAT (^) as the delimiter.
			///
			/// The equivalent character constant is CARAT.
			/// </summary>
			Carat = 1 ,
			
			/// <summary>
			/// Specify a Carriage Return (CR) control character as the delimiter.
			/// 
			/// The equivalent character constant is CARRIAGE_RETURN.
			/// </summary>
			CarriageReturn = 2 ,

			/// <summary>
            /// Specify a comma as the delimiter.
            ///
            /// The equivalent character constant is COMMA.
            /// </summary>
            Comma = 3,

			/// <summary>
			/// Specify a Line Feed (LF) control character as the delimiter.
			///
			/// The equivalent character constant is LINE_FEED.
			/// </summary>
			LineFeed = 4,

			/// <summary>
			/// Specify a SPACE (' ') as the delimiter.
			///
			/// The equivalent character constant is SPACE.
			/// </summary>
			Space = 5 ,

            /// <summary>
			/// Specify a tab control character as the delimiter.
            ///
            /// The equivalent character constant is TAB.
            /// </summary>
            Tab = 6,

            /// <summary>
            /// Specify a vertical bar ('|'), commonly called the pipe symbol,
            /// as the delimiter.
            ///
            /// The equivalent character constant is VERTICAL_BAR.
            /// </summary>
            VerticalBar = 7,

            /// <summary>
            /// Infrastructure: The delimiter is something besides the
            /// enumerated choices.
            ///
            /// You cannot specify this type as input to the constructor. Use
            /// the overload that takes a character.
            /// </summary>
            Other = 8
        }   // DelimiterChar


        /// <summary>
        /// The GuardChar enumeration simplifies specifying any of the
        /// commonly used field delimiter protector characters.
		/// 
		/// All but the first and last values map to one of the public constants
		/// defined by this class.
		/// </summary>
		[ComVisible ( true ) ,
			   Guid ( "A9271285-A237-4778-924F-4D0FA90F9604" )]
		public enum GuardChar
        {
			/// <summary>
			/// This value indicates that the GuardChar is uninitialized.
			/// </summary>
			None = 0 ,

            /// <summary>
            /// Specify a backwards quotation mark as the protector of delimiters.
            ///
            /// The equivalent character constant is BACK_QUOTE.
            /// </summary>
            BackQuote = 1 ,

			/// <summary>
            /// Specify a double quotation mark as the protector of delimiters.
            ///
            /// The equivalent character constant is DOUBLE_QUOTE.
            /// </summary>
            DoubleQuote = 2,

            /// <summary>
            /// Specify a single quotation mark as the protector of delimiters.
            ///
            /// The equivalent character constant is SINGLE_QUOTE.
            /// </summary>
            SingleQuote = 3,

            /// <summary>
            /// Infrastructure: The delimiter is something besides the
            /// enumerated choices.
            ///
            /// You cannot specify this type as input to the constructor. Use
            /// the overload that takes a character.
            /// </summary>
            Other = 4,
        }   // GuardChar


        /// <summary>
        /// Indicate whether to keep or discard field guard characters. Guards
		/// that simply appear in the body of a field are always preserved.
        /// </summary>
		[ComVisible ( true ) ,
			   Guid ( "71CCEE5A-CF43-43B2-A6C8-F219EEC9E30F" )]
		public enum GuardDisposition
        {
            /// <summary>
            /// Keep guards that surround a whole field.
            /// </summary>
            Keep = 0,

            /// <summary>
            /// Strip (discard) guards that surround a whole field.
            /// </summary>
            Strip = 1,
        }   // GuardDisposition


		/// <summary>
		/// Once locked, this flag tracks whether the lock was applied by an
		/// explicit call to LockSettings or by an internal call made on the
		/// first call to the Parse method. Until then, its value is IsUnlocked,
		/// which reflects the uninitialized state of its instance member.
		/// </summary>
		[ComVisible ( true ) ,
			   Guid ( "8F92D749-FD8E-4573-8D5B-5997280B370A" )]
		public enum LockMethod
		{
			/// <summary>
			/// The properties are unlocked, which is their initial state, even
			/// when the constructor sets one or more of them.
			/// </summary>
			IsUnlocked ,

			/// <summary>
			/// The LockSettings method was called explicitly by the code that
			/// called this instance into existence.
			/// </summary>
			LockedExplicitly ,

			/// <summary>
			/// The LockSettings method was called by the Parse method.
			/// </summary>
			LockedImplicitly
		}	// LockMethod


		/// <summary>
		/// This flag tracks the lock state of the instance properties that
		/// govern operation of the CSV parsing engine when the instance method
		/// calls upon it to parse a string.
		/// </summary>
		[ComVisible ( true ) ,
			   Guid ( "31C2E565-76BB-4DCE-9648-530B5C83CECA" )]
		public enum LockState
		{
			/// <summary>
			/// Initially, the operating parameters are unlocked.
			/// </summary>
			Unlocked ,

			/// <summary>
			/// When the object is locked, either explicitly by calling the
			/// LockSettings method on the instance, or implicitly, the first
			/// time that an instance Parse method is called, whichever comes
			/// first, a private LockState member transitions to this state.
			/// 
			/// Thereafter, it is an error to change any property, and any
			/// attempt to do so elicits an InvalidOperationException exception.
			/// </summary>
			Locked
		};	// LockState


        /// <summary>
        /// Indicate whether to trim leading and trailing space from fields. By
		/// default, leading and trailing white space is left. The other three
		/// options are self explanatory.
        /// </summary>
		[ComVisible ( true ) ,
			   Guid ( "093B06A4-2450-430E-8E67-0D988638FE2A" )]
		public enum TrimWhiteSpace
        {
            /// <summary>
            /// Leave leading and trailing white space. Assume that its presence
            /// is meaningful. This is the default behavior.
            /// </summary>
            Leave = 0,

            /// <summary>
            /// Trim leading white space. This is designed specifically for use
            /// with Issuer and Subject fields of X.509 digital certificates.
            /// </summary>
            TrimLeading = 1,

            /// <summary>
            /// Trim trailing white space. This option is especially useful with
            /// CSV files generated by Microsoft Excel, which often have long
            /// runs of meaningless white space, especially when a worksheet has
            /// blank rows or columns in its UsedRange.
            /// </summary>
            TrimTrailing = 2,

            /// <summary>
            /// Given that TrimLeading and TrimTrailing are required use cases,
            /// trimming both ends is essentially free.
            ///
            /// This flag is implemented such that it can be logically processed
            /// as TrimLeading | TrimTrailing.
            /// </summary>
            TrimBoth = 3,
        }   // TrimWhiteSpace


        /// <summary>
        /// This property governs the static Parse methods that omit a delimiter
        /// argument.
        /// </summary>
		[ComVisible ( false )]
		public const char DEFAULT_DELIMITER = COMMA;


        /// <summary>
        /// This constant governs the static Parse methods that omit a delimiter
        /// protector (guard) character.
        /// </summary>
		[ComVisible ( false )]
		public const char DEFAULT_DELIMITER_GUARD = DOUBLE_QUOTE;


        /// <summary>
        /// This constant governs the static Parse methods that leave the
        /// disposition of guard characters unspecified.
        /// </summary>
		[ComVisible ( false )]
		public const GuardDisposition DEFAULT_GUARD_DISPOSITION = GuardDisposition.Strip;


        /// <summary>
        /// This constant governs the static Parse methods that leave the
        /// treatment of white space unspecified. The default is the most
        /// conservative treatment.
        /// </summary>
		[ComVisible ( false )]
		public const TrimWhiteSpace DEFAULT_WHITESPACE_TREATMENT = TrimWhiteSpace.Leave;
        #endregion  // Public Constants and Enumerations


        #region Private Structures, Constants, and Enumerations
		/// <summary>
		/// Specify whether and why the current character is guarded.
		/// </summary>
        enum GuardState
        {
			/// <summary>
			/// The current character is unguarded.
			/// </summary>
            Unguarded,										// There are currently no unmatched quotes.

			/// <summary>
			/// The current character belongs to a guarded field.
			/// </summary>
            FieldIsGuarded,									// The first character of the current field is a guard character.

			/// <summary>
			/// The current character belongs to a guarded segment of a field.
			/// </summary>
            SegmentIsGuarded,								// A guard character was found in the middle of a field.
        }   // GuardState


		/// <summary>
		/// An element of an array of these structures identifies the delimiter
		/// character, the corresponding member of the DelimiterChar enumeration,
		/// and a string representation to display on reports and in debugger
		/// windows.
		/// </summary>
		struct DelimiterMap
        {
			/// <summary>
			/// The DelimiterChar enumeration does double duty, both as an index
			/// for the array of these structures that defines preset delimiter
			/// characters, and as a convenient mechanism for using unambiguous
			/// numeric constants as inputs to the parsing engine.
			/// </summary>
            public DelimiterChar DelimiterEnum;

			/// <summary>
			/// This member stores a preset character, to simplify using several
			/// popular field delimiter characters, most of which are tricky to
			/// define correctly in code.
			/// </summary>
            public char DelimiterCharacter;

			/// <summary>
			/// This member stores a preset string, constructed from a template,
			/// that displays the character, itself, or a literal proxy, along
			/// with decimal and hexadecimal representations of its numeric
			/// value.
			/// </summary>
            public string DelimiterDisplay;

			/// <summary>
			/// The purpose of this constructor is to permit the array of preset
			/// delimiters to be constructed at compile time.
			/// </summary>
			/// <param name="penmDelimiterEnum">
			/// Specify the DelimiterChar member to store into the DelimiterEnum
			/// member.
			/// </param>
			/// <param name="pchrDelimiterCharacter">
			/// Specify the character to store into the DelimiterCharacter
			/// member.
			/// </param>
			/// <param name="pstrDelimiterDisplay">
			/// Specify the string to store into the DelimiterDisplay member.
			/// </param>
			public DelimiterMap ( 
				DelimiterChar penmDelimiterEnum, 
				char pchrDelimiterCharacter, 
				string pstrDelimiterDisplay)
			{
				this.DelimiterEnum = penmDelimiterEnum;
				this.DelimiterCharacter = pchrDelimiterCharacter;
				this.DelimiterDisplay = pstrDelimiterDisplay;
			}	// public DelimiterMap constructor

			/// <summary>
			/// Take control of the string representation of this structure.
			/// </summary>
			/// <returns>
			/// Use the DelimiterDisplay member as the string representation.
			/// </returns>
			/// <remarks>
			/// The default string representation of a user defined type is its
			/// absolute (fully qualified) type name, which isn't much use in a
			/// debugger windows, since such a rendering duplicates information
			/// that appears in the last column anyway. On the other hand, a
			/// labeled display of the properties means that the value shown in
			/// the debug window is immediately useful, and expanding the member
			/// list is unnecessary.
			/// </remarks>
			public override string ToString ( )
			{
				return this.DelimiterDisplay;
			}	// public ToString method override
			
			/// <summary>
			/// Take control of the IEquatable interface implementation of this
			/// structure.
			/// </summary>
			/// <param name="obj">
			/// Specify the other GuardMap structure against which to compare
			/// this structure.
			/// </param>
			/// <returns>
			/// Equality is evaluated based on the values of the GuardEnum and
			/// GuardCharacter members of each structure.
			/// </returns>
			/// <remarks>
			/// Taking control of the IEquatable implementation partially paves
			/// the way for sorting the list in a meaningful fashion. Though it
			/// is probably overkill, it's ready if needed for some future 
			/// extension.
			/// </remarks>
			public override bool Equals ( object obj )
			{
				if ( obj.GetType ( ) == typeof ( DelimiterMap ) )
				{	// Cast once, use twice.
					DelimiterMap Comparand = ( DelimiterMap ) obj;
					return this.DelimiterEnum.Equals ( Comparand.DelimiterEnum ) & this.DelimiterCharacter.Equals ( Comparand.DelimiterCharacter );
				}	// TRUE (anticipated outcome) block, if ( obj.GetType ( ) == typeof ( GuardChar ) )
				else
				{
					throw new InvalidCastException ( FormatInvalidCastExceptionMessage ( this, obj ) );
				}	// FALSE (unanticipated outcome) block, if ( obj.GetType ( ) == typeof ( GuardChar ) )
			}	// Public Equals method override


			/// <summary>
			/// Since I overrode the Equals method, the compiler insists that I
			/// must also override GetHashCode. That being the case, I'll accept
			/// the default method generated by the compiler.
			/// </summary>
			/// <returns></returns>
			public override int GetHashCode ( )
			{
				return base.GetHashCode ( );
			}	// public GetHashCode method override
		};  // DelimiterMap


		/// <summary>
		/// An element of an array of these structures identifies the delimiter
		/// guard character, the corresponding member of the GuardChar 
		/// enumeration, and a string representation to display on reports and
		/// in debugger windows.
		/// </summary>
        struct GuardMap
        {
			/// <summary>
			/// The GuardChar enumeration does double duty, both as an index
			/// for the array of these structures that defines preset delimiter
			/// guard characters, and as a convenient mechanism for using
			/// unambiguous numeric constants as inputs to the parsing engine.
			/// </summary>
            public GuardChar GuardEnum;

			/// <summary>
			/// This member stores a preset character, to simplify using several
			/// popular field delimiter guard characters, most of which are 
			/// tricky to define correctly in code.
			/// </summary>
            public char GuardCharacter;

			/// <summary>
			/// This member stores a preset string, constructed from a template,
			/// that displays the character, itself, or a literal proxy, along
			/// with decimal and hexadecimal representations of its numeric
			/// value.
			/// </summary>
            public string GuardDisplay;

			public GuardMap (
				GuardChar penmGuardEnum,
				char pchrGuardCharacter,
				string pstrGuardDisplay)
			{
				this.GuardEnum = penmGuardEnum;
				this.GuardCharacter = pchrGuardCharacter;
				this.GuardDisplay = pstrGuardDisplay;
			}	// public GuardMap constructor

			/// <summary>
			/// Take control of the string representation of this structure.
			/// </summary>
			/// <returns>
			/// Use the GuardDisplay member as the string representation.
			/// </returns>
			/// <remarks>
			/// The default string representation of a user defined type is its
			/// absolute (fully qualified) type name, which isn't much use in a
			/// debugger windows, since such a rendering duplicates information
			/// that appears in the last column anyway. On the other hand, a
			/// labeled display of the properties means that the value shown in
			/// the debug window is immediately useful, and expanding the member
			/// list is unnecessary.
			/// </remarks>
			[ComVisible ( false )]
			public override string ToString ( )
			{
				return this.GuardDisplay;
			}	// public ToString method override

			/// <summary>
			/// Take control of the IEquatable interface implementation of this
			/// structure.
			/// </summary>
			/// <param name="obj">
			/// Specify the other GuardMap structure against which to compare
			/// this structure.
			/// </param>
			/// <returns>
			/// Equality is evaluated based on the values of the GuardEnum and
			/// GuardCharacter members of each structure.
			/// </returns>
			/// <remarks>
			/// Taking control of the IEquatable implementation partially paves
			/// the way for sorting the list in a meaningful fashion. Though it
			/// is probably overkill, it's ready if needed for some future 
			/// extension.
			/// </remarks>
			[ComVisible ( false )]
			public override bool Equals ( object obj )
			{
				if ( obj.GetType ( ) == typeof ( GuardChar ) )
				{	// Cats once, use twice.
					GuardMap Comparand = ( GuardMap ) obj;
					return this.GuardEnum.Equals ( Comparand.GuardEnum ) & this.GuardCharacter.Equals ( Comparand.GuardCharacter );
				}	// TRUE (anticipated outcome) block, if ( obj.GetType ( ) == typeof ( GuardChar ) )
				else
				{
					throw new InvalidCastException ( FormatInvalidCastExceptionMessage ( this , obj ) );
				}	// FALSE (unanticipated outcome) block, if ( obj.GetType ( ) == typeof ( GuardChar ) )
			}	// Public Equals method override

			/// <summary>
			/// Since I overrode the Equals method, the compiler insists that I
			/// must also override GetHashCode. That being the case, I'll accept
			/// the default method generated by the compiler.
			/// </summary>
			/// <returns></returns>
			[ComVisible ( false )]
			public override int GetHashCode ( )
			{
				return base.GetHashCode ( );
			}	// public GetHashCode method override
		};  // GuardMap


		const string ARG_NAME_DELIMITER_ENUM = @"penmDelimiter";
		const string ARG_NAME_GUARD_CHAR = @"penmGuardChar";
		const string ARG_NAME_TRIM_WHITESPACE = @"penmTrimWhiteSpace";

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
		/// <summary>
		/// This array stores information about the preset delimiter characters,
		/// ordered by the integral value to which their DelimiterChar members
		/// resolve.
		/// </summary>
		static readonly DelimiterMap [ ] s_aDelimiterMap =
		{
			new DelimiterMap (
				DelimiterChar.Carat ,
				CARAT ,
				Properties.Resources.DLM_DSP_CARAT ) ,
			new DelimiterMap (
				DelimiterChar.CarriageReturn ,
				CARRIAGE_RETURN ,
				Properties.Resources.DLM_DSP_CARRIAGE_RETURN ),
			new DelimiterMap (
				DelimiterChar.Comma ,
				COMMA ,
				Properties.Resources.DLM_DSP_COMMA ),
			new DelimiterMap (
				DelimiterChar.LineFeed ,
				LINE_FEED ,
				Properties.Resources.DLM_DSP_LINE_FEED ),
			new DelimiterMap (
				DelimiterChar.Space ,
				SPACE ,
				Properties.Resources.DLM_DSP_SPACE ),
			new DelimiterMap (
				DelimiterChar.Tab ,
				TAB ,
				Properties.Resources.DLM_DSP_TAB ),
			new DelimiterMap (
				DelimiterChar.VerticalBar ,
				VERTICAL_BAR ,
				Properties.Resources.DLM_DSP_VERTICAL_BAR )
		};	// s_aDelimiterMap array


		/// <summary>
		/// This array stores information about the preset delimiter guard
		/// characters, ordered by the integral value to which their GuardChar
		/// members resolve.
		/// </summary>
		static readonly GuardMap [ ] s_aGuardrMap =
		{
			new GuardMap (
				GuardChar.BackQuote ,
				BACK_QUOTE ,
				Properties.Resources.GRD_DSP_BACK_QUOTE ) ,
			new GuardMap (
				GuardChar.DoubleQuote ,
				DOUBLE_QUOTE ,
				Properties.Resources.GRD_DSP_DOUBLE_QUOTE ) ,
			new GuardMap (
				GuardChar.SingleQuote ,
				SINGLE_QUOTE ,
				Properties.Resources.GRD_DSP_SINGLE_QUOTE )
		};	// s_aGuardrMap
        #endregion  // Private Static Storage


        #region Instance Storage
		/// <summary>
		/// Instance storage for the delimiter character is marked as protected,
		/// so that derived classes can access it directly, rather than wasting
		/// stack space and processor time to go through its public property.
		/// </summary>
        protected char _chrDelimiter = DEFAULT_DELIMITER;

		/// <summary>
		/// Instance storage for the delimiter guard character is marked as 
		/// protected, so that derived classes can access it directly, rather
		/// than wasting stack space and processor time to go through its
		/// public property.
		/// </summary>
		protected char _chrGuard = DEFAULT_DELIMITER_GUARD;

		/// <summary>
		/// Instance storage for the delimiter character enumeration is marked
		/// as protected, so that derived classes can access it directly, rather
		/// than wasting stack space and processor time to go through its public
		/// property.
		/// </summary>
		protected DelimiterChar _enmDelimiter = DelimiterEnumFromChar ( DEFAULT_DELIMITER );

		/// <summary>
		/// Instance storage for the delimiter guard character enumeration is
		/// marked as protected, so that derived classes can access it directly,
		/// rather than wasting stack space and processor time to go through its
		/// public property.
		/// </summary>
		protected GuardChar _enmGuard = GuardEnumFromChar ( DEFAULT_DELIMITER_GUARD );

		/// <summary>
		/// Instance storage for the delimiter guard character enumeration is
		/// marked as protected, so that derived classes can access it directly,
		/// rather than wasting stack space and processor time to go through its
		/// public property.
		/// </summary>
		protected GuardDisposition _enmGuardDisposition = DEFAULT_GUARD_DISPOSITION;

		/// <summary>
		/// This flag maintains the method by which the class property values
		/// became locked.
		/// </summary>
		protected LockMethod _enmLockMethod = LockMethod.IsUnlocked;

		/// <summary>
		/// Instance storage for the settings lock state enumeration is marked
		/// as protected, so that derived classes can access it directly, rather
		/// than wasting stack space and processor time to go through its public
		/// property.
		/// </summary>
		protected LockState _fSettingsLocked = LockState.Unlocked;

		/// <summary>
		/// Use this object to synchronize access by multiple threads. The only
		/// case in which this seems necessary is when the properties are edited
		/// or locked.
		/// </summary>
		protected object _objSettingsLockSync = new object ( );

		/// <summary>
		/// Use this object to synchronize access to the Parse method by multiple
		/// threads.
		/// </summary>
		/// <remarks>
		/// The parse method uses this lock only very briefly while the settings
		/// lock is evaluated and set if it isn't already.
		/// </remarks>
		protected object _objParseingLockSync = new object ( );

		/// <summary>
		/// Instance storage for the white space disposition enumeration is
		/// marked as protected, so that derived classes can access it directly,
		/// rather than wasting stack space and processor time to go through its
		/// public property.
		/// </summary>
		protected TrimWhiteSpace _enmTrimWhiteSpace = DEFAULT_WHITESPACE_TREATMENT;
		#endregion  // Instance Storage


        #region Constructors
        /// <summary>
        /// The default constructor creates a Parser that uses its namesake, the
        /// comma, as its delimiter, protects commas that occur within double
        /// quotation marks, discards double quotation marks that surround whole
        /// fields, and preserves leading and trailing white space.
        /// </summary>
		public CSVParseEngine ( ) { }   // The base class provides only the default constructor.
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
			[return: MarshalAs ( UnmanagedType.I1 )]
			get { return _chrDelimiter; }

			[param: MarshalAs ( UnmanagedType.I1 )]
			set
			{
				lock ( _objSettingsLockSync )
					if ( _fSettingsLocked == LockState.Locked )
						throw new InvalidOperationException (
							Properties.Resources.ERRMSG_SETTINGS_ARE_LOCKED );
					else
						if ( value != _chrGuard )
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
			[return: MarshalAs ( UnmanagedType.I1 )]
            get { return _chrGuard; }

			[param: MarshalAs ( UnmanagedType.I1 )]
			set
			{
				lock ( _objSettingsLockSync )
					if ( _fSettingsLocked == LockState.Locked )
						throw new InvalidOperationException (
							Properties.Resources.ERRMSG_SETTINGS_ARE_LOCKED );
					else
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
				lock ( _objSettingsLockSync )
					if ( _fSettingsLocked == LockState.Locked )
						throw new InvalidOperationException (
							Properties.Resources.ERRMSG_SETTINGS_ARE_LOCKED );
					else
						_enmGuardDisposition = value;
			}   // GuardDisposition setter method
        }   // public GuardDisposition GuardCharDisposition


		/// <summary>
		/// This read only flag indicates the event that caused the other
		/// settings to become locked.
		/// </summary>
		/// <see cref="SettingsLocked"/>
		public LockMethod SettingsLockMethod
		{
			get
			{
				lock ( _objSettingsLockSync )
					return _enmLockMethod;
			}	// LockMethod SettingsLockMethod getter method
		}	// public LockMethod SettingsLockMethod property


		/// <summary>
		/// This read only flag indicates whether the other settings are
		/// locked, and cannot henceforth be changed, for the remaining lifetime
		/// of the instance.
		///
		/// There are two ways for settings to become locked.
		///
		/// 1) Call the instance Parse method.
		///
		/// 2) Call the LockSettings method.
		/// </summary>
		/// <remarks>
		/// Since access to this property is synchronized, its value reflects
		/// the most recent change made by code running on another thread.
		/// </remarks>
		public LockState SettingsLocked
		{
			get
			{
				lock ( _objSettingsLockSync )
					return _fSettingsLocked;
			}	// SettingsLocked getter method
		}	// public LockState SettingsLocked property


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
				lock ( _objSettingsLockSync )
					if ( _fSettingsLocked == LockState.Locked )
						throw new InvalidOperationException (
							Properties.Resources.ERRMSG_SETTINGS_ARE_LOCKED );
					else
						_enmTrimWhiteSpace = value;
			}   // TrimWhiteSpace setter method
        }   // public TrimWhiteSpace WhiteSpaceDisposition
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
		/// The fact that there is no inverse (unlock) method is by design, to
		/// ensure that a series of calls to parse all records in a file use the
		/// same settings.
		/// 
        /// This method is manifestly thread-safe.
        /// </remarks>
		public void LockSettings ( )
        {
			lock ( _objSettingsLockSync )
			{	// Synchronize access to the variables affected by this method.
				_fSettingsLocked = LockState.Locked; 

				if ( _enmLockMethod == LockMethod.IsUnlocked)
				{	// The Parse method sets this flag before it calls this method.
					_enmLockMethod = LockMethod.LockedExplicitly;
				}	// if ( _enmLockMethod == LockMethod.IsUnlocked)
			}	// lock ( s_objSyncLock )
        }   // public void LockSettings


		/// <summary>
        /// Use the properties set on the current Parser instance to parse any
        /// valid CSV string.
        /// </summary>
        /// <param name="pstrAnyCSV">
        /// The string may be any type of well formed CSV string. See Remarks
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
			lock ( _objParseingLockSync )
			{	// To avoid a deadlock, this method uses a dedicated synchronization object.
				if ( _enmLockMethod == LockMethod.IsUnlocked )
				{	// Since the initial value of this flag equates to unlocked, unnecessary calls to LockSettings are easily avaoidable.
					_enmLockMethod = LockMethod.LockedImplicitly;
					LockSettings ( );
				}	// if ( _enmLockMethod == LockMethod.IsUnlocked )
			}	// lock ( _objParseingLockSync )

			return Parse (
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
        /// The string may be any type of well formed CSV string. See Remarks.
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
							}	// if ( fProtectDelimiters )
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
        /// The string may be any type of well formed CSV string. See Remarks.
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
		public static string [ ] Parse (
            string pstrAnyCSV ,
            char pchrDelimiter ,
            char pchrProtector ,
            GuardDisposition penmGuardDisposition )
        {
            return Parse (
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
        /// The string may be any type of well formed CSV string. See Remarks.
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
            return Parse (
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
        /// The string may be any type of well formed CSV string. See Remarks.
        /// </param>
        /// <param name="pchrDelimiter">
        /// Specify the character to treat as the delimiter. Any character that
        /// can occur in a text file is valid.
        /// </param>
        /// <returns>
        /// The return value is the array of fields parsed from the string.
        /// </returns>
		/// <seealso cref="StandardCSVParse"/>
		public static string [ ] Parse (
            string pstrAnyCSV ,
            char pchrDelimiter )
        {
            return Parse (
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
        /// The string may be any type of well formed CSV string. See Remarks.
        /// </param>
        /// <returns>
        /// The return value is the array of fields parsed from the string.
        /// </returns>
        /// <remarks>
        /// This method gets its own name, to distinguish it from a similarly
        /// named instance method that has the same signature. This is just as
        /// well, because its distinctive name clarifies that its use is limited
        /// to true Comma Separated Values (CSV) strings.
        /// </remarks>
		/// <seealso cref="Parse(string, char, char, GuardDisposition, TrimWhiteSpace)"/>
		public static string [ ] StandardCSVParse ( string pstrAnyCSV )
        {
            return Parse (
                pstrAnyCSV ,
                DEFAULT_DELIMITER ,
                DEFAULT_DELIMITER_GUARD ,
                DEFAULT_GUARD_DISPOSITION ,
                DEFAULT_WHITESPACE_TREATMENT );
        }   // public static string [ ] StandardCSVParse
        #endregion  // Public Methods


        #region Regular Protected Static Methods
		/// <summary>
		/// The inverse of DelimiterEnumFromChar, this method returns the 
		/// character that corresponds to the specified member of the
		/// DelimiterChar enumeration, unless the specified DelimiterChar is not
		/// that of one of the presets.
		/// </summary>
		/// <param name="penmDelimiter">
		/// Specify the DelimiterChar member to be mapped to the corresponding
		/// delimiter character. This routine treats DelimiterChar.None and
		/// DelimiterChar.Other as invalid values, and raises an
		/// InvalidEnumArgumentException Exception, as it does if its value is
		/// outright invalid (out of range).
		/// </param>
		/// <returns>
		/// The return value is the character that corresponds to the specified
		/// DelimiterChar enumeration member.
		/// </returns>
		/// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
		/// An InvalidEnumArgumentException exception is thrown if the specified
		/// DelimiterChar value is either out of range, or is either of
		/// DelimiterChar.None or DelimiterChar.None.
		/// </exception>
		/// <seealso cref="DelimiterEnumFromChar"/>
		/// <seealso cref="GuardCharFromEnum"/>
		protected static char DelimiterCharFromEnum ( DelimiterChar penmDelimiter )
        {
            foreach ( DelimiterMap dm in s_aDelimiterMap )
                if ( dm.DelimiterEnum == penmDelimiter )
                    return dm.DelimiterCharacter;

            throw new InvalidEnumArgumentException (
				ARG_NAME_DELIMITER_ENUM ,
                ( int ) penmDelimiter ,
                typeof ( DelimiterChar ) );
        }   // DelimiterCharFromEnum


		/// <summary>
		/// The Equals methods on the two structures, DelimiterMap and GuardMap,
		/// use this static method to format a detailed exception report when
		/// either is called to compare an instance with an object of the wrong
		/// type.
		/// </summary>
		/// <param name="pobjThis">
		/// This argument is the instance on which the Equals method was called.
		/// </param>
		/// <param name="pobjOther">
		/// This argument is the instance against which a comparison was 
		/// requested. This method is called when the System.Type of pobjOther
		/// differs from that of pobjThis.
		/// </param>
		/// <returns>
		/// The return value is a string that endeavors to provide the person
		/// who is tasked with investigating the exception with enough data to
		/// identify and correct its cause.
		/// </returns>
		protected static string FormatInvalidCastExceptionMessage (
			object pobjThis ,
			object pobjOther )
		{
			return string.Format (
				Properties.Resources.ERRMSG_TYPE_MISMATCH ,	// Format control string
				new object [ ]
				{
					pobjOther ,								// Format Item 0 = string representation of the comparand object
					pobjOther.GetType ( ).FullName ,		// Format Item 1 = Absolute (fully qualified) type of the comparand object
					pobjThis ,								// Format Item 2 = string representation of this object
					pobjThis.GetType ( ).FullName			// Format Item 3 = Absolute (fully qualified) type of this object
				} );
		}	// FormatInvalidCastExceptionMessage


		/// <summary>
		/// The inverse of GuardEnumFromChar, this method returns the
		/// character that corresponds to the specified member of the GuardChar
		/// enumeration, unless the specified GuardChar is not that of one of the
		/// presets.
		/// </summary>
		/// <param name="penmGuardChar">
		/// Specify the GuardMap member to be mapped to the corresponding guard
		/// character. This routine treats GuardMap.None and GuardMap.Other as
		/// invalid values, and raises an InvalidEnumArgumentException
		/// Exception, as it does if its value is outright invalid (out of
		/// range).
		/// </param>
		/// <returns>
		/// The return value is the character that corresponds to the specified
		/// GuardChar enumeration member.
		/// </returns>
		/// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
		/// An InvalidEnumArgumentException exception is thrown if the specified
		/// GuardChar value is either out of range, or is either of GuardChar.None
		/// or GuardChar.None.
		/// </exception>
		/// <seealso cref="GuardEnumFromChar"/>
		/// <seealso cref="DelimiterEnumFromChar"/>
		protected static char GuardCharFromEnum ( GuardChar penmGuardChar )
        {
            foreach ( GuardMap pm in s_aGuardrMap )
                if ( pm.GuardEnum == penmGuardChar )
                    return pm.GuardCharacter;

            throw new InvalidEnumArgumentException (
				ARG_NAME_GUARD_CHAR ,
                ( int ) penmGuardChar ,
                typeof ( GuardChar ) );
        }   // ProtectorCharFromEnum


		/// <summary>
		/// Return the DelimiterChar enumeration member that corresponds to a given
		/// character, if it is one of the presets.
		/// </summary>
		/// <param name="pchrDelimiter">
		/// Specify the character to be mapped onto the DelimiterChar
		/// enumeration.
		/// </param>
		/// <returns>
		/// If the pchrDelimiter argument is one of the preset characters, the
		/// return value is the corresponding member of the DelimiterChar
		/// enumeration. Otherwise, DelimiterChar.Other, the highest value in
		/// the enumeration, is returned.
		/// </returns>
		/// <seealso cref="DelimiterCharFromEnum"/>
		/// <seealso cref="GuardCharFromEnum"/>
        protected static DelimiterChar DelimiterEnumFromChar ( char pchrDelimiter )
        {
            foreach ( DelimiterMap dm in s_aDelimiterMap )
                if ( dm.DelimiterCharacter == pchrDelimiter )
                    return dm.DelimiterEnum;

            return DelimiterChar.Other;
        }   // DelimiterEnumFromChar


		/// <summary>
		/// Return the GuardChar enumeration member that corresponds to a given
		/// character, if it is one of the presets.
		/// </summary>
		/// <param name="pchrProtector">
		/// Specify the character to be mapped onto the GuardChar enumeration.
		/// </param>
		/// <returns>
		/// If the pchrProtector argument is one of the preset characters, the
		/// return value is the corresponding member of the GuardChar
		/// enumeration. Otherwise, GuardChar.Other, the highest value in the
		/// enumeration, is returned.
		/// </returns>
		/// <seealso cref="GuardCharFromEnum"/>
		/// <seealso cref="DelimiterCharFromEnum"/>
		protected static GuardChar GuardEnumFromChar ( char pchrProtector )
        {
            foreach ( GuardMap pm in s_aGuardrMap )
                if ( pm.GuardCharacter == pchrProtector )
                    return pm.GuardEnum;

            return GuardChar.Other;
        }   // GuardEnumFromChar


		/// <summary>
		/// Format a message that unambiguously describes the delimiter and
		/// guard characters specified for an instance.
		/// </summary>
		/// <param name="pchrDelimiter">
		/// Specify the selected delimiter character.
		/// </param>
		/// <param name="pchrGuard">
		/// Specify the selected guard character.
		/// </param>
		/// <returns>
		/// The return value is a message string that fully describes the guard
		/// and delimiter characters, including both decimal and hexadecimal
		/// representations of their numeric values.
		/// </returns>
		protected static string ShowDelimiterAndGuardChars (
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
        }   // ShowDelimiterAndGuardChars


		/// <summary>
		/// Other methods on the base class and its derived classes, usually
		/// working through a base class method, invoke this routine to put the
		/// finishing touches on a parsed substring, which may consist of
		/// trimming its guard characters, if present, trimming leading and/or
		/// trailing white space, or neither.
		/// </summary>
		/// <param name="psbCurrentField">
		/// This string is the raw substring returned by the parsing engine.
		/// </param>
		/// <param name="pchrProtector">
		/// This character is the guard character that protects delimiter
		/// characters that are to be ignored.
		/// </param>
		/// <param name="penmGuardDisposition">
		/// This flag specifies whether guard characters, if present, should be
		/// striped or left.
		/// </param>
		/// <param name="penmTrimWhiteSpace">
		/// This flag specifies whether to trim or leave leading and/or trailing
		/// white space.
		/// </param>
		/// <returns>
		/// A copy of the string, with guard characters, leading white space, and
		/// trailing white space removed if so directed.
		/// </returns>
		protected static string TransformAsDirectoed (
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
        }   // TransformAsDirectoed


		/// <summary>
		/// The companion TransformAsDirectoed method calls upon this method as
		/// needed to trim leading and/or trailing white space, depending upon
		/// the state of the TrimWhiteSpace flag.
		/// </summary>
		/// <param name="pstrAlmostReady">
		/// This string is the input that was fed to TransformAsDirectoed, with
		/// guard characters, if any, stripped from both ends.
		/// </param>
		/// <param name="penmTrimWhiteSpace">
		/// The TrimWhiteSpace flag is passed through from TransformAsDirectoed,
		/// and it determines the action taken by this routine.
		/// </param>
		/// <returns>
		/// The returned string is handed up through TransformAsDirectoed, since
		/// it is ready to return to the caller following this transformation.
		/// </returns>
		protected static string TransformWhiteSpace (
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
						ARG_NAME_TRIM_WHITESPACE ,
                        ( int ) penmTrimWhiteSpace ,
                        typeof ( TrimWhiteSpace ) );
            }   // switch ( penmTrimWhiteSpace )
        }   // TransformWhiteSpace
		#endregion  // Regular Protected Static Methods


		#region Overridden Root Class Methods
		/// <summary>
		/// Overriding the Equals method permits meaningful equality comparisons
		/// of CSVParseEngine instances.
		/// </summary>
		/// <param name="obj">
		/// To be valid, the other object must be a CSVParseEngine object or a
		/// derivative thereof. Otherwise, an InvalidCastException exception is
		/// thrown.
		/// </param>
		/// <returns>
		/// If all four public properties other than the locked state are equal,
		/// two CSVParseEngine instances are evaluated as equal to each other.
		/// Otherwise, they are evaluates as unequal.
		/// </returns>
		/// <exception cref="System.InvalidCastException">
		/// Attempting to evaluate the equality of a CSVParseEngine against an
		/// object of another type raises an InvalidCastException exception,
		/// which reports the fully qualified types of both comparands, along
		/// with a string representation of each (whatever their respective
		/// ToString methods return).
		/// </exception>
		[ComVisible ( false )]
		public override bool Equals ( object obj )
		{
			if ( this.GetType ( ) == obj.GetType ( ) )
			{	// Cast once, use twice.
				CSVParseEngine objComparand = ( CSVParseEngine ) obj;
				return ( ( this._chrDelimiter == objComparand._chrDelimiter )
					&& ( this._chrGuard == objComparand._chrGuard )
					&& ( this._enmGuardDisposition == objComparand._enmGuardDisposition )
					&& ( this._enmTrimWhiteSpace == objComparand._enmTrimWhiteSpace ) );
			}	// TRUE (anticipated outcome) block, if ( this.GetType ( ) == obj.GetType ( ) )
			else
			{
				return false;
			}	// FALSE (unanticipated outcome) block, if ( this.GetType ( ) == obj.GetType ( ) )
		}	// Equals


		/// <summary>
		/// Return the hash code of a string composed of a concatenation of the
		/// string representations of the key properties of the instance.
		/// </summary>
		/// <returns>
		/// The return value is the hash code generated from a comma-separated
		/// list of the properties of the instance.
		/// </returns>
		[ComVisible ( false )]
		public override int GetHashCode ( )
		{	// 
			return string.Format (
				Properties.Resources.HASH_CODE_TEMPLATE ,	// Format control string
				new object [ ]
				{
					_chrDelimiter ,							// Format item 0 = Delimiter character
					_chrGuard ,								// Format Item 1 = Guard character
					_enmTrimWhiteSpace ,					// Format Item 2 = White space disposition flag
					_enmGuardDisposition ,					// Format item 3 = Guard character disposition flag
					_fSettingsLocked ,						// Format Item 4 = Property lock state (Locked or Unlocked)
					_enmLockMethod							// Format Item 5 = Property Locking method (unlocked, explicit, or implicit)
				} ).GetHashCode ( );
		}	// GetHashCode


		/// <summary>
		/// Return a message that displays the state of the properties that
		/// govern the behavior of the Parse method.
		/// </summary>
		/// <returns>
		/// The returned string makes all properties visible in debugger windows
		/// without expanding the properties, and provides a quick way to list
		/// them on a report.
		/// </returns>
		[ComVisible ( false )]
		public override string ToString ( )
		{
			return string.Format (
				Properties.Resources.TOSTRING_TEMPLATE ,	// Format control string
				new object [ ]
				{
					_chrDelimiter ,							// Format item  0 = Delimiter character					                         Delimiter         = {0} (0x{1:x2}, {2} decimal){10}
					( int ) _chrDelimiter ,					// Format Item  1 = Delimiter character (hexadecimal)                            Delimiter         = {0} (0x{1:x2}, {2} decimal){10}
					( int ) _chrDelimiter ,					// Format Item  2 = Delimiter character (decimal)                                Delimiter         = {0} (0x{1:x2}, {2} decimal){10}
					_chrGuard ,								// Format Item  3 = Guard character                                              Guard Character   = {3} (0x{4:x2}, {5} decimal){10}
					( int ) _chrGuard ,						// Format Item  4 = Guard character (hexadecimal)                                Guard Character   = {3} (0x{4:x2}, {5} decimal){10}
					( int ) _chrGuard ,						// Format Item  5 = Guard character (decimal)                                    Guard Character   = {3} (0x{4:x2}, {5} decimal){10}
					_enmTrimWhiteSpace ,					// Format Item  6 = White space disposition flag                                 Trim White Space  = {6}{10}
					_enmGuardDisposition ,					// Format item  7 = Guard character disposition flag                             Guard Disposition = {7}{10}
					_fSettingsLocked ,						// Format Item  8 = Property lock state (Locked or Unlocked)                     Lock State        = {8}{10}
					_enmLockMethod ,						// Format Item  9 = Property Locking method (unlocked, explicit, or implicit)    Lock Method       = {9}{10}
					Environment.NewLine						// Format Item 10 = Embedded Newline
				} );
		}	// ToString
		#endregion	// Overridden Root Class Methods
	}	// public abstract class CSVParseEngine
}	// partial namespace WizardWrx.AnyCSV