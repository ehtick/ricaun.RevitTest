using NamedPipeWrapper;
using System;
using System.Diagnostics;

namespace ricaun.RevitTest.Shared
{
    internal static class NamedPipeDebugExtension
    {
        [Conditional("DEBUG")]
        internal static void NamedPipeDebug<TRead, TWrite>(this NamedPipeServer<TRead, TWrite> server)
            where TRead : class
            where TWrite : class
        {
            server.ClientConnected += (connection) =>
            {
                Console.WriteLine($"[{connection.Id}] ClientConnected");
            };
            server.ClientDisconnected += (connection) =>
            {
                Console.WriteLine($"[{connection.Id}] ClientDisconnected");
            };
            server.ClientMessage += (connection, message) =>
            {
                Console.WriteLine($"[{connection.Id}] ClientMessage: \t{message}");
            };
        }

        [Conditional("DEBUG")]
        internal static void NamedPipeDebug<TRead, TWrite>(this NamedPipeClient<TRead, TWrite> client)
            where TRead : class
            where TWrite : class
        {
            client.Connected += (connection) =>
            {
                Console.WriteLine($"[{connection.Id}] Connected");
            };

            client.Disconnected += (connection) =>
            {
                Console.WriteLine($"[{connection.Id}] Disconnected");
            };

            client.ServerMessage += (connection, message) =>
            {
                Console.WriteLine($"[{connection.Id}] ServerMessage: \t{message}");
            };
        }
    }
}
