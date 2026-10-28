
namespace Console_app5.Services;

public class DownloadService
{
    public async Task DownloadFileAsync(
        string fileName, int delayMilliseconds)
    {
        Console.WriteLine($"{fileName}: Started");

        await Task.Delay(delayMilliseconds);

        Console.WriteLine($"{fileName}: Completed");
    }

    public async Task<int> GetValueAsync(
        int value, int delayMilliseconds)
    {
        await Task.Delay(delayMilliseconds);

        return value * 10;
    }

    public async Task<string> GetMessageAsync(
        string message, int delayMilliseconds)
    {
        await Task.Delay(delayMilliseconds);

        return $"{message} finished";
    }
}
s