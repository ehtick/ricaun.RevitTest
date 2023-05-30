using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using NUnit.Framework;

namespace ricaun.RevitTest.Tests
{
    public class TestsRevitSetUp : TestsRevitSetUpBaseUI
    {
        [SetUp]
        public void SetUp()
        {
            System.Console.WriteLine("SetUp");
        }
        [SetUp]
        public void SetUp2()
        {
            System.Console.WriteLine("SetUp2");
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
    public abstract class TestsRevitSetUpBaseDB
    {
        protected Application application;

        [OneTimeSetUp]
        public void OneTimeSetUpBaseDB(Application application)
        {
            this.application = application;
        }
    }

    public abstract class TestsRevitSetUpBaseUI : TestsRevitSetUpBaseDB
    {
        protected UIApplication uiapp;

        [OneTimeSetUp]
        public void OneTimeSetUpBaseUI(UIApplication uiapp)
        {
            this.uiapp = uiapp;
        }
    }
}