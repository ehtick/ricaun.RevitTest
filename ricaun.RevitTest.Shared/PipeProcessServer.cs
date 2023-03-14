using NamedPipeWrapper;
using System;
using System.Diagnostics;

namespace ricaun.RevitTest.Shared
{
    public class PipeProcessServer<TServer, TClient> : IDisposable
        where TServer : class, new()
        where TClient : class, new()
    {
        public TServer ServerMessage { get; set; }
        public TClient ClientMessage { get; internal set; }
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
            ClientMessage = message;
        }

        public void Dispose()
        {
            namedPipe?.Stop();
        }
    }

    public class PipeProcessClient<TServer, TClient> : IDisposable
        where TServer : class, new()
        where TClient : class, new()
    {
        public TServer ServerMessage { get; internal set; }
        public TClient ClientMessage { get; set; }
        public string PipeName => pipeName;
        public NamedPipeClient<TServer, TClient> NamedPipe => namedPipe;

        private NamedPipeClient<TServer, TClient> namedPipe;
        private string pipeName;

        public PipeProcessClient(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public bool Initialize()
        {
            namedPipe = new NamedPipeClient<TServer, TClient>(pipeName);
            namedPipe.Connected += NamedPipe_Connected;
            namedPipe.ServerMessage += NamedPipe_ServerMessage;
            namedPipe.NamedPipeDebug();

            namedPipe.AutoReconnect = false;

            namedPipe.Start();
            namedPipe.WaitForConnection(1000);

            return true;
        }

        public void Update(Action<TClient> response)
        {
            if (ClientMessage is null) ClientMessage = new TClient();
            response(ClientMessage);
            NamedPipe?.PushMessage(ClientMessage);
        }

        private void NamedPipe_Connected(NamedPipeConnection<TServer, TClient> connection)
        {
            if (ClientMessage != null)
                connection.PushMessage(ClientMessage);
        }

        private void NamedPipe_ServerMessage(NamedPipeConnection<TServer, TClient> connection, TServer message)
        {
            ServerMessage = message;
        }

        public void Dispose()
        {
            namedPipe?.Stop();
            namedPipe?.WaitForDisconnection(1000);
        }
    }
}
