﻿using NUnit.Framework;
using System.Threading;

namespace ricaun.RevitTest.Tests
{
    public class TestsFail
    {
        [Explicit]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        public void Tests(int i)
        {
            Thread.Sleep(10 * i);
            Assert.Fail($"{i}");
        }
    }
}