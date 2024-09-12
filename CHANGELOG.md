# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.4.2] / 2024-09-11
### Features
- Support tests with `TestCaseSource`. (Fix: #55)
### Application
- Update `ricaun.NUnit` to `1.4.0-beta`.
### Tests
- Add `TestsCaseSource` to test `TestCaseSourceAttribute`.
- Add tests with `TestCaseData` class with expected result.

## [1.4.1] / 2024-09-06
### TestAdapter
- Update project reference to `PrivateAssets` to remove reference in the package.

## [1.4.0] / 2024-09-05
### Features
- Support `NUnit.Timeout` option to abort application. (default '10' minutes) (Fix: #51)
- Autoclose `VIEWER MODE` warning dialog box when Revit starts. (Fix: #52)
### Application
- Add `DialogBoxResolver` to cancel dialog box before revit initialization.
### Console
- Change `TimeoutMinutesDefault` to 10 minutes in the `RevitTestUtils`.
- Remove deprecated `wait` option and `RevitDebugUtils`.
- Add `Timeout` option to abort application.
- Fail all tests when `Timeout` happen.
- Update execution/timeout message in the log.
### Command
- Add `RevitTestProcessStart` to run tests with `ProcessStart`.
- Remove deprecated `wait` option.
- Add `Timeout` option to abort application.
### TestAdapter
- Add project `ricaun.RevitTest.Command` with `ricaun.NUnit`
- Remove references `CommandLineParser` and `NUnit` not used.
- Use `RevitTestProcessStartUtils` to start the process.
- Add `NUnit.Timeout` option to timeout the application.

## [1.3.6] / 2024-08-30
### TestAdapter
- Remove log in the `LocalFileExists` in the `ApplicationUtils`.

## [1.3.5] / 2024-07-22 - 2024-08-28
### Features
- Update `Application` images to support `light` and `dark` theme.
### Application
- Update project to build `net47` and `net48`.
- Update `Images` with light and dark theme.
- Update `ricaun.Revit.UI` with the image auto theme based in the `light` and `dark` in the file name.
- Update `Exception` when null.
- Update timestamp in temp folder to hexadecimal.
- Update test folder to temporarty the folder `RevitTest` to reduce the `PathTooLongException`. (Fix #48)
- Update to use `ricaun.Revit.UI.Tasks` library to run async tests.
- Update to use `ricaun.Revit.UI.Busy` library to check if revit is busy.
### Console
- Throw exception when `RevitTestUtils` is empty.
### TestAdapter
- Show Warning when Console fails.
- Fix local file copy to temporary by zip folder.
- Add `TestsFilePathTooLong` to similate long path inside test.
- Update to use `ricaun.Revit.UI.Tasks` library to run async tests.
- Update `ApplicationUtils` check local file exists.
- Update `RevitTestConsole` with `SetCurrentDirectory` using the source test file.

## [1.3.4] / 2024-05-27
### Console
- Update `ricaun.Revit.Installation` to fix bundle not loading when Revit is open. (Fix: #46)
- Update `ricaun.Revit.Installation` with a different `RevitInstallationUtils`.

## [1.3.3] / 2024-05-22
### Fixed
- Fix `TestCase` with the same Guid using `GuidFromString`. (Fix: #43)
- Fix `TestCase` with `.` in the name. (Fix: #44)
### TestAdapter
- Add `GuidFromString` in the `TestCaseUtils`.
- Update `SplitTestName` to check `(` and `"`.
- Update Application for `LocalFileExists`.
### Tests
- Add `TestDoubleCase` with double with `.` in the name.

## [1.3.2] / 2024-05-09
### Fixed
- Fix `ApplicationUtils` to clear temp folder after some minutes. (Fix: #41)
- Fix Net Core tests not stay after rebuild. (Fix: #40)
### TestAdapter
- Fix application and rebuild issue.
- Update `ApplicationUtils` to use `Guid` temp folder.

## [1.3.1] / 2024-04-02
### Features
- Support Revit 2025 to 2017.
### Fixed
- Fix `Installation` Application issue.
- Fix `Timeout` issue with `testsFinishedForceToEnd`.
### Command
- Update `ricaun.Revit.Installation` to `1.1.2`
- Update `debugger` with debug logs.
- Update `Command` with `SemanticVersion`.
### Application
- Fix `AssemblyResolve` in `.NET Core` updated `AppDomainExtension`.
### Shared
- Update `NamedPipeWrapper.Json` to `1.7.0`
### TestAdapter
- Update `TestAdapter` with debug logs.
- Update `TestAdapter` with `PackageTags`
- Update `TestAdapter` with `PackageLicenseFile`
- Add `IsLogDebug` with `Verbosity`.


## [1.3.0] / 2024-03-28
### Features
- Readme information to wiki.
### Application
- Add class `ApplicationValidUser` to validate user.
### Removed
- Remove all `ricaun.Auth` references.

## [1.2.1] / 2023-11-21 - 2024-02-06
### Features
- Support `net8.0-windows` for Revit 2025.
### Application
- Add `EnsureApsUserHaveOpenId` to make sure `OpenId` has data. 
- Disable `EnsureApsUserHaveOpenId`, not working `OpenId` is null after `Refresh`.
- Update `OpenFile` in `Log` with `UseShellExecute=true`.
- Update `net8.0-windows` for Revit 2025.
- Update `csproj`.
- Add `ApplicationPreviewUtils` to enable know if Revit application is preview.
- Enable run without `LoginUserId` when `IsPreviewRelease`.
- Update `ricaun.Revit.UI`
- Update `RibbonUtils` with updated `ricaun.Revit.UI`
- Update images to `ico`.
- Update `ricaun.Auth` to beta - `LicenseAgreement` enable.
- Add `ApsApplicationPipeTest` class.
- Remove `ApsApplicationCheck`
### Shared
- Update `net6.0-windows` framework.
- Update support `net8.0-windows` framework.
### Console
- Update `net8.0-windows` framework.
- Update `csproj`.
- Update `MarshalUtils` in `net7.0`
- Update `RevitTestUtils` with no `hiddenRevitArgument`, use `NUnit.Language` to add extra arguments to open Revit.
- Add `SleepMillisecondsDebuggerAttached` to wait for debugger.
### Build
- Update `net48` and `net8.0-windows` console zip.
### TestAdapter
- Update `TestAdapter` to `net6.0` and `net8.0`
- Select resource console zip for NetFramework and NetCore.
- Update `ResourceConsoleUtils`
- Update `RevitTestConsole` to remove `+` in FileInfo.
- Add `FileVersionInfoUtils` to remove `+` in FileInfo.
- Update to `HttpClient`.
- Update `RevitTestConsole` to use `HttpClient`.
- Update `Log` to show download fail and the `ProductVersion`.
- Update `Download` exception to Warning.
- Add `EncodeParameterArgumentExtension` to fix `[TestCase(@"C:\help.pdf")]` issue. (Fix #36)
### Warning
- Ignore `Newtonsoft.Json` warning in `Application` project.

## [1.2.0] / 2023-10-28
### Features
- Application works offline.
- Application force Revit Login.
### Application
- Update `ricaun.Auth.Aps.UI` to 1.0.4
- Update scope to `OpenId`.
- Update only with `IsLoggedIn` and same `UserId`.

## [1.1.3] / 2023-10-16
### Features
- Application show error in log if fail.
### TestAdapter
- Update `RevitTestConsole` with log application location.
- Update `ApplicationUtils` clear temp folder.
### Application
- Update Log before test Runs.

## [1.1.2] / 2023-10-07
### Features
- `TestAdapter` with `net6` support.
### TestAdapter
- Update `net6` to `EnableDynamicLoading` to fix `System.IO.FileNotFoundException` reference. (Fix: #32)
### Command
- Update `AppUtils.GetInfo` to show framework version (Net Framework or Net Core).
- Update `Run` to show in Log the Exception.
### Console
- Remove `KeyAvailable` in the `RevitTestUtils`. (Fix issue in Rider.)

## [1.1.1] / 2023-10-03
### Features
- `TestDiscoverer` enable log feature.
- `TestAdapter` only execute Application if trusted.
- `TestAdapter` with `net6` support.
### Updated
- Update `ricaun.NUnit` to `1.3.1`
### Command
- Update TargetFramework added `net5`
- Update `JsonExtension` with `System.Text.Json`
### Console
- Update `ricaun.Revit.Installation` to `1.1.0` (Fix: #30)
### TestAdapter
- Update `TestDiscoverer` with `Log` feature
- Update `RevitTestConsole` with `RunTestReadWithLog` feature
- Add `ricaun.Security.WinTrust` package with `PrivateAssets` 
- Add `TrustApplicationUtils` to check if application is trusted
- Add `DelayAfterExit` in `ProcessStart` with value `100`
- Remove not used `XmlBool` 
- Add `JsonService` for net framework and net code.
- Add `TargetFrameworkUtils` to get `TargetFramework` name.
### Fixed
- Fix Folder not exist (Fix: #30)
### Build
- Update to version `1.6.1` enable prerelease with filter
- Update `IBuildCommand` and `IBuildConsole`
- Disable `IPackRelease`

## [1.1.0] / 2023-08-28
### Features
- Support Revit Language 
### Command 
- Update option with RevitLanguage
### Console
- Add `RevitLanguageUtils` to get language argument
### TestAdapter
- Add `Language` support in the NUnit adapter executer
### Tests
- Add Language test for Revit UI

## [1.0.22] / 2023-08-08
### Fixed
- Fix `AssemblyResolveEvent handlers cannot return Assemblies loaded for reflection only.` in version 2021 by not using `ContainNUnit` in each file.
### Application
- Change `TestDirectoryAsync` and add filePattern to filter files and test only the original file.

## [1.0.21] / 2023-08-07
### Fixed
- Fix buzy problem when `TestAsync` is running.
### Application
- Add `IsTestRunning` to prevent buzy to turn off when `TestAsync` is running.
- `IsTestRunning` disable `RevitBusyControlPropertyChanged`
- Ignore `FileNotFoundException` in `TestExecuteUtils`.
### Tests
- Add `TestAsync_Idling_Timeout` to test `IsTestRunning`

## [1.0.20] / 2023-07-28
### Features
- Enable to run async tests outside the Revit Context.
### Application
- Update `TestExecuteUtils`
- Add `CopyFilesUsingZipFolder` and `CopyFilesBackUsingZip` to `TestExecuteUtils`
- Update `ZipExtension` to delete `zip` file after extract
- Clear run tests in `App`
- Update `TestEngineFilter.CancellationTokenTimeOut` to set 1 minute
- Update `TestExecuteUtils` with `ExecuteAsync`
- Add `FileVersionInfoUtils` to configurate `TestAsync` using `AssemblyDescriptionAttribute`
- Add `ConfigurationComments` with `TimeOut` and `TestAsync`
### Tests
- Add `TestRevitTask` tests with `RevitTask`, async, Idling, and `Dialog_API_MacroManager`

## [1.0.19] / 2023-07-26
### Features
- Validate `ApsUser` with `ApsApplicationCheck`
- `RevitTaskService` to run `Tests`
### Application
- Update `ricaun.Revit.Async` to `1.0.4`
- Update `RevitTask` to `RevitTaskService`
- Update `RevitBusyControl` to `RevitBusyService`
- Update to use `TestAssemblyModel` in `TestExecuteUtils`
- Add `ApsResponse` with `isValid`
- Add `ApsApplicationCheck` with `Check`
- Validate the `ApsResponse` before each test using `ApsApplicationCheck`

## [1.0.18] / 2023-07-18
### Features
- Application `zip` downloads with any `exe` file.

## [1.0.17] / 2023-07-17
### Features
- Installed Revit Version not found show error in the Tests.
### Console
- Update `RevitTestUtils` to show error when Revit Version not found.
### TestAdapter
- Add `ITestLoggerExtension` with `DebugOnlyLocal`
- Change `Logger` in the `ExtractZipToDirectory` to `DebugOnlyLocal`

## [1.0.16] / 2023-07-17
### Features
- `TestAdapter` configuration `Application` works with `EnvironmentVariable`.
### TestAdapter
- Update `ValidadeApplication` to work with `EnvironmentVariable`.
- Fix `Metadata` with same key problem.

## [1.0.15] / 2023-07-12
### Features
- Open `ApsView` if not connected and wait for `Login` or close.
### Updated
- Update `ricaun.NUnit` to 1.3.0 (`TestEngine.Fail`)
### Application
- Fail tests with `TestEngine.Fail` when NUnit is not valid.
- Open `ApsView` when not `Login` when tests.

## [1.0.14] / 2023-07-11
### Features
- Update Logger to another server to improve speed.
### Application
- Logger when `Login` with `ApsView`
- Update `ApsLog` with product info.

## [1.0.13] / 2023-06-30
### Features
- Add Aps Loggin and logger test count
### Application
- Add `ApsApplicationLogger`
- Add `ricaun.Auth.Aps.UI` version `1.0.1`
### Console
- Add time delay close and Kill process
### TestAdapter
- Add `OrdinalIgnoreCase` to fix Metadata Mapper

## [1.0.12] / 2023-06-26
### Features
- Add authentication with `ricaun.Auth`
### Application
- Add `ApsApplication` with `ricaun.Auth.Aps.UI`
- Add `TestExceptionUtils` to show `Exception` in the `TextExplorer`

## [1.0.11] / 2023-06-15
### TestAdapter
- Fix Metadata problem not working... From `1.0.10`

## [1.0.10] / 2023-06-14
### TestAdapter
- Change everything to internal to remove possible Reference
- Problem with `DeserializeXml` only work with public classes
- Create new `MapperKey` to work with KeyValue
- Create `XmlUtils` to transform xml to KeyValue
### Application
- Create `RibbonUtils` with custom images for `pass`, `fail`, `skip`, and `wait`.

## [1.0.9] / 2023-06-13
### TestAdapter
- Fix duplicated result by using double `RecordResultTestModel` 
### Application
- Update Icon when running tests

## [1.0.8] / 2023-06-13
### TestAdapter
- Update to show OutputConsole in the Debug (NUnit.Verbosity = 2).
- Update `TestModels`, `TestAssemblyModel` and `TestCaseModel`
- Update `TestAssemblyModel` to record TestCase with the method `RecordResultTestModel`
- Fix Download Application File

## [1.0.7] / 2023-06-12
### Console
- Decouple using interface `IRunTestService`
- Add `Command` project reference
- Clear old `Command` references
### Command
- Command library to run tests with `IRunTestService`
- Create build for package `ricaun.RevitTest.Command`

## [1.0.6] / 2023-05-30
### Updated
- [x] Update `ricaun.NUnit` to 1.2.9 (`MultipleSetUp`)
### Tests
- [x] Add Test `TestsRevitSetUp`
- [x] Add Test `TestsRevitIdling`

## [1.0.5] / 2023-04-25
### Fixed
- [x] Fix Tests with special char, example `°C`
### TestAdapter
- [x] Update `ProcessStart` to default Encoding.
- [x] Update Extensions to internal
### Tests
- [x] Add Test with special char `°C`

## [1.0.4] / 2023-04-21
### Updated
- [x] Update `ricaun.NUnit` to 1.2.8 (`GetTestFullNames` get tests with *)
### Fixed
- [x] Fix Version Revit 2024 no Tests
### Console
- [x] Add Debug Tests in `RevitTestUtils`
- [x] Update `RevitTestUtils` to min and max (2021-2023)
### TestAdapter
- [x] Add Metadata Discovery Tests
- [x] Add `IsSimilarTestName` in `TestCaseUtils` (Make work with *)
### Tests
- [x] Add `TestRevitConstructor`
- [x] Add `TestRevitOneTimeSetUp`

## [1.0.3] / 2023-04-17
### Features
- [x] Update `TestAdapter` to work with '.' in the TestName. ('TestCaseUtils') 
- [x] Update `RevitTest` to work with ',' and '\\' in the TestName.
### Application
- [x] Update `Tests` without Split(',')
### Console
- [x] Update Command `Test` to multiple parameters using `IEnumerable`.
### Shared
- [x] Add Info to `TestRequest`
- [x] Update `TestFilter` to `TestFilters` with `string[]`
### TestAdapter
- [x] Update `ProcessStart` to work with `IEnumerable` arguments, join with ' '.
- [x] Update `RevitTestProcessStart` to work filters `string[]`. 

## [1.0.2] / 2023-04-15
### Features
- [x] Update `ricaun.NUnit` to 1.2.7
### Tests
- [x] TestsCase - TestCase with string and null
### Update
- [x] Update `TestAdapter` to work with `ricaun.NUnit` 1.2.7
- [x] Update `ProcessStart` in `TestAdapter` to work with `"` in the filter.

## [1.0.1] / 2023-04-11
### Features
- [x] Update `ricaun.NUnit` to 1.2.6
### Tests
- [x] TestRevit - Similar Parameter `(IDisposable is UIApplication)`

## [1.0.0] / 2023-02-27 - 2023-04-04
### Features
### Application
- [x] App - RibbonPanel/RibbonItem
- [x] PipeServer - Shared
- [x] UserUtils - IsValid/ISNotValid
- [x] PipeClient - PropertyChanged
- [x] Run Tests - ricaun.NUnit
- [x] Run Tests - Filter Tests
- [x] Zip - Copy Zip Folder
- [x] Zip - Copy back Zip Folder
- [] Zip - Zip max size file
- [x] Log - Debug/Jornal
- [x] Log - FileOpen/JornalOpen
### Console
- [x] Info - icon.ico
- [x] PipeClient - Shared
- [x] PipeClient - PropertyChanged
- [x] RevitInstallation - Get Revit Installed
- [x] CommandLine - Run File Tests
- [x] CommandLine - Get File Tests FullName
- [x] CommandLine - Output to Console/File
- [x] CommandLine - Logger enable/disable
- [x] CommandLine - Select RevitVersion
- [x] CommandLine - Force Open/Close/Wait
- [x] CommandLine - Run Tests with Filter
- [x] CommandLine - DebuggerAttach hidden options (DebuggerUtils with EnvDTE)
- [x] CommandLine - Update Parser (Ignore Unknown Arguments)
### Shared
- [x] PipeServer/Client - Json
- [x] PipeServer/Client - Default is new()
- [x] PipeServer/Client - Mapper.Map (Update Map static)
- [x] TestRequests - PropertyChanged
- [x] TestResponse - PropertyChanged
- [x] TestResponse - Test/Tests/Info
- [x] Fody.PropertyChanged
### TestAdapter
- [x] TestAdapter - Discoverer
- [x] TestAdapter - Executor
- [x] TestAdapter - Logger (AdapterLogger)
- [x] TestAdapter - TestCaseUtils
- [x] TestAdapter - RunSettings (AdapterSettings, XmlExtension, RunSettingModel, XmlBool)
- [x] TestAdapter - RunSettingsModel (Version, Open, Close, Verbosity, Application, Metadata)
- [x] TestAdapter - Custom Application Process (Download Zip)
- [x] TestAdapter - Add Debugger in executor if needed
- [X] TestAdapter - MetadataMapper (AssemblyMetadataAttribute)
- [x] Services - RevitTestConsole
- [x] Services - ProcessStart
- [x] Services - RevitTestProcessStart
- [x] Services - ApplicationUtils
- [x] Test Models
- [x] Package - Icon
### Tests
- [x] TestsRevit
- [x] TestsRevitDocument
- [x] TestsDebugger
- [x] TestsFile
- [x] TestsFolder
- [x] TestsPass
- [x] TestsIgnore
- [x] TestsFail

[vNext]: ../../compare/1.0.0...HEAD
[1.4.2]: ../../compare/1.4.1...1.4.2
[1.4.1]: ../../compare/1.4.0...1.4.1
[1.4.0]: ../../compare/1.3.6...1.4.0
[1.3.6]: ../../compare/1.3.5...1.3.6
[1.3.5]: ../../compare/1.3.4...1.3.5
[1.3.4]: ../../compare/1.3.3...1.3.4
[1.3.3]: ../../compare/1.3.2...1.3.3
[1.3.2]: ../../compare/1.3.1...1.3.2
[1.3.1]: ../../compare/1.3.0...1.3.1
[1.3.0]: ../../compare/1.2.1...1.3.0
[1.2.1]: ../../compare/1.2.0...1.2.1
[1.2.0]: ../../compare/1.1.3...1.2.0
[1.1.3]: ../../compare/1.1.2...1.1.3
[1.1.2]: ../../compare/1.1.1...1.1.2
[1.1.1]: ../../compare/1.1.0...1.1.1
[1.1.0]: ../../compare/1.0.22...1.1.0
[1.0.22]: ../../compare/1.0.21...1.0.22
[1.0.21]: ../../compare/1.0.20...1.0.21
[1.0.20]: ../../compare/1.0.19...1.0.20
[1.0.19]: ../../compare/1.0.18...1.0.19
[1.0.18]: ../../compare/1.0.17...1.0.18
[1.0.17]: ../../compare/1.0.16...1.0.17
[1.0.16]: ../../compare/1.0.15...1.0.16
[1.0.15]: ../../compare/1.0.14...1.0.15
[1.0.14]: ../../compare/1.0.13...1.0.14
[1.0.13]: ../../compare/1.0.12...1.0.13
[1.0.12]: ../../compare/1.0.11...1.0.12
[1.0.11]: ../../compare/1.0.10...1.0.11
[1.0.10]: ../../compare/1.0.9...1.0.10
[1.0.9]: ../../compare/1.0.8...1.0.9
[1.0.8]: ../../compare/1.0.7...1.0.8
[1.0.7]: ../../compare/1.0.6...1.0.7
[1.0.6]: ../../compare/1.0.5...1.0.6
[1.0.5]: ../../compare/1.0.4...1.0.5
[1.0.4]: ../../compare/1.0.3...1.0.4
[1.0.3]: ../../compare/1.0.2...1.0.3
[1.0.2]: ../../compare/1.0.1...1.0.2
[1.0.1]: ../../compare/1.0.0...1.0.1
[1.0.0]: ../../compare/1.0.0