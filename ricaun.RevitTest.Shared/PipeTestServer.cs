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
        public NamedPipeServer<TestRequest, TestResponse> NamedPipe { get; private set; }
        public PipeTestServer()
        {
        }

        public bool Initialize()
        {
            var pipeName = ProcessPipeNameUtils.GetPipeName();
            if (NamedPipeUtils.PipeFileExists(pipeName) == false)
            {
                NamedPipe = new NamedPipeServer<TestRequest, TestResponse>(pipeName);
                NamedPipe.Start();
                NamedPipeDebug();
            }
            return NamedPipe != null;
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
                Debug.WriteLine($"[{connection.Id}] ClientConnected");
            };
            NamedPipe.ClientDisconnected += (connection) =>
            {
                Debug.WriteLine($"[{connection.Id}] ClientDisconnected");
            };
            NamedPipe.ClientMessage += (connection, message) =>
            {
                Debug.WriteLine($"[{connection.Id}] ClientMessage: \t{message}");
            };
        }
    }

    public class PipeTestClient : IPipeTest
    {
        public NamedPipeClient<TestResponse, TestRequest> NamedPipe { get; private set; }
        public PipeTestClient()
        {
        }

        public bool Initialize()
        {
            var pipeName = ProcessPipeNameUtils.GetPipeName();
            NamedPipe = new NamedPipeClient<TestResponse, TestRequest>(pipeName);
            NamedPipe.Start();
            NamedPipeDebug();
            return true;
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
                Debug.WriteLine($"[{connection.Id}] Connected");
            };

            NamedPipe.Disconnected += (connection) =>
            {
                Debug.WriteLine($"[{connection.Id}] Disconnected");
            };

            NamedPipe.ServerMessage += (connection, message) =>
            {
                Debug.WriteLine($"[{connection.Id}] ServerMessage: \t{message}");
            };
        }
    }
}
