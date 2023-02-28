using NamedPipeWrapper;
using System;
using System.Diagnostics;

namespace ricaun.RevitTest.Shared
{
    public class PipeTestServer : PipeProcessServer<TestResponse, TestRequest>
    {
        public PipeTestServer() : base(ProcessPipeNameUtils.GetPipeName()) { }
        public PipeTestServer(Process process) : base(process) { }
    }

    public class PipeTestClient : PipeProcessClient<TestResponse, TestRequest>
    {
        public PipeTestClient() : base(ProcessPipeNameUtils.GetPipeName()) { }
        public PipeTestClient(Process process) : base(process) { }
    }
}
