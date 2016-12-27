                                 AnyCSV ReadMe

================================================================================
2016-12-27 00:41:36 is the publication date of COM-visible version 4.0.
2016-06-10 01:13:49 is the initial public release date.
================================================================================

AnyCSV is the definitive CSV parsing engine; so far as I know, there isn't a CSV
string that it can't handle, including the unusually formatted strings found in
X.509 digital certificates. Unlike every CSV parser that I considered before
concluding that the world really does need Yet Another CSV Parser, this library
accepts guard characters anywhere in a string; if a pair guards a delimiter,
it guards a delimiter, even if the guarded delimiter is the only character
between the opening and closing guard characters. The result is a textbook
example of Separation of Concerns; this class parese strings, period.
Reading them is the responsibility of other code; I see no point in deciding for
you how you may acquire them.

This project includes one single-purpose class library, WizardWrx.AnyCSV.dll,
and its unit test program, AnyCSVTestStand.exe. The library defines namespace
WizardWrx.AnyCSV, which contains one class, Parser, which defines two methods,
Parse, its reason for being, and LockSettings, a helper method intended for use
in multithreaded projects.

WizardWrx.AnyCSV.dll is a freestanding library; apart from a working Microsoft
.NET Framework, it has zero dependencies.

The Parse method can be called as an instance method on a class that keeps its
settings (such as delimiter, guard character, etc) or as a static method that
passes all of that in arguments. The static method is ideal for one-off parsing,
such as the fields in a X.509 digital certificate, while the instance method is
best suited to use within a loop that processes the records in a file.

To maximize compatibility with client code, the library targets version 2.0 of
the Microsoft .NET Framework, enabling it to support projects that target that
version, or any later version, of the framework. Since its implementation needs
only core features of the Base Class Library, I have yet to discover an issue in
using it with any of the newer frameworks.

The class belongs to the WizardWrx namespace, which I created to organize the
helper libraries that I use in virtually every production assembly, regardless
of what framework version is its target, and whether its surface is a Windows
console, the Windows desktop, or the ASP.NET Web server. To date, I have used
classes and methods in these libraries in all three environments. The dedicated
namespace pretty much eliminates name collisions as an issue; at the very least,
a name can be qualified to disambiguate it.

The next several sections cover special considerations of which you must be
aware if you incorporate this package into your code as is or if you want to
modify it.

--------------------------
Unmanaged Native Code DLLs
--------------------------

Though WizardWrx.AnyCSV.dll is freestanding, unit test program AnyCSVTestStand
is not. There are a number of native code dynamic link libraries in the output
directories. Only two, P6CUtilLib1.dll and WWConAid.dll, are directly invoked
by the unit test program.

Though the routines in those two libraries that are used, via Platform/Invoke,
use a handful of routines from the other libraries, most are there to satisfy
the loader, which expects to find every DLL mentioned in the Import Address
Table, regardless of whether that routine is actually invoked. After all, the
loader has no way to know that you intend to use just one of possibly dozens or
hundreds of exported routines.

All of these DLLs are my own work, and were created using the Microsoft Visual
Studio compilers and tools. The newer libraries were built with Visual Studio
2013, and link to MSVCR120.dll. The rest were built with Visual Studio 6, and
link to MSVCRT.dll. Although I made significant use of the CRT libraries, even
the routines that write to streams, no resources created by one version of the
CRT DLL is ever processed by the other, and I have run these routines in dozens
of applications, many of which run daily, for over two years, without incident.

--------------------------
Required External Tools
--------------------------

The pre and post build tesks and the test scripts found in the /scripts
directory use a number of tools that I have developed over many years. Since
they live in a directory that is on the PATH list on my machine, they are "just
there" when I need them, and I seldom give them a second thought. To simplify
matters for anybody who wants to run the test scripts or build the project, they
are in DAGDevTOOLS.ZIP, which can be extracted into any directory that happens
to be on your PATH list. None of them requires installation, none of the DLLs is
registered for COM, and none of them or their DLLs use the Windows Registry.

A few use MSVCR120.dll, which is not included, but you probably have it if you
have a compatible version of Microsoft Visual Studio. The rest use MSVCRT.DLL,
which ships with Microsoft Windows.

Rather than deposit a copy of the tool kit in each repository, and incur a very
significant maintenance burden, they have their own repository, at the following
URL.

     https://github.com//txwizard/DAGDevTOOLS

Whereas this repository has a three-clause BSD license, the tool kit has a
freeware license. Although I anticipate eventually putting most, if not all, of
the binary code in the tool kit into open source repositories, due to the way
their source code is organized, making usable repositories is nontrivial, and
must wait for another day. Meanwhile, the shell scripts shall remain completely
free to use and adapt.

--------------------------
COM Interop
--------------------------

Beginning with version 4.0, this library is COM visible, and the project
configuration includes instructions to the build engine to register the
assembly for COM interop. As was so with the initial work, the addtion of COM
interoperability meet a pressing need for a robust CSV string parsing engine
that could run in a Microsoft Excel application.

The idisyncrhatic way that VBA processes inidividual characters is reflected in
the COM interface, and the enumerated types that expose the most popular guard
and delimiter characters are the best way to set your object properties from
your COM client.

--------------------------
Internal Documentation
--------------------------

The source code includes comprehenisve technical documentation, including XML to
generate IntelliSense help, from which the build engine generates XML documents,
which are included herein. Argument names follow Hungarian notation, to make the
type immediately evident in most cases. A lower case "p" precedes a type prefix,
to differentiate arguments from local variables, followed by a lower case "a" to
designate arguments that are arrays. Object variables have an initial underscore
and static variables begin with "s_"; this naming scheme makes variable scope
crystal clear.

The classes are thoroughly cross referenced, and many properties and methods
have working links to relevant MSDN pages.
================================================================================
