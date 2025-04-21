using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackgroundServiceMonitor
{
    public class MyServerTcp
    {
        private readonly int _port;
        private TcpListener _listener;

        public MyServerTcp(int port)
        {
            _port = port;
        }

        public async Task StartMyServer()
        {
            _listener = new TcpListener(IPAddress.Loopback,_port);
            _listener.Start();
            Console.WriteLine($"Lang nghe cong {_port}");

            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Ham lap đe lang nghe su kien: 1 client moi đa ket noi");
                _ = HandleMyClient(client);
            }
        }


        private async Task HandleMyClient(TcpClient client)
        {
            using var stream = client.GetStream();
            var myObservable = ServiceStatusPublisher.TMTCreateMyObservable(stream);

            var subscription = myObservable.Subscribe(
                async data =>
                {
                    Console.WriteLine($"[SERVER] Nhan đuoc: {data}");

                    // Gửi phản hồi lại client
                    string response = $"Server da nhan: {data}";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                },
                ex => Console.WriteLine($"[SERVER] Loi: {ex.Message}"),
                () => Console.WriteLine("[SERVER] Client Mat ket noi.")
            );

            /* // cái trên là cách viết khác của khai báo string class Observer() và trền vào dữ liệu,
             // và mấy cái hàm onNext, onError, onCompleted mình gọi khi triển khai hàm bên kia sẽ tương ứng với đoạn string đã khai báo
             myObservable.Subscribe(
                new Observer<string>(
                    onNext: data => Console.WriteLine($"Received: {data}"),
                    onError: ex => Console.WriteLine($"Error: {ex.Message}"),
                    onCompleted: () => Console.WriteLine("Completed.")
                )
            );
             */

            // Giữ kết nối đến khi client ngắt


            while (client.Connected)
            {
                await Task.Delay(1000);
            }

            subscription.Dispose();
        }


        private async Task DelayMyWork()
        {
            await Task.Delay(5000);
            Console.WriteLine("Đã xử lý xong công việc nặng");
        }


        private async Task UIPrint()
        {
            await Task.Delay(2000);
            Console.WriteLine("My UI");
        }

    }
}
