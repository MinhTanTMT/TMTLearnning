using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TMTLearn_Tcp_Observable
{
    public class TcpClientApp
    {
        private readonly string _host;
        private readonly int _port;

        public TcpClientApp(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task StartAsync()
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            Console.WriteLine("[CLIENT] Connected to server.");
            using var stream = client.GetStream();

            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;
                if (input == "exit") break;

                byte[] buffer = Encoding.UTF8.GetBytes(input);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }

            Console.WriteLine("[CLIENT] Disconnected.");
        }
    }
}
