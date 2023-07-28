using Autodesk.Revit.UI;
using NUnit.Framework;

namespace ricaun.RevitTest.Tests
{
    public class TestsRevitIdling : TestsRevitSetUpBaseUI
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            uiapp.Idling += Uiapp_Idling;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            uiapp.Idling -= Uiapp_Idling;
        }

        private void Uiapp_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            TaskDialog.Show("Title", uiapp.Application.VersionBuild);
            uiapp.Idling -= Uiapp_Idling;
        }

        [Test]
        public void RevitTestsUI()
        {
            Assert.IsNotNull(uiapp);
            Assert.IsTrue(uiapp.IsValidObject);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void RevitTests(int number)
        {
            Assert.IsNotNull(uiapp);
            Assert.IsTrue(uiapp.IsValidObject);
            System.Console.WriteLine($"RevitTests: {this.GetHashCode()}");
        }
    }
}