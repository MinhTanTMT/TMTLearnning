using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TMTLearn_TcpChatClient
{
    public class TcpChatClient
    {
        private readonly string _host;
        private readonly int _port;

        public TcpChatClient(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task StartAsync()
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            Console.WriteLine("[CLIENT] Connected to chatroom.");
            var stream = client.GetStream();

            // Đọc dữ liệu từ server (chat của người khác)
            _ = Task.Run(async () =>
            {
                var buffer = new byte[4];
                while (true)
                {
                    try
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"\n[Chat] {message}");
                        Console.Write("> ");
                    }
                    catch
                    {
                        break;
                    }
                }
            });

            // Gửi input của mình
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (input == "exit") break;

                byte[] data = Encoding.UTF8.GetBytes(input + "\n");
                await stream.WriteAsync(data);
            }

            Console.WriteLine("[CLIENT] Disconnected.");
        }
    }
}
