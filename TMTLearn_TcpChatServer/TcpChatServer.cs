using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMTLearn_TcpChatServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Sockets;
    using System.Reactive.Subjects;
    using System.Text;
    using System.Threading.Tasks;

    public class TcpChatServer
    {
        private readonly int _port; // khai báo số cổng port
        private TcpListener? _listener; // khai báo biến lắng nghe

        // Broadcast đến tất cả client
        private readonly Subject<string> _broadcastSubject = new(); // ko rõ Broadcast là cái gì

        // Danh sách các stream để gửi message đến client
        private readonly ConcurrentBag<NetworkStream> _clientStreams = new(); // cái này hinh như dùng để thu gom các cái ip của các appClient để có thể gọi cho nó hay sao ấy

        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public TcpChatServer(int port)
        {
            _port = port;
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port); // Địa chỉ loopback (nội bộ) 127.0.0.1 – chỉ dùng được trên chính máy đó thế
                                                                    // nên call bên Program mới có mỗi new TcpChatServer(5000); vì IP tự set rồi
                                                                    // còn nếu thay cái IPAddress.Any thì nó sẽ tìm mọi IP miễn sao có cổng 5000 và kết nối
            _listener.Start();
            Console.WriteLine($"[SERVER] Chatroom server listening on port {_port}...");

            while (true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("[SERVER] Client connected.");
                _ = HandleClientAsync(client); 
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            _clientStreams.Add(stream);

            var subscription = _broadcastSubject.Subscribe(async message =>
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(data);
                }
                catch
                {
                    // Ignore lỗi ghi (client ngắt chẳng hạn)
                }
            });

            _subscriptions.Add(subscription); // Lưu subscription để có thể unsubscribe sau này

            var buffer = new byte[4];
            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[SERVER] Received: {message}");

                    _broadcastSubject.OnNext(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVER] Error: {ex.Message}");
            }
            finally
            {
                subscription.Dispose(); // Unsubscribe khi client ngắt
                _subscriptions.Remove(subscription); // Xóa khỏi danh sách
                client.Close();
                Console.WriteLine("[SERVER] Client disconnected.");
            }
        }

        // Hàm để unsubscribe tất cả subscriptions chủ động
        private void UnsubscribeAll()
        {
            foreach (var sub in _subscriptions)
            {
                sub.Dispose();
            }
            _subscriptions.Clear();
            Console.WriteLine("Đã hủy tất cả subscriptions.");
        }
    }

}
