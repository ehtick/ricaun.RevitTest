using NamedPipeWrapper.Json;
using System;

namespace ricaun.RevitTest.Shared
{
    public class TestResponse
    {
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}