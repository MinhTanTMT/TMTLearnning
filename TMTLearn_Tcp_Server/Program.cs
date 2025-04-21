using System.Threading.Tasks;
using TMTLearn_Tcp_Observable;

class Program
{
    static async Task Main()
    {
        var server = new TcpServer(5000);
        await server.StartAsync();
    }
}
