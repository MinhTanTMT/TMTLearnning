using System.Threading.Tasks;
using TMTLearn_Tcp_Observable;

class Program
{
    static async Task Main()
    {
        var client = new TcpClientApp("192.168.1.2", 5000);
        await client.StartAsync();
    }
}

