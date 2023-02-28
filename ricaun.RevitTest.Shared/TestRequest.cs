using NamedPipeWrapper.Json;
using System;

namespace ricaun.RevitTest.Shared
{
    public class TestRequest
    {
        public int MyProperty { get; set; } = 1;
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}