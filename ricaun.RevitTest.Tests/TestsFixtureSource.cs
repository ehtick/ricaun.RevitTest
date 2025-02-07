using NUnit.Framework;
using System;

namespace ricaun.RevitTest.Tests
{
    [TestFixtureSource(nameof(FixtureArgs))]
    public class TestsFixtureSource
    {
        public static object[] FixtureArgs = { 1, 2, 3 };
        private readonly int value;
        public TestsFixtureSource(int value)
        {
            this.value = value;
        }

        [Test]
        public void TestValue()
        {
            Console.WriteLine(value);
        }
    }
}
