using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundClientMonitor
{
    public class MyClientTcp
    {
        private readonly string _host;
        private readonly int _port;

        public MyClientTcp(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task StartMyClient()
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            Console.WriteLine("[CLIENT] Đa ket noi voi server.");
            using var myStream = client.GetStream();

            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;
                if (input == "exit") break;
                byte[] buffer = Encoding.UTF8.GetBytes(input);
                await myStream.WriteAsync(buffer, 0, buffer.Length);

            }
        }
    }
}
