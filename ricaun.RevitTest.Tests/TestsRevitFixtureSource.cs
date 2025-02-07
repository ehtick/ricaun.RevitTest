using Autodesk.Revit.ApplicationServices;
using NUnit.Framework;
using System;

namespace ricaun.RevitTest.Tests
{
    [TestFixtureSource(nameof(FixtureArgs))]
    public class TestsRevitFixtureSource
    {
        public static object[] FixtureArgs = { 1, 2, 3 };
        private readonly int value;
        public TestsRevitFixtureSource(int value)
        {
            this.value = value;
        }

        [Test]
        public void TestValue(Application application)
        {
            Console.WriteLine(value);
            Console.WriteLine(application);
            Assert.IsNotNull(application);
        }
    }

    [TestFixtureSource(typeof(TestsRevitFixtureSource), nameof(TestsRevitFixtureSource.FixtureArgs))]
    public class TestsRevitFixtureSourceAnother
    {
        private readonly int value;
        public TestsRevitFixtureSourceAnother(int value)
        {
            this.value = value;
        }

        [Test]
        public void TestValue(Application application)
        {
            Console.WriteLine(value);
            Console.WriteLine(application);
            Assert.IsNotNull(application);
        }
    }
}