using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoDEADLOCKConfigureAwait
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _host = "127.0.0.1";
        private readonly int _port = 5000;
        private TcpClient _client;
        private NetworkStream _myStream;

        //
        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (s, e) => await StartMyClient();
        }

        public async Task StartMyClient()
        {
            _client = new TcpClient();
            await _client.ConnectAsync(_host, _port);
            MessageBox.Show("[CLIENT] Đa ket noi voi server.");
            _myStream = _client.GetStream();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            string result = GetDataAsync().Result;

            await PushDataAsync();

            try
            {
                string response = await WaitForResponseAsync(TimeSpan.FromSeconds(5));
                MessageBox.Show($"Server trả lời: {response}");
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show("Hết thời gian chờ server phản hồi!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private async Task<string> GetDataAsync()
        {
            await Task.Delay(1000).ConfigureAwait(false); // tránh deadlock
            //await Task.Delay(1000); // bị deadlock
            return "Done!";
        }

        private async Task PushDataAsync()
        {
            string? input = myMess.Text;
            if (string.IsNullOrWhiteSpace(input)) input = "";
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            await _myStream.WriteAsync(buffer, 0, buffer.Length);
        }




        //Đợi server phản hồi trong thời gian giới hạn (timeout). Nếu quá thời gian thì tự huỷ.
        private async Task<string> WaitForResponseAsync(TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<string>();
            var cts = new CancellationTokenSource(timeout);
            var buffer = new byte[1024];

            // Khi bị huỷ (timeout), set TaskCancelled
            cts.Token.Register(() => tcs.TrySetCanceled());

            // Đọc dữ liệu từ server
            _ = Task.Run(async () =>
            {
                try
                {
                    int bytesRead = await _myStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                    if (bytesRead > 0)
                    {
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        tcs.TrySetResult(response);
                    }
                    else
                    {
                        tcs.TrySetResult("[CLIENT] Không nhận được dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return await tcs.Task;
        }



    }
}