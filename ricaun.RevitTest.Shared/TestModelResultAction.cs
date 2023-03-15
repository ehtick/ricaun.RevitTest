using ricaun.NUnit.Models;
using System;

namespace ricaun.RevitTest.Shared
{
    public class TestModelResultAction : ricaun.NUnit.ITestModelResult
    {
        private readonly Action<TestModel> action;

        public TestModelResultAction(Action<TestModel> action)
        {
            this.action = action;
        }
        public void Result(TestModel testModel)
        {
            this.action?.Invoke(testModel);
        }
    }

}