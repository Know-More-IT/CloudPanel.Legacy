using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CloudPanel Scheduler")]
[assembly: AssemblyDescription("Schedules and runs tasks at specific times")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Know More IT")]
[assembly: AssemblyProduct("CloudPanel Scheduler")]
[assembly: AssemblyCopyright("Copyright ©  2014 Know More IT")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Logging utility from log4net
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "App.Config", Watch = true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7be0021a-30e7-4fa5-80f5-c7243d687314")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.2.1.0")]
[assembly: AssemblyFileVersion("1.2.1.0")]
