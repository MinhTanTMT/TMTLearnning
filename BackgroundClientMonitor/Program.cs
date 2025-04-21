using BackgroundClientMonitor;

class Program
{
    static async Task Main()
    {
        var client = new MyClientTcp("127.0.0.1", 5000);
        await client.StartMyClient();
    }
}