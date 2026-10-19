using Microsoft.EntityFrameworkCore;

namespace SoChiTieu.Models;

/// <summary>
/// DbContext (tương đương kết quả scaffold Database First).
/// Chuỗi kết nối KHÔNG viết cứng ở đây: được truyền vào qua DbContextOptions
/// (xem Helpers/DbContextHelper.cs, lấy từ appsettings.json).
/// </summary>
public partial class QL_ChiTieuContext : DbContext
{
    public QL_ChiTieuContext()
    {
    }

    public QL_ChiTieuContext(DbContextOptions<QL_ChiTieuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTieu> ChiTieus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTieu>(entity =>
        {
            entity.HasKey(e => e.MaCT).HasName("PK_ChiTieu");

            entity.ToTable("ChiTieu");

            entity.Property(e => e.NgayChi).HasColumnType("date");
            entity.Property(e => e.NoiDung).HasMaxLength(100);
            entity.Property(e => e.LoaiChi).HasMaxLength(30);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.GhiChu).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
