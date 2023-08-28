using NUnit.Framework;
using System;

//[assembly: System.Reflection.AssemblyMetadata("NUnit.Language", "pt")]
//[assembly: System.Reflection.AssemblyMetadata("NUnit.Language", "es")]
//[assembly: System.Reflection.AssemblyMetadata("NUnit.Language", "en")]

namespace ricaun.RevitTest.Tests
{
    public class TestsLanguage
    {
        [Test]
        public void TestLanguage()
        {
            var cultureInfo = System.Globalization.CultureInfo.CurrentUICulture;
            Console.WriteLine(cultureInfo.Name);
        }
    }
}