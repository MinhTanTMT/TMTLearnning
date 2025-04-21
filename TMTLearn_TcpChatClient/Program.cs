using TMTLearn_TcpChatClient;

class Program
{
    static async Task Main()
    {
        var client = new TcpChatClient("127.0.0.1", 5000);
        await client.StartAsync();
    }
}
