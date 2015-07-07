AspNetBackgroundTasks
===

This is a fork of [AspNetBackgroundTasks](https://github.com/StephenCleary/AspNetBackgroundTasks) with the following changes:

* Targets the .NET 4.0 framework
* Replaced Microsoft.Fakes with [Prig](https://github.com/urasandesu/Prig) so unit tests can be ran in any version of Visual Studio.

Running The Tests
===

1. Build the project ( :] )

2. From the NuGet console manager do the following:

  * Navigate to the output dir of the unit tests project (i.e. `cd <rootDit>/src/UnitTests/bin/Debug`) 

  * run the following command: `prig run -process "C:\Users\Administrator\github\AspNetBackgroundTasks\src\packages\NUnit.Runners.2.6.4\tools\nunit-console.exe" -arguments "UnitTests.dll /domain=None /framework=4.0"`
  
  __TODO__: A PR to automate the above would be greatly appreciated! (e.g. gulp,ect) The prig framework seems to have some gotchas when running the pre-build .csproj and unit test commands.