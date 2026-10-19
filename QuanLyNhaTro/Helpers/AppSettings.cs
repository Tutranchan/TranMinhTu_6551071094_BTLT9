using Microsoft.Extensions.Configuration;

namespace QuanLyNhaTro.Helpers;

public class ConnectionStringsSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
}

/// <summary>Mục "ProvinceApi" trong appsettings.json.</summary>
public class ProvinceApiSettings
{
    public string BaseUrl { get; set; } = "https://provinces.open-api.vn/api/v2/";
    public int TimeoutSeconds { get; set; } = 10;
}

/// <summary>
/// Lớp cấu hình có kiểu (strongly-typed).
/// appsettings.json được đọc trước, appsettings.Development.json (nếu có) ghi đè lên.
/// </summary>
public class AppSettings
{
    public ConnectionStringsSettings ConnectionStrings { get; set; } = new();

    public ProvinceApiSettings ProvinceApi { get; set; } = new();

    public static AppSettings Current { get; private set; } = new();

    public static AppSettings Load()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .Build();

        var settings = config.Get<AppSettings>() ?? new AppSettings();

        if (string.IsNullOrWhiteSpace(settings.ConnectionStrings.DefaultConnection))
            throw new InvalidOperationException(
                "Thiếu ConnectionStrings:DefaultConnection trong appsettings.json.");

        if (settings.ProvinceApi.TimeoutSeconds <= 0)
            settings.ProvinceApi.TimeoutSeconds = 10;

        Current = settings;
        return settings;
    }
}
