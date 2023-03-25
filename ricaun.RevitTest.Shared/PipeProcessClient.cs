using NamedPipeWrapper;
using System;

namespace ricaun.RevitTest.Shared
{
    public class PipeProcessClient<TServer, TClient> : IDisposable
        where TServer : class, new()
        where TClient : class, new()
    {
        public TServer ServerMessage { get; internal set; } = new TServer();
        public TClient ClientMessage { get; internal set; } = new TClient();
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
            //namedPipe.NamedPipeDebug();

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
            Mappers.Mapper.Map(message, ServerMessage);
        }

        public void Dispose()
        {
            namedPipe?.Stop();
            namedPipe?.WaitForDisconnection(1000);
        }
    }
}
