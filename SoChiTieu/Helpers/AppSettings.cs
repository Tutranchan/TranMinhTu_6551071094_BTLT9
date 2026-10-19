using Microsoft.Extensions.Configuration;

namespace SoChiTieu.Helpers;

/// <summary>Mục "ConnectionStrings" trong appsettings.json.</summary>
public class ConnectionStringsSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
}

/// <summary>
/// Lớp cấu hình có kiểu (strongly-typed), gom toàn bộ appsettings.json.
/// Đọc bằng ConfigurationBuilder: appsettings.json, sau đó appsettings.Development.json ghi đè (nếu có).
/// </summary>
public class AppSettings
{
    public ConnectionStringsSettings ConnectionStrings { get; set; } = new();

    public static AppSettings Current { get; private set; } = new();

    public static AppSettings Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)               // cạnh file .exe, không phụ thuộc thư mục hiện hành
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .Build();

        var settings = config.Get<AppSettings>() ?? new AppSettings();

        if (string.IsNullOrWhiteSpace(settings.ConnectionStrings.DefaultConnection))
            throw new InvalidOperationException(
                "Thiếu ConnectionStrings:DefaultConnection trong appsettings.json.");

        Current = settings;
        return settings;
    }
}
