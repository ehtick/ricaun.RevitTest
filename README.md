# ricaun.RevitTest

ricaun.RevitTest is a Framework to execute NUnit tests using Visual Studio.

[![Revit 2017](https://img.shields.io/badge/Revit-2017+-blue.svg)](../..)
[![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)](../..)
[![Nuke](https://img.shields.io/badge/Nuke-Build-blue)](https://nuke.build/)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build](../../actions/workflows/Build.yml/badge.svg)](../../actions)

## Installation

* Download and install [ricaun.RevitTest.Console.exe](../../releases/latest/download/ricaun.RevitTest.Console.zip)

## CommandLine

```bash
.\ricaun.RevitTest.Console.exe --help
.\ricaun.RevitTest.Console.exe --version
```

### Read
```bash
.\ricaun.RevitTest.Console.exe --file "C:\Users\ricau\source\repos\TestProject.Tests\TestProject.Tests\bin\Debug\TestProject.Tests.dll" --read
.\ricaun.RevitTest.Console.exe --file "C:\Users\ricau\source\repos\TestProject.Tests\TestProject.Tests\bin\Debug\TestProject.Tests.dll" --read --output "output.json"
```

### Test
```bash
.\ricaun.RevitTest.Console.exe --file "C:\Users\ricau\source\repos\TestProject.Tests\TestProject.Tests\bin\Debug\TestProject.Tests.dll" --output "console"
.\ricaun.RevitTest.Console.exe --file "C:\Users\ricau\source\repos\TestProject.Tests\TestProject.Tests\bin\Debug\TestProject.Tests.dll" -v 2021 -o "console" --close
```

```
.\ricaun.RevitTest.Console\bin\Debug\ricaun.RevitTest.Console.exe --file "D:\Users\ricau\source\repos\RevitTest0\RevitTest0\bin\Debug\RevitTest0.dll" -v 2021 -o "console"
```

## Tests
### PackageReference 

* `ricaun.RevitTest.TestAdapter`
* `NUnit` need to be 3.13.3
* `Microsoft.NET.Test.Sdk` works with 17.3.0

### `dotnet test`
```bash
dotnet test ricaun.RevitTest.Tests.dll -- NUnit.RevitVersion=2021 NUnit.RevitOpen=true NUnit.RevitClose=true
```
```bash
dotnet test ricaun.RevitTest.Tests.dll --settings:.runsettings
```
```bash
dotnet test ricaun.RevitTest.Tests.dll --settings:.runsettings -- NUnit.RevitVersion=2023
```

### `.runsettings`
.csproj
```xml
  <!--.runsettings-->
  <PropertyGroup>
    <RunSettingsFilePath>$(MSBuildProjectDirectory)\.runsettings</RunSettingsFilePath>
  </PropertyGroup>
  <ItemGroup>
    <None Update=".runsettings">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
```
.runsettings
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
	<NUnit>
		<RevitVersion>2021</RevitVersion>
		<RevitOpen>true</RevitOpen>
		<RevitClose>true</RevitClose>
	</NUnit>
</RunSettings>
```

## License

This project is [licensed](LICENSE) under the [MIT Licence](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](../../stargazers)!