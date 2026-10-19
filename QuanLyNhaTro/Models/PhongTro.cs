namespace QuanLyNhaTro.Models;

/// <summary>Model ánh xạ bảng dbo.PhongTro (tương đương kết quả scaffold Database First).</summary>
public partial class PhongTro
{
    public int MaPhong { get; set; }

    public string TenPhong { get; set; } = string.Empty;

    public decimal GiaThue { get; set; }

    public string TinhTrang { get; set; } = "Trống";

    /// <summary>Navigation property: các khách đang thuê phòng này (quan hệ 1 - nhiều).</summary>
    public virtual ICollection<KhachThue> KhachThues { get; set; } = new List<KhachThue>();
}
