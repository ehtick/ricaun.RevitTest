using NamedPipeWrapper.Json;
using System;

namespace ricaun.RevitTest.Shared
{
    public class TestResponse
    {
        public bool IsBusy { get; set; }
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}