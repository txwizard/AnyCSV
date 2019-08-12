# WizardWrx AnyCSV ReadMe

AnyCSV is the definitive CSV parsing engine; so far as I know, there isn't a CSV
string that it can't handle, including the unusually formatted strings found in
X.509 digital certificates. Unlike every CSV parser that I considered before
concluding that the world needs Yet Another CSV Parser, this library accepts
guard characters anywhere in a string; if a pair guards a delimiter, it guards a
delimiter, even if the guarded delimiter is the only character between the
opening and closing guard characters. The result is a textbook example of
Separation of Concerns; this class parses strings, period. Reading them is the
responsibility of other code; I see no point in deciding for you how to acquire
them.

## News Flash

2019/08/11 00:41:48 - A few days ago, it came to my attention that COM interop
was broken in the published version of the library. Prior to the release of
version 7.2, I tested the COM Interop interface in the VBA code of the Microsoft
Excel add-in that makes it accessible to every Excel workbook in my possession.
The macro-enabled test workbook, `P6CXLSLib4_Dev.XLSM`, is included in the
`/scripts` directory, along with two Windows NT command scripts,
`AnyCSV_Demo_Debug.CMD` and `AnyCSV_Demo_Release.CMD`, that run the unit test
program, `AnyCSVTestStand.exe`, from a File Explorer window. The test program
has a built-in stop, so that you can review or save its output.

## User Documentation and Binary Availability

Complete documentation is available online at
[https://txwizard.github.io/AnyCSV](https://txwizard.github.io/AnyCSV).

The binary version of this library is available as a NuGet package from
[https://www.nuget.org/packages/WizardWrx.AnyCSV/](https://www.nuget.org/packages/WizardWrx.AnyCSV/).

## Library Orgamization

This project includes one single-purpose class library, `WizardWrx.AnyCSV.dll`,
and its unit test program, `AnyCSVTestStand.exe`. The library defines namespace
`WizardWrx.AnyCSV`, which contains one class, `Parser`, which defines two methods,
`Parse`, its reason for being, and `LockSettings`, a helper method intended for use
in multithreaded projects.

`WizardWrx.AnyCSV.dll` is a freestanding library; apart from a working Microsoft
.NET Framework, version 3.5 Client Profile or better, it has zero dependencies.

The `Parse` method can be called as an instance method on a class that keeps its
settings (such as delimiter, guard character, etc) or as a static method that
passes everything in arguments. The static method is ideal for one-off parsing,
such as the fields in a X.509 digital certificate, while the instance method is
better suited to use within a loop that processes the records in a file.

## Basic Usage

Everything revolves around the static `Parse` method, implemented by abstract base
class `CSVParseEngine`, which exists as five overloads.

1) `Parse ( string pstrAnyCSV , char pchrDelimiter , char pchrProtector , GuardDisposition penmGuardDisposition , TrimWhiteSpace penmTrimWhiteSpace )` explicitly specifies all four parameters.

2) `Parse ( string pstrAnyCSV , char pchrDelimiter , char pchrProtector , GuardDisposition penmGuardDisposition                                     )` defaults `penmTrimWhiteSpace` to `TrimWhiteSpace.Leave`.

3) `Parse ( string pstrAnyCSV , char pchrDelimiter , char pchrProtector                                                                             )` defaults `penmGuardDisposition` to `GuardDisposition.Strip` and `penmTrimWhiteSpace` to `TrimWhiteSpace.Leave`.

4) `Parse ( string pstrAnyCSV , char pchrDelimiter                                                                                                  )` defaults `pchrProtector` to a double quotation mark, `penmGuardDisposition` to `GuardDisposition.Strip` and `penmTrimWhiteSpace` to `TrimWhiteSpace.Leave`.

5) `Parse ( string pstrAnyCSV                                                                                                                       )` defaults `pchrDelimiter` to a comma, `pchrProtector` to a double quotation mark, `penmGuardDisposition` to `GuardDisposition.Strip` and `penmTrimWhiteSpace` to `TrimWhiteSpace.Leave`.

To simplify specifying the `pchrDelimiter` and `pchrProtector` parameters, the base
class exposes the constants listed in the following table.

