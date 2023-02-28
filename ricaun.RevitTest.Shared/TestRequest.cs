using NamedPipeWrapper.Json;
using System;

namespace ricaun.RevitTest.Shared
{
    public class TestRequest
    {
        public int Id { get; set; }
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}