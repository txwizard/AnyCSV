# Introduction to the WizardWrx AnyCSV API

I created the __WizardWrx AnyCSV__ class library to implement the most robust
CSV string parser that I can imagine.

The library was compiled against versions 2.0 of the Microsoft .NET Framework,
and it is compatible with versions from 2.0 up. The classes in this library
define a handful of constants and a combination of static and instance methods
to cover every imaginable scenario that involves CSV-type strings.

Use the __API Documentation__ link in the navigation bar at the top of this page
to display a summary of the classes, along with links to complete documentation
of the public constants, enumerations, and methods exposed by them.

Since I encountered a need to process CSV strings inside a Microsoft Excel
worksheet, this library is exposed to COM, and the build script registers it
with the COM subsystem.  If you prefer or need manual registration, e. g., on
another machine, the release distribution includes a type library, which also
comes with the NuGet package.

# Road Map

So far as I am concerned, this library is complete. Though I expect to maintain
it in terms of bug fixes, no new features are planned.

# Contributing

If you have a fabulous idea for extending this library, please contact me
privately to make your case. If you can convince me, I'll add you as a
contributor.

If you find and fix a bug, please submit a pull requeest.