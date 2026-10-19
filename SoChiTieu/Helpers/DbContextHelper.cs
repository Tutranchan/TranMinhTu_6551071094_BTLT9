using Microsoft.EntityFrameworkCore;
using SoChiTieu.Models;

namespace SoChiTieu.Helpers;

/// <summary>Tạo DbContext mới cho mỗi thao tác, dùng chuỗi kết nối đọc từ appsettings.json.</summary>
public static class DbContextHelper
{
    public static QL_ChiTieuContext Create()
    {
        var options = new DbContextOptionsBuilder<QL_ChiTieuContext>()
            .UseSqlServer(AppSettings.Current.ConnectionStrings.DefaultConnection)
            .Options;

        return new QL_ChiTieuContext(options);
    }
}
