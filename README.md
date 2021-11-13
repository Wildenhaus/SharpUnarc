# SharpUnarc
A C# wrapper library for extracting FreeArc archives.

FreeArc is a compression utility developed by Bulat Ziganshin.
It is provided as open-source software under the GNU GPL 2.0 License.

# About
This library allows developers to use FreeArc's Unarc.dll within their applications. It targets .NET Standard 2.0.

Currently, Unarc.dll only supports 32-bit processes. Targeting AnyCPU or x64 will cause exceptions to be thrown.

# Unarc.dll
The supplied version of Unarc.dll is built off the source code of FreeArc 0.67. 

However, one modification has been made:
In FreeArc's Common.cpp, there exists logic to connect to the COM component to display progress on the Windows taskbar. 
This can cause the CLR to deadlock, as it is unable to resolve the proper handles for some applications. 
If you want to use a version of Unarc.dll with this library, you will likely need to remove this code before compiling.
Additionally, if you are experiencing deadlocks with a different version of the DLL, you can debug it by disabling "Just My Code"
in Visual Studio's settings and enabling Native Code Debugging in the project settings.

# Usage
See the test classes for example usage.
