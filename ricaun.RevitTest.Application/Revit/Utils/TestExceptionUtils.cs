using ricaun.NUnit.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ricaun.RevitTest.Application.Revit
{
    public static class TestExceptionUtils
    {
        public static TestAssemblyModel CreateTestAssemblyModelWithException(string fileToTest, string[] testNames, Exception exception)
        {
            var testTypeModel = new TestTypeModel();
            testTypeModel.Tests = new List<TestModel>();

            var testAssemblyModel = new TestAssemblyModel();
            testAssemblyModel.Name = Path.GetFileName(fileToTest);
            testAssemblyModel.FileName = Path.GetFileName(fileToTest);
            testAssemblyModel.Tests = new List<TestTypeModel>() { testTypeModel };

            foreach (var testName in testNames)
            {
                var testModel = new TestModel()
                {
                    FullName = testName,
                    Success = false,
                    Message = exception.Message,
                };

                testTypeModel.Tests.Add(testModel);
            }

            return testAssemblyModel;
        }

    }
}
