using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;
using System;

namespace ricaun.RevitTest.Tests
{
    public class TestsCase
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void TestStringCase(string value)
        {
            Assert.IsTrue(string.IsNullOrWhiteSpace(value));
        }

    }
}
