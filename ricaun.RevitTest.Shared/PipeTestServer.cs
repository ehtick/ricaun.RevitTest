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
        public NamedPipeServer<Read, Write> NamedPipe { get; private set; }
        public PipeTestServer()
        {
        }

        public bool Initialize()
        {
            var pipeName = ProcessPipeNameUtils.GetPipeName();
            if (NamedPipeUtils.PipeFileExists(pipeName) == false)
            {
                NamedPipe = new NamedPipeServer<Read, Write>(pipeName);
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
            NamedPipe.ClientConnected += (client) =>
            {
                Debug.WriteLine($"ClientConnected: {client}");
            };
            NamedPipe.ClientDisconnected += (client) =>
            {
                Debug.WriteLine($"ClientDisconnected: {client}");
            };
            NamedPipe.ClientMessage += (client, message) =>
            {
                Debug.WriteLine($"ClientMessage: {client}: \t{message}");
            };
        }
    }

    public class PipeTestClient : IPipeTest
    {
        public NamedPipeClient<Read, Write> NamedPipe { get; private set; }
        public PipeTestClient()
        {
        }

        public bool Initialize()
        {
            var pipeName = ProcessPipeNameUtils.GetPipeName();
            NamedPipe = new NamedPipeClient<Read, Write>(pipeName);
            NamedPipe.Start();
            NamedPipeDebug();
            return NamedPipeUtils.PipeFileExists(pipeName);
        }

        public void Dispose()
        {
            NamedPipe?.Stop();
        }

        [Conditional("DEBUG")]
        private void NamedPipeDebug()
        {
            NamedPipe.ServerMessage += (server, message) =>
            {
                Debug.WriteLine($"ServerMessage: {server}: \t{message}");
            };
        }
    }
}
