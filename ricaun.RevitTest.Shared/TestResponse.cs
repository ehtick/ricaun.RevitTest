using NamedPipeWrapper.Json;
using ricaun.NUnit.Models;
using System;

namespace ricaun.RevitTest.Shared
{
    public class TestResponse
    {
        public bool IsBusy { get; set; } = true;
        public string Text { get; set; }
        public TestModel Test { get; set; }
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}