using NUnit.Framework;
using ricaun.RevitTest.Shared.Mappers;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ricaun.RevitTest.Shared.Tests.Mappers
{
    //<!-- Fody -->
    //<ItemGroup>
    //  <PackageReference Include = "PropertyChanged.Fody" Version="3.*" IncludeAssets="build; compile" PrivateAssets="all" />
    //</ItemGroup>
    //<PropertyGroup>
    //  <WeaverConfiguration>
    //    <Weavers>
    //      <PropertyChanged />
    //    </Weavers>
    //  </WeaverConfiguration>
    //</PropertyGroup>
    /// <summary>
    /// This test need Fody to Work.
    /// </summary>
    internal class MapperPropertyChangedTests
    {
        public class TestClass : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            public string TestProperty { get; set; }
        }

        public class TestDestination : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            public string TestProperty { get; set; }
        }

        [Test]
        public void Test_Destination_PropertyChanged()
        {
            var source = new TestClass() { TestProperty = "text" };
            var destination = new TestDestination();

            var propertyChangedCount = 0;

            destination.PropertyChanged += (s, e) =>
            {
                var propertyName = e.PropertyName;
                var value = destination.TestProperty;
                Console.WriteLine($"NotifyPropertyChanged: {propertyName}: {value}");
                Assert.AreEqual("TestProperty", propertyName);
                Assert.AreEqual(source.TestProperty, destination.TestProperty);
                propertyChangedCount++;
            };

            Mapper.Map(source, destination);

            Assert.AreEqual(source.TestProperty, destination.TestProperty);

            Mapper.Map(source, destination);

            source.TestProperty = null;

            Mapper.Map(source, destination);

            Assert.AreEqual(source.TestProperty, destination.TestProperty);

            Assert.AreEqual(2, propertyChangedCount);
        }
    }
}