|Name            |Character |Hex |Dec.|
|----------------|----------|----|----|
|BACK_QUOTE      |\`        |0x60| 96 |
|CARAT           |^         |0x5e| 94 |
|CARRIAGE_RETURN |\r        |0x0d| 13 |
|COMMA           |,         |0x2c| 44 |
|DOUBLE_QUOTE    |\"        |0x22| 34 |
|LINE_FEED       |\n        |0x0a| 10 |
|SINGLE_QUOTE    |\'        |0x27| 39 |
|SPACE           |          |0x20| 32 |
|TAB             |          |0x09|  9 |
|VERTICAL_BAR    |\|        |0x7c|124 |

Convenience enumerations can be used to set the delimiter and guard characters
when constructing instances of the `Parser` class. The following table lists
the enumeration values and maps them to the applicable parameter and the
defined constants, where applicable.

|Parse Parameter|Constructor Parameter|Enumeration Name |Symbol         |Value  |Equivalent Constant|Comment                                                                                                                                            |
|---------------|---------------------|-----------------|---------------|-------|-------------------|---------------------------------------------------------------------------------------------------------------------------------------------------|
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |None           |0      |                   |DelimiterChar is uninitialized.                                                                                                                    |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |Carat          |1      |CARAT              |Specify a CARAT (^) as the delimiter.                                                                                                              |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |CarriageReturn |2      |CARRIAGE_RETURN    |Specify a Carriage Return (CR) control character as the delimiter.                                                                                 |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |Comma          |3      |COMMA              |Specify a comma as the delimiter.                                                                                                                  |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |LineFeed       |4      |LINE_FEED          |Specify a Line Feed (LF) control character as the delimiter.                                                                                       |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |Space          |5      |SPACE              |Specify a space character as the delimiter.                                                                                                        |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |Tab            |6      |TAB                |Specify a Tab control character as the delimiter.                                                                                                  |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |VerticalBar    |7      |VERTICAL_BAR       |Specify a Vertical Bar ("|") character as the delimiter.                                                                                           |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |VerticalBar    |7      |VERTICAL_BAR       |Specify a Vertical Bar ("|") character as the delimiter.                                                                                           |
|pchrDelimiter  |penmDelimiter        |DelimiterChar    |Other          |8      |                   |Infrastructure: The delimiter is something besides the enumerated choices. It is documented because it may appear in the list returned by ToString.|
|pchrProtector  |penmProtector        |GuardChar        |None           |0      |                   |GuardChar is uninitialized.                                                                                                                        |
|pchrProtector  |penmProtector        |GuardChar        |BackQuote      |1      |BACK_QUOTE         |Specify a backwards quotation as the protector of delimiters.                                                                                      |
|pchrProtector  |penmProtector        |GuardChar        |DoubleQuote    |2      |DOUBLE_QUOTE       |Specify a double quotation mark as the protector of delimiters.                                                                                    |
|pchrProtector  |penmProtector        |GuardChar        |SingleQuote    |3      |SINGLE_QUOTE       |Specify a single quotation mark as the protector of delimiters.                                                                                    |
|pchrProtector  |penmProtector        |GuardChar        |Other          |4      |                   |Infrastructure: The guard is something besides the enumerated choices. It is documented because it may appear in the list returned by ToString.    |

The `Parser` class provides nine constructors, a default and eight overrides,
that offer a variety of ways to specify all but the first parameter,
`pstrAnyCSV`, of its `Parse` method.

1) `Parser ( )` sets all four optional properties to their defaults: `pchrDelimiter` to a comma, `pchrProtector` to a double quotation mark, `penmGuardDisposition` to `GuardDisposition.Strip` and `penmTrimWhiteSpace` to `TrimWhiteSpace.Leave`.

2) `Parser ( char pchrDelimiter )` overrides the default delimiter character, while setting `pchrProtector` to a double quotation mark, `penmGuardDisposition` to `GuardDisposition.Strip` and `penmTrimWhiteSpace` to `TrimWhiteSpace.Leave`.

3) `Parser ( DelimiterChar penmDelimiter )` uses the `DelimiterChar` enumeration to specify delimiter character `pchrDelimiter`, while setting `pchrProtector` to a double quotation mark, `penmGuardDisposition` to `GuardDisposition.Strip` and `penmTrimWhiteSpace` to `TrimWhiteSpace.Leave`.

4) `Parser ( GuardChar penmProtector )` uses the `GuardChar` enumeration to override the `pchrProtector` paramter, while everything else gets its default value: `pchrDelimiter` is a comma, `penmGuardDisposition` is `GuardDisposition.Strip`, and `penmTrimWhiteSpace` is `TrimWhiteSpace.Leave`.

5) `Parser ( char pchrDelimiter , char pchrProtector )` overrides the default delimiter and guard characters by specifying C# char variable values, while `penmGuardDisposition` is set to `GuardDisposition.Strip` and `penmTrimWhiteSpace` is set to `TrimWhiteSpace.Leave`.

6) `Parser ( DelimiterChar penmDelimiter , GuardChar penmProtector )` uses the `DelimiterChar` enumeration to override the default delimiter and the `GuardChar` enumeration to override the default guard character, while `penmGuardDisposition` is set to `GuardDisposition.Strip` and `penmTrimWhiteSpace` is set to `TrimWhiteSpace.Leave`.

7) `Parser ( DelimiterChar penmDelimiter , GuardChar penmProtector , GuardDisposition penmGuardDisposition )` uses the `DelimiterChar` enumeration to override the default delimiter, the `GuardChar` enumeration to override the default guard character, and the `GuardDisposition` enumeration to override the `penmGuardDisposition` parameter, leaving `penmTrimWhiteSpace` set to `TrimWhiteSpace.Leave`.

8) `Parser ( DelimiterChar penmDelimiter , GuardChar penmProtector , TrimWhiteSpace penmTrimWhiteSpace )` uses the `DelimiterChar` enumeration to override the default delimiter, the `GuardChar` enumeration to override the default guard character, and the `TrimWhiteSpace` enumeration to override the `penmTrimWhiteSpace` parameter, leaving `penmGuardDisposition` set to `GuardDisposition.Strip`.

9) `Parser ( DelimiterChar penmDelimiter , GuardChar penmProtector , GuardDisposition penmGuardDisposition , TrimWhiteSpace penmTrimWhiteSpace )`uses the `DelimiterChar` enumeration to override the default delimiter, the `GuardChar` enumeration to override the default guard character, the `GuardDisposition` enumeration to set the `penmGuardDisposition` parameter, and the `TrimWhiteSpace` enumeration to override the `penmTrimWhiteSpace` parameter.

Since everything else is governed by properties, `Parser` instances have only the simplest `Parse` method, which takes the string to be parsed, returning an array of substrings.

## Compatibility

To maximize compatibility with client code, the library targets version 3.5,
Client Profile, of the Microsoft .NET Framework, enabling it to support projects
that target that version, or any later version, of the framework. Since its
implementation needs only core features of the Base Class Library, I have yet to
discover an issue in using it with any of the newer frameworks.

The class belongs to the `WizardWrx` namespace, which I created to organize the
helper libraries that I use in virtually every production assembly, regardless
of what framework version is its target, and whether its surface is a Windows
console, the Windows desktop, or the ASP.NET Web server. To date, I have used
classes and methods in these libraries in all three environments. The dedicated
namespace pretty much eliminates name collisions as an issue; at the very least,
a common name, such as `Properties`, can be qualified to disambiguate it.

The next several sections cover special considerations of which you must be
aware if you incorporate this package into your code as is or if you want to
modify it.

## Helper Libraries for the Unit Test/Demonstration Program

Though `WizardWrx.AnyCSV.dll` is freestanding, unit test program
`AnyCSVTestStand.exe` is not. The current version uses 100% managed versions of
the helper classes. Earlier versions of the test program depended on a couple of
unmanaged DLLs that have since been replaced with managed code.

## COM Interop

Beginning with version 4.0, this library is COM visible, and the project
configuration includes instructions to the build engine to register the
assembly for COM interop. As was so with the initial work, the addtion of COM
interoperability met a pressing need for a robust CSV string parsing engine
that could run in a Microsoft Excel application. _Only the Release build is
automatically registered._ However, since registration requires elevated
privileges, __you must build for release with Visual Studio running elevated.__

The idisyncrhatic way that VBA processes inidividual characters is reflected in
the COM interface, and the enumerated types that expose the most popular guard
and delimiter characters are the best way to set your object properties from
your COM client.

Beginning with version 7.2, this library includes `P6CXLSLib4_Dev.XLSM` as proof
that the COM interface works. Functions defined in this macro-enambed Excel file
make the capabilities of the library available through a set of custom worksheet
functions. Saving this file as an Excel add-in puts them, along with many more
custom functions, at your fingertips. The VBA project embedded in this file is
self-signed with a long-lived signature, and the project is protected against
accidental changes; the protection password is __TheCodeProject__.

Alongside `P6CXLSLib4_Dev.XLSM`, which lives in the `/scripts` directory, is
`Dependencies.7z`, which includes  another add-in, `P6CXLSLib3.XLAM`, upon which
`P6CXLSLib4_Dev.XLSM` depends, along with several other DLLs that it imports.
This add-in exposes even more custom worksheet functions.

Many of the custom worksheet functions in both add-in libraries rely upon one or
more custom native (C and C++) dynamic-link libraries, all of which are listed
and briefely described in the following table.

|File Name                           |Description                                                                                                                                                                   |
|------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|HMAC_SHAx.dll                       |HMAC SHAx (Secure Hash Algorithm) functions with strengths to 512 bits.                                                                                                       |
|MD5Digest.dll                       |MD5 (RSA Message Digest 5) hash algorithm, included for backward compatibility, since MDF digests remain fairly common)                                                       |
|P6CStringLib1.dll                   |Assorted string mainpulation functions, mostly used as infrastructure by other libraries                                                                                      |
|P6CUtilLib1.dll                     |Assorted utility functions that leverage features of the Windows API                                                                                                          |
|P6CXLSLib3.XLAM                     |The Excel add-in function library upon which `P6CXLSLib4_Dev.XLSM` relies                                                                                                     |
|P6VersionInfo.dll                   |Wrapper around Windows API library Version.dll, used to extract version information from program files                                                                        |
|SafeMemCpy.dll                      |Safe, in the sense of being resistant to buffer overflows, string copying routines                                                                                            |
|WizardWrx.AssemblyPropertyViewer.dll|Managed routines exposed to COM for extracting details from .NET Assembly files - IMPORTANT: This library must be registered!                                                 |
|WizardWrx.AssemblyPropertyViewer.pdb|Debug symbols for `WizardWrx.AssemblyPropertyViewer.dll`                                                                                                                      |
|WizardWrx.AssemblyPropertyViewer.tlb|COM type library for for `WizardWrx.AssemblyPropertyViewer.dll`                                                                                                               |
|WizardWrx.SharedUtl2.dll            |Managed static Infrastructure methods used by `WizardWrx.AssemblyPropertyViewer.dll` - Note: This library is no longer actively maintained, but it has been stable since 2012.|
|WizardWrx.SharedUtl2.pdb            |Debug symbols for `WizardWrx.SharedUtl2.dll`                                                                                                                                  |
|WizardWrx.SharedUtl2.xml            |Technical documentation for `WizardWrx.SharedUtl2.dll`                                                                                                                        |
|WWDatelib.dll                       |Date formatting, Gregorian Leap Year evaluation, and other routines used by functions exposed by `P6VersionInfo.dll`, though invisible to VBA                                 |
|WWKernelLibWrapper.dll              |Infrastructure functions for managing heaps and other tasks, used by virtually everything else in this collection                                                             |

## Internal Documentation

The source code includes comprehenisve technical documentation, including XML to
generate IntelliSense help, from which the build engine generates XML documents,
which are included herein. Argument names follow Hungarian notation, to make the
type immediately evident in most cases. A lower case `p` precedes a type prefix,
to differentiate arguments from local variables, followed by a lower case `a` to
designate arguments that are arrays. Object variables have an initial underscore
and static variables begin with `s_`; this naming scheme makes variable scope
crystal clear.

The classes are thoroughly cross referenced, and many properties and methods
have working links to relevant MSDN pages.

## Required External Tools

The pre and post build tesks and the test scripts found in the `/scripts`
directory use a number of tools that I have developed over many years. Since
they live in a directory that is on the __PATH__ list on my machine, they are "just
there" when I need them, and I seldom give them a second thought. To simplify
matters for anybody who wants to run the test scripts or build the project, they
are in `DAGDevTOOLS.ZIP`, which can be extracted into any directory that happens
to be on your `PATH` list. None of them requires installation, none of the DLLs is
registered for COM, and none of them or their DLLs use the Windows Registry,
although some can read keys and values from it.

A few use `MSVCR120.dll`, which is not included, but you probably have it if you
have a compatible version of Microsoft Visual Studio. The rest use `MSVCRT.DLL`,
which ships with Microsoft Windows, and has since all but the RTM release of
Windows 95.

Rather than deposit a copy of the tool kit in each repository, and incur a very
significant maintenance burden, they have their own repository, at
[https://github.com//txwizard/DAGDevTOOLS](https://github.com//txwizard/DAGDevTOOLS).

Whereas this repository has a three-clause BSD license, the tool kit has a
freeware license. Although I anticipate eventually putting most, if not all, of
the binary code in the tool kit into open source repositories, due to the way
their source code is organized, making usable repositories is nontrivial, and
must wait for another day. Meanwhile, the shell scripts shall remain completely
free to use and adapt.

## Revision History

- 2016/06/10 Initial public release, including the NuGet package
- 2016/12/27 COM-visible version 4.0
- 2018/09/19 Initial publication of DocFX documentation
- 2018/12/01 Conversion of ReadMe from plain ASCII text to Markdown, fix display issues with DocFX documentation
- 2019/07/03 Correct string returned by ToString method on parser instances, and add basic usage help and a link to the NuGet package to this ReadMe file.
- 2019/08/11 Fix broken COM interface, and incorporate the Excel workbook that demonstrates that it works.