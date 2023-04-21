using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using NUnit.Framework;

namespace ricaun.RevitTest.Tests
{
    public class TestsRevitOneTimeSetUp : TestsRevitOneTimeSetUpBase
    {
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

    public abstract class TestsRevitOneTimeSetUpBase
    {
        protected Application application;
        protected UIApplication uiapp;

        [OneTimeSetUp]
        public void OneTimeSetUpBase(Application application, UIApplication uiapp)
        {
            this.application = application;
            this.uiapp = uiapp;
        }
    }
}