/*
    ============================================================================

    Namespace:          WizardWrx.AnyCSV

    Class Name:         ICSVParser

    File Name:          ICSVParser.cs

    Synopsis:           This class defines the interface by which I shall expose
						the most robust CSV string parser that I can conceive to
						COM.

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
    2016/09/25 4.0     DAG    Define an interface, so that I can expose the base
                              class to COM.
    ============================================================================
*/

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace WizardWrx.AnyCSV
{
	[ComVisible ( true ) ,
		   Guid ( "89879EAA-AFAF-4B9D-8D2D-8FE3AD396FE7" ) ]
	[ InterfaceType ( ComInterfaceType.InterfaceIsDual ) ]
	interface ICSVParser
	{
		#region Properties
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
		char FieldDelimiter
		{
			get;
			set;
		}   // FieldDelimiter property


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
		char DelimiterGuard
		{
			get;
			set;
		}   // DelimiterGuard property


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
		CSVParseEngine.GuardDisposition GuardCharDisposition
		{
			get;
			set;
		}   // GuardCharDisposition


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
		CSVParseEngine.TrimWhiteSpace WhiteSpaceDisposition
		{
			get;
			set;
		}   // WhiteSpaceDisposition


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
		CSVParseEngine.LockState SettingsLocked
		{
			get;
		}	// SettingsLocked property
		#endregion  // Properties


		#region Methods
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
		void LockSettings ( );


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
		string [ ] Parse ( string pstrAnyCSV );
		#endregion	// Methods
	}	// interface ICSVParser
}	// partial namespace WizardWrx.AnyCSV