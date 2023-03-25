using NamedPipeWrapper;
using System;
using System.Diagnostics;

namespace ricaun.RevitTest.Shared
{
    public class PipeProcessServer<TServer, TClient> : IDisposable
        where TServer : class, new()
        where TClient : class, new()
    {
        public TServer ServerMessage { get; internal set; } = new TServer();
        public TClient ClientMessage { get; internal set; } = new TClient();
        public string PipeName => pipeName;
        public NamedPipeServer<TClient, TServer> NamedPipe => namedPipe;

        private string pipeName;
        private NamedPipeServer<TClient, TServer> namedPipe;
        public PipeProcessServer(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public bool Initialize()
        {
            if (NamedPipeUtils.PipeFileExists(pipeName) == false)
            {
                namedPipe = new NamedPipeServer<TClient, TServer>(pipeName);
                namedPipe.ClientConnected += OnClientConnected;
                namedPipe.ClientMessage += OnClientMessage;
                namedPipe.NamedPipeDebug();
                namedPipe.Start();
            }
            return namedPipe != null;
        }

        public void Update(Action<TServer> response)
        {
            if (ServerMessage is null) ServerMessage = new TServer();
            response(ServerMessage);
            NamedPipe?.PushMessage(ServerMessage);
        }

        private void OnClientConnected(NamedPipeConnection<TClient, TServer> connection)
        {
            if (ServerMessage != null)
            {
                connection.PushMessage(ServerMessage);
            }
        }

        private void OnClientMessage(NamedPipeConnection<TClient, TServer> connection, TClient message)
        {
            Mappers.Mapper.Map(message, ClientMessage);
        }

        public void Dispose()
        {
            namedPipe?.Stop();
        }
    }
}
