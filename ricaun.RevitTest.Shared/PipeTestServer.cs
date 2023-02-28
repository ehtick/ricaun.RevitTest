using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Shared
{
    public interface IPipeTest : IDisposable
    {
        bool Initialize();
    }

    public class PipeTestServer : IPipeTest
    {
        public TestResponse Response { get; set; }
        public TestRequest Request { get; private set; }
        public NamedPipeServer<TestRequest, TestResponse> NamedPipe { get; private set; }
        public string PipeName { get; }
        public PipeTestServer()
        {
            PipeName = ProcessPipeNameUtils.GetPipeName();
        }

        public bool Initialize()
        {
            if (NamedPipeUtils.PipeFileExists(PipeName) == false)
            {
                NamedPipe = new NamedPipeServer<TestRequest, TestResponse>(PipeName);
                NamedPipe.Start();
                NamedPipe.ClientConnected += NamedPipe_ClientConnected;
                NamedPipe.ClientMessage += NamedPipe_ClientMessage;
                NamedPipeDebug();
            }
            return NamedPipe != null;
        }

        public void SendResponse(Action<TestResponse> response)
        {
            if (Response is null) Response = new TestResponse();
            response(Response);
            NamedPipe?.PushMessage(Response);
        }

        private void NamedPipe_ClientMessage(NamedPipeConnection<TestRequest, TestResponse> connection, TestRequest message)
        {
            Request = message;
        }

        private void NamedPipe_ClientConnected(NamedPipeConnection<TestRequest, TestResponse> connection)
        {
            if (Response != null)
            {
                connection.PushMessage(Response);
                Console.WriteLine($"[{connection.Id}] PushMessage: \t{Response}");
            }
        }

        public void Dispose()
        {
            NamedPipe?.Stop();
        }

        [Conditional("DEBUG")]
        private void NamedPipeDebug()
        {
            NamedPipe.ClientConnected += (connection) =>
            {
                Console.WriteLine($"[{connection.Id}] ClientConnected");
            };
            NamedPipe.ClientDisconnected += (connection) =>
            {
                Console.WriteLine($"[{connection.Id}] ClientDisconnected");
            };
            NamedPipe.ClientMessage += (connection, message) =>
            {
                Console.WriteLine($"[{connection.Id}] ClientMessage: \t{message}");
            };
        }
    }

    public class PipeTestClient : IPipeTest
    {
        public TestRequest Request { get; set; }
        public TestResponse Response { get; private set; }
        public NamedPipeClient<TestResponse, TestRequest> NamedPipe { get; private set; }
        public string PipeName { get; }

        public PipeTestClient()
        {
            PipeName = ProcessPipeNameUtils.GetPipeName();
        }

        public PipeTestClient(Process process)
        {
            PipeName = process.GetPipeName();
        }

        public bool Initialize()
        {
            NamedPipe = new NamedPipeClient<TestResponse, TestRequest>(PipeName);
            NamedPipe.Start();
            NamedPipe.Connected += NamedPipe_Connected;
            NamedPipe.ServerMessage += NamedPipe_ServerMessage;
            NamedPipeDebug();
            return true;
        }

        private void NamedPipe_Connected(NamedPipeConnection<TestResponse, TestRequest> connection)
        {
            if (Request != null)
                connection.PushMessage(Request);
        }

        private void NamedPipe_ServerMessage(NamedPipeConnection<TestResponse, TestRequest> connection, TestResponse message)
        {
            Response = message;
        }

        public void Dispose()
        {
            NamedPipe?.Stop();
        }

        [Conditional("DEBUG")]
        private void NamedPipeDebug()
        {
            NamedPipe.Connected += (connection) =>
            {
                Console.WriteLine($"[{connection.Id}] Connected");
            };

            NamedPipe.Disconnected += (connection) =>
            {
                Console.WriteLine($"[{connection.Id}] Disconnected");
            };

            NamedPipe.ServerMessage += (connection, message) =>
            {
                Console.WriteLine($"[{connection.Id}] ServerMessage: \t{message}");
            };
        }
    }
}
