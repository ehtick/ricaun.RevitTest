using Autodesk.Revit.UI;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

[assembly: System.Reflection.AssemblyMetadata("ricaun.RevitTest.Application.VendorId", "VendorId")]
// [assembly: System.Reflection.AssemblyMetadata("ricaun.RevitTest.Application.AddInId", "11111111-2222-3333-4444-555555555555")]

namespace ricaun.RevitTest.Tests
{
    public class TestsAddinFile
    {
        const string AssemblyName = "ricaun.RevitTest";

        [Test]
        public void CheckVendorIdInAddinFile(UIApplication uiapp)
        {
            var addinFile = GetRevitTestAddinFilePath(uiapp);
            Assert.IsTrue(File.Exists(addinFile), $"RevitTest addin file not found: {addinFile}");

            var hasVendorId = TryGetElementValue(addinFile, "VendorId", out var vendorId);
            Assert.IsTrue(hasVendorId, $"VendorId element not found in addin file: {addinFile}");

            var addinName = uiapp.ActiveAddInId.GetAddInName();
            Console.WriteLine($"Checking VendorId for {addinName}: {vendorId}");

            var expectedVendorId = "VendorId";
            Assert.AreEqual(expectedVendorId, vendorId, $"VendorId value mismatch in addin file: {addinFile}");
        }

        [Test]
        public void CheckAddInIdInAddinFile(UIApplication uiapp)
        {
            var addinFile = GetRevitTestAddinFilePath(uiapp);
            Assert.IsTrue(File.Exists(addinFile), $"RevitTest addin file not found: {addinFile}");

            var hasAddInId = TryGetElementValue(addinFile, "AddInId", out var addInId);
            Assert.IsTrue(hasAddInId, $"AddInId element not found in addin file: {addinFile}");

            var addinName = uiapp.ActiveAddInId.GetAddInName();

            if (addinName.StartsWith(AssemblyName))
            {
                var expectedAddInId = uiapp.ActiveAddInId.GetGUID().ToString();
                Console.WriteLine($"Checking AddInId for {addinName}: {expectedAddInId}");
                Assert.AreEqual(expectedAddInId, addInId, $"AddInId value mismatch in addin file: {addinFile}");
            }
            else
            {
                Assert.Ignore($"Active AddInId does not match expected assembly name: {addinName}");
            }
        }

        private bool TryGetElementValue(string filePath, string elementName, out string value)
        {
            value = null;
            try
            {
                var addinXml = File.ReadAllText(filePath);
                var elementStartTag = $"<{elementName}>";
                var elementEndTag = $"</{elementName}>";
                var startIndex = addinXml.IndexOf(elementStartTag) + elementStartTag.Length;
                var endIndex = addinXml.IndexOf(elementEndTag);
                if (startIndex >= 0 && endIndex > startIndex)
                {
                    value = addinXml.Substring(startIndex, endIndex - startIndex).Trim();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading addin file: {ex}");
            }
            return false;
        }

        private string GetRevitTestAddinFilePath(UIApplication uiapp)
        {
            var externalApplication = GetRevitTestExternalApplication(uiapp);
            var type = externalApplication.GetType();
            var location = type.Assembly.Location;
            return Path.ChangeExtension(location, ".addin");
        }

        private IExternalApplication GetRevitTestExternalApplication(UIApplication uiapp)
        {
            var externalApplications = uiapp.LoadedApplications.OfType<IExternalApplication>();
            var externalApplication = externalApplications.FirstOrDefault(e => e.GetType().Assembly.GetName().Name.StartsWith(AssemblyName));
            return externalApplication;
        }
    }
}