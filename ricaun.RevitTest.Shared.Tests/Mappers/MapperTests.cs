using NUnit.Framework;
using ricaun.RevitTest.Shared.Mappers;

namespace ricaun.RevitTest.Shared.Tests.Mappers
{
    internal class MapperTests
    {
        public class TestClass
        {
            public string TestProperty { get; set; }
        }

        public class TestDestination
        {
            public string TestProperty { get; set; }
        }

        [Test]
        public void Test_Mapper_NotifyPropertyChanged()
        {
            var source = new TestClass() { TestProperty = "text" };
            var destination = new TestDestination();

            var propertyChangedCount = 0;

            Mapper.NotifyPropertyChanged notify = (propertyName, value) =>
            {
                System.Console.WriteLine($"NotifyPropertyChanged: {propertyName}: {value}");
                Assert.AreEqual("TestProperty", propertyName);
                Assert.AreEqual(source.TestProperty, value);
                propertyChangedCount++;
            };

            Mapper.Map(source, destination, notify);

            Assert.AreEqual(source.TestProperty, destination.TestProperty);

            Mapper.Map(source, destination, notify);

            source.TestProperty = null;

            Mapper.Map(source, destination, notify);

            Assert.AreEqual(source.TestProperty, destination.TestProperty);

            Assert.AreEqual(2, propertyChangedCount);
        }
    }
}