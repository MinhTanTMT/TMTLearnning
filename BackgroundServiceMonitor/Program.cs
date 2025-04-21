/*
 
Đề bài: Giám sát dịch vụ nền (Background Service Monitor)
Mô tả:
Viết một chương trình console gồm 2 phần:

Service giả lập:
Một dịch vụ nền chạy bất đồng bộ, cứ mỗi 2 giây lại gửi trạng thái hiện tại (ví dụ: "Đang hoạt động", "Ngắt kết nối", v.v.). Trạng thái này được phát dưới dạng IObservable<string>.

Client giám sát:
Một thành phần đăng ký (Subscribe) vào observable của dịch vụ để nhận trạng thái mới. Sau 10 giây, client sẽ yêu cầu hủy theo dõi bằng CancellationTokenSource.

Yêu cầu kỹ thuật:

Sử dụng TaskCompletionSource để đợi đến khi trạng thái đầu tiên được gửi từ service (trước khi bắt đầu giám sát đầy đủ).

Trong quá trình phát trạng thái, dùng ConfigureAwait(false) ở các await bên trong service để kiểm soát context.

Cho phép dừng chương trình bằng CancellationToken.
 
 */

using BackgroundServiceMonitor;

class Program
{
    static async Task Main(string[] args)
    {

        var server = new MyServerTcp(5000);
        await server.StartMyServer();

        // 1. Tạo CancellationTokenSource
        // 2. Tạo TaskCompletionSource để chờ trạng thái đầu tiên
        // 3. Đăng ký Observer
        // 4. Sau 10s thì hủy
    }


    //static void Main()
    //{
    //    Console.WriteLine("Starting...");
    //    string result = GetDataAsync().Result;  // Chặn luồng chính và có nguy cơ deadlock
    //    Console.WriteLine(result);
    //}

    //static async Task<string> GetDataAsync()
    //{
    //    await Task.Delay(1000); // Có thể yêu cầu quay lại context (trong môi trường có SynchronizationContext)
    //    return "Done!";
    //}

}