using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using QuanLyNhaTro.Helpers;

namespace QuanLyNhaTro.Services;

/// <summary>Một tỉnh/thành từ API (GET .../api/v2/).</summary>
public class Tinh
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("code")] public int Code { get; set; }
    public override string ToString() => Name;
}

/// <summary>Một phường/xã từ API (nằm trong mảng "wards" của GET .../api/v2/p/{code}?depth=2).</summary>
public class PhuongXa
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("code")] public int Code { get; set; }
    public override string ToString() => Name;
}

internal class TinhChiTiet
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("wards")] public List<PhuongXa> Wards { get; set; } = new();
}

/// <summary>
/// Gọi API hành chính Việt Nam (provinces.open-api.vn).
/// - MỘT HttpClient dùng chung cho cả ứng dụng (không tạo mới mỗi lần gọi).
/// - Timeout lấy từ appsettings.json (mặc định 10 giây).
/// - Lỗi mạng / timeout / JSON sai được ném ra cho Form bắt (HttpRequestException, TaskCanceledException, JsonException).
/// </summary>
public static class DiaChiService
{
    private static readonly Lazy<HttpClient> _client = new(() => new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(AppSettings.Current.ProvinceApi.TimeoutSeconds)
    });

    private static HttpClient Http => _client.Value;

    private static string BaseUrl => AppSettings.Current.ProvinceApi.BaseUrl.TrimEnd('/') + "/";

    /// <summary>GET https://provinces.open-api.vn/api/v2/</summary>
    public static async Task<List<Tinh>> LayDanhSachTinhAsync(CancellationToken ct = default)
    {
        using var resp = await Http.GetAsync(BaseUrl, ct);
        resp.EnsureSuccessStatusCode();

        await using var stream = await resp.Content.ReadAsStreamAsync(ct);
        var ds = await JsonSerializer.DeserializeAsync<List<Tinh>>(stream, cancellationToken: ct);
        return ds ?? new List<Tinh>();
    }

    /// <summary>GET https://provinces.open-api.vn/api/v2/p/{code}?depth=2 rồi lấy mảng "wards".</summary>
    public static async Task<List<PhuongXa>> LayPhuongXaAsync(int maTinh, CancellationToken ct = default)
    {
        var url = $"{BaseUrl}p/{maTinh}?depth=2";

        using var resp = await Http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();

        await using var stream = await resp.Content.ReadAsStreamAsync(ct);
        var tinh = await JsonSerializer.DeserializeAsync<TinhChiTiet>(stream, cancellationToken: ct);
        return tinh?.Wards ?? new List<PhuongXa>();
    }
}
