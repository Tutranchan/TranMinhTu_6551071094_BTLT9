using Serilog;

namespace QuanLyNhaTro.Helpers;

/// <summary>Serilog ghi file theo ngày vào thư mục Logs cạnh file .exe (log-yyyyMMdd.txt).</summary>
public static class LogHelper
{
    public static string LogFolder => Path.Combine(AppContext.BaseDirectory, "Logs");

    public static void Init()
    {
        Directory.CreateDirectory(LogFolder);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                path: Path.Combine(LogFolder, "log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                shared: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static void Close() => Log.CloseAndFlush();
}
