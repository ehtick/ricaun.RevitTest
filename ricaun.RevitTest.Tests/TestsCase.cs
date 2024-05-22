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

        [TestCase("a")]
        [TestCase("1")]
        [TestCase("_")]
        [TestCase("/")]
        [TestCase("\\")]
        [TestCase("\"")]
        [TestCase("\"\"")]
        //[TestCase("\n")]
        [TestCase(".")]
        [TestCase(",")]
        [TestCase("°C")]
        [TestCase("a\\a")]
        [TestCase("\\a")]
        [TestCase("\\\\")]
        [TestCase("\\\\b")]
        [TestCase(@"C:\help.pdf")]
        public void TestStringCase_IsNot(string value)
        {
            Console.WriteLine(value);
            Assert.IsFalse(string.IsNullOrWhiteSpace(value));
        }

        [TestCase(1.0)]
        [TestCase(2.0)]
        [TestCase(3.0)]
        [TestCase(4.2)]
        [TestCase(5.3)]
        [TestCase(6.4)]
        public void TestDoubleCase(double scale)
        {
            Assert.NotZero(scale);
        }
    }
}
