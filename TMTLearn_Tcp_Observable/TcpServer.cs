using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TMTLearn_Tcp_Observable
{
    public class TcpServer
    {
        private readonly int _port;
        private TcpListener? _listener;

        public TcpServer(int port)
        {
            _port = port;
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();
            Console.WriteLine($"[SERVER] Listening on port {_port}...");

            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("[SERVER] Client connected.");

                _ = HandleClientAsync(client); // fire-and-forget
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using var stream = client.GetStream();
            var observable = TcpObservableStream.CreateObservable(stream);

            var subscription = observable.Subscribe(
                data => Console.WriteLine($"[SERVER] Received: {data}"),
                ex => Console.WriteLine($"[SERVER] Error: {ex.Message}"),
                () => Console.WriteLine("[SERVER] Client disconnected.")
            );

            // Giữ kết nối đến khi client ngắt
            while (client.Connected)
            {
                await Task.Delay(1000);
            }

            subscription.Dispose();
        }
    }
}
