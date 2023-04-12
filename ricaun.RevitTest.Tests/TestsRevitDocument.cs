using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using NUnit.Framework;
using System;

namespace ricaun.RevitTest.Tests
{
    public class TestsRevitDocument
    {
        private readonly Application application;

        public TestsRevitDocument(Application application)
        {
            this.application = application;
        }

        [TestCase(UnitSystem.Metric, ExpectedResult = DisplayUnit.METRIC)]
        [TestCase(UnitSystem.Imperial, ExpectedResult = DisplayUnit.IMPERIAL)]
        public DisplayUnit NewProjectDocument(UnitSystem unitSystem)
        {
            using (var document = application.NewProjectDocument(unitSystem))
            {
                Console.WriteLine($"{document.Title} {document.DisplayUnitSystem}");
                var displayUnit = document.DisplayUnitSystem;
                document.Close(false);
                return displayUnit;
            }
        }

        [Test]
        public void NewProjectDocument_Metric(Application application)
        {
            using (var document = application.NewProjectDocument(UnitSystem.Metric))
            {
                Console.WriteLine($"{document.Title} {document.DisplayUnitSystem}");
                Assert.IsTrue(document.IsValidObject);
                document.Close(false);
            }
        }

        [Test]
        public void NewProjectDocument_Imperial(Application application)
        {
            using (var document = application.NewProjectDocument(UnitSystem.Imperial))
            {
                Console.WriteLine($"{document.Title} {document.DisplayUnitSystem}");
                Assert.IsTrue(document.IsValidObject);
                document.Close(false);
            }
        }
    }
}