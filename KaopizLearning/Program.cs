public class MyNumber : IMyNumber
{
    private int value;

    public MyNumber(int val)
    {
        value = val;
    }

    public void Display()
    {
        Console.WriteLine($"Giá trị: {value}");
    }

    public void Dispose()
    {
        Console.WriteLine("Giải phóng tài nguyên!");
    }
}

// Dùng với using:
class Program
{
    static async Task Main()
    {
        // Dùng với using:
        using (var number = new MyNumber(42))
        {
            number.Display();
        } // ✅ Khi kết thúc khối, `Dispose()` sẽ tự động được gọi
    }
}