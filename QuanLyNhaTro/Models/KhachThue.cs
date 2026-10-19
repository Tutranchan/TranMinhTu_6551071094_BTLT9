namespace QuanLyNhaTro.Models;

/// <summary>Model ánh xạ bảng dbo.KhachThue (tương đương kết quả scaffold Database First).</summary>
public partial class KhachThue
{
    public int MaKhach { get; set; }

    public string HoTen { get; set; } = string.Empty;

    public string GioiTinh { get; set; } = "Nam";

    public DateTime NgaySinh { get; set; }

    public string SoDienThoai { get; set; } = string.Empty;

    /// <summary>Mã tỉnh lấy từ API; null nếu người dùng gõ tay khi mất mạng.</summary>
    public int? MaTinh { get; set; }

    public string TenTinh { get; set; }

    /// <summary>Mã phường/xã lấy từ API; null nếu người dùng gõ tay khi mất mạng.</summary>
    public int? MaPhuongXa { get; set; }

    public string TenPhuongXa { get; set; }

    public bool DaDangKyTamTru { get; set; }

    public int MaPhong { get; set; }

    /// <summary>Navigation property: phòng mà khách đang thuê.</summary>
    public virtual PhongTro MaPhongNavigation { get; set; } = null;
}
