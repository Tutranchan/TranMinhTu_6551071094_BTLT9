using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaTro.Models;

/// <summary>
/// DbContext (tương đương kết quả scaffold Database First).
/// Chuỗi kết nối được truyền qua DbContextOptions, lấy từ appsettings.json (Helpers/DbContextHelper.cs).
/// </summary>
public partial class QL_NhaTroContext : DbContext
{
    public QL_NhaTroContext()
    {
    }

    public QL_NhaTroContext(DbContextOptions<QL_NhaTroContext> options)
        : base(options)
    {
    }

    public virtual DbSet<KhachThue> KhachThues { get; set; }

    public virtual DbSet<PhongTro> PhongTros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhongTro>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK_PhongTro");

            entity.ToTable("PhongTro");

            entity.Property(e => e.TenPhong).HasMaxLength(20);
            entity.Property(e => e.GiaThue).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TinhTrang).HasMaxLength(20);
        });

        modelBuilder.Entity<KhachThue>(entity =>
        {
            entity.HasKey(e => e.MaKhach).HasName("PK_KhachThue");

            entity.ToTable("KhachThue");

            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.NgaySinh).HasColumnType("date");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15).IsUnicode(false);
            entity.Property(e => e.TenTinh).HasMaxLength(100);
            entity.Property(e => e.TenPhuongXa).HasMaxLength(100);

            entity.HasOne(d => d.MaPhongNavigation)
                  .WithMany(p => p.KhachThues)
                  .HasForeignKey(d => d.MaPhong)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_KhachThue_PhongTro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
