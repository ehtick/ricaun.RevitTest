using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using NUnit.Framework;

namespace ricaun.RevitTest.Tests
{
    public class TestsRevitDocument
    {
        [Test]
        public void NewProjectDocument(Application application)
        {
            using (var document = application.NewProjectDocument(UnitSystem.Metric))
            {
                System.Console.WriteLine(document.Title);
                Assert.IsTrue(document.IsValidObject);
                document.Close(false);
            }
        }

        [Test]
        public void NewProjectDocument_Imperial(Application application)
        {
            using (var document = application.NewProjectDocument(UnitSystem.Imperial))
            {
                System.Console.WriteLine(document.Title);
                Assert.IsTrue(document.IsValidObject);
                document.Close(false);
            }
        }
    }
}