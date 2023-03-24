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

## License

This project is [licensed](LICENSE) under the [MIT Licence](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](../../stargazers)!