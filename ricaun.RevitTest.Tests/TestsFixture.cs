using NUnit.Framework;
using System;

namespace ricaun.RevitTest.Tests
{
    [TestFixture(1)]
    [TestFixture(2)]
    [TestFixture(3)]
    public class TestsFixture
    {
        private readonly int value;
        public TestsFixture(int value)
        {
            this.value = value;
        }

        [Test]
        public void TestValue()
        {
            Console.WriteLine(value);
        }
    }

    [TestFixture("this")]
    [TestFixture("is")]
    [TestFixture("string")]
    public class TestsFixtureString
    {
        private readonly string value;
        public TestsFixtureString(string value)
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
