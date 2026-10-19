namespace SoChiTieu.Models;

/// <summary>
/// Model ánh xạ bảng dbo.ChiTieu (tương đương kết quả scaffold Database First).
/// Cột NgayChi kiểu date được ánh xạ sang DateTime để gắn lưới / DateTimePicker dễ hơn.
/// </summary>
public partial class ChiTieu
{
    public int MaCT { get; set; }

    public DateTime NgayChi { get; set; }

    public string NoiDung { get; set; } = string.Empty;

    public string LoaiChi { get; set; } = string.Empty;

    public decimal SoTien { get; set; }

    public string GhiChu { get; set; }
}
