using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using NUnit.Framework;

namespace ricaun.RevitTest.Tests
{
    /// <summary>
    /// dotnet test .\ricaun.RevitTest.Tests\bin\Debug\ricaun.RevitTest.Tests.dll -v:detailed -- NUnit.Verbosity=1
    /// dotnet test .\ricaun.RevitTest.Tests\bin\Debug\ricaun.RevitTest.Tests.dll -v:detailed -- NUnit.Version=2021
    /// dotnet test .\ricaun.RevitTest.Tests\bin\Debug\ricaun.RevitTest.Tests.dll --settings:.\ricaun.RevitTest.Tests\bin\Debug\.runsettings -v:detailed
    /// -- NUnit.Version=2021 NUnit.Open=true NUnit.Close=true
    /// </summary>
    public class TestsRevit
    {
        [Test]
        public void TestRevit1(UIApplication uiapp)
        {
            Assert.IsNotNull(uiapp);
            Assert.Pass(uiapp.Application.Username);
        }

        [Test]
        public void TestRevit2(UIControlledApplication application)
        {
            Assert.IsNotNull(application);
            Assert.Pass(application.ControlledApplication.VersionName);
        }

        [Test]
        public void TestRevit3(Application application)
        {
            Assert.IsNotNull(application);
            Assert.Pass(application.VersionNumber);
        }

        [Test]
        public void TestRevit4(ControlledApplication application)
        {
            Assert.IsNotNull(application);
            Assert.Pass(application.VersionBuild);
        }
    }
}