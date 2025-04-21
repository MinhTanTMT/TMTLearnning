using TMTLearn_TcpChatServer;

class Program
{
    static async Task Main()
    {
        var server = new TcpChatServer(5000);
        await server.StartAsync();
    }
}
