using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Models;

namespace QuanLyNhaTro.Helpers;

/// <summary>Tạo DbContext mới cho mỗi thao tác, dùng chuỗi kết nối từ appsettings.json.</summary>
public static class DbContextHelper
{
    public static QL_NhaTroContext Create()
    {
        var options = new DbContextOptionsBuilder<QL_NhaTroContext>()
            .UseSqlServer(AppSettings.Current.ConnectionStrings.DefaultConnection)
            .Options;

        return new QL_NhaTroContext(options);
    }
}
