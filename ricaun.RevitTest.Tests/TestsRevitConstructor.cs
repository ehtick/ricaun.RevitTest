using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;

namespace ricaun.RevitTest.Tests
{
    public class TestsRevitConstructor
    {
        protected readonly Application application;
        protected readonly UIApplication uiapp;
        public TestsRevitConstructor(Application application, UIApplication uiapp)
        {
            this.application = application;
            this.uiapp = uiapp;
        }

        [Test]
        public void RevitTestsDB()
        {
            Assert.IsNotNull(application);
            Assert.IsTrue(application.IsValidObject);
        }
        [Test]
        public void RevitTestsUI()
        {
            Assert.IsNotNull(uiapp);
            Assert.IsTrue(uiapp.IsValidObject);
        }
    }
}