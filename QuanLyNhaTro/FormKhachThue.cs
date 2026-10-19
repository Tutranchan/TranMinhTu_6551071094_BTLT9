using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Helpers;
using QuanLyNhaTro.Models;
using QuanLyNhaTro.Services;
using Serilog;

namespace QuanLyNhaTro;

/// <summary>Một mục ComboBox phòng (Mã phòng + chuỗi hiển thị).</summary>
internal class PhongItem
{
    public int MaPhong { get; set; }
    public string Hien { get; set; } = string.Empty;
    public override string ToString() => Hien;
}

/// <summary>Một dòng của lưới = kết quả JOIN KhachThue + PhongTro.</summary>
internal class KhachRow
{
    public int MaKhach { get; set; }
    public string HoTen { get; set; }
    public string GioiTinh { get; set; }
    public DateTime NgaySinh { get; set; }
    public string SoDienThoai { get; set; }
    public int MaPhong { get; set; }
    public string TenPhong { get; set; }
    public decimal GiaThue { get; set; }
    public int? MaTinh { get; set; }
    public string TenTinh { get; set; }
    public int? MaPhuongXa { get; set; }
    public string TenPhuongXa { get; set; }
    public bool DaDangKyTamTru { get; set; }

    /// <summary>Quê quán hiển thị: "Phường/Xã - Tỉnh".</summary>
    public string QueQuan => string.Join(" - ",
        new[] { TenPhuongXa, TenTinh }.Where(s => !string.IsNullOrWhiteSpace(s)));
}

public partial class FormKhachThue : Form
{
    private static readonly CultureInfo Vi = new("vi-VN");
    private const string TatCa = "Tất cả";
    private const string Trong = "Trống";
    private const string DangThue = "Đang thuê";

    private bool _suppressFilter = true;   // chặn sự kiện bộ lọc khi đang nạp ComboBox
    private bool _loadingGrid;             // chặn SelectionChanged khi đang gắn DataSource cho lưới
    private bool _suppressAddr;            // chặn SelectedIndexChanged của ComboBox Tỉnh khi gán bằng code
    private bool _apiTinhOk;               // true: đã nạp được danh sách tỉnh từ API
    private int _maKhach;                  // khách đang chọn để sửa (0 = chưa chọn)
    private CancellationTokenSource _xaCts; // hủy lần tải phường/xã cũ khi người dùng đổi tỉnh nhanh

    public FormKhachThue()
    {
        InitializeComponent();
        CauHinhLuoi();

        cboPhong.DisplayMember = "Hien";
        cboPhong.ValueMember = "MaPhong";
        cboLocPhong.DisplayMember = "Hien";
        cboLocPhong.ValueMember = "MaPhong";
        cboTinh.DisplayMember = "Name";    // thuộc tính "name" của JSON
        cboTinh.ValueMember = "Code";      // thuộc tính "code" của JSON
        cboXa.DisplayMember = "Name";
        cboXa.ValueMember = "Code";

        Load += FormKhachThue_Load;
    }

    private async void FormKhachThue_Load(object sender, EventArgs e)
    {
        try
        {
            _suppressFilter = true;
            TaiPhongChoCombo(null);
            TaiBoLoc();
            _suppressFilter = false;
            XoaForm();
            TaiDuLieu();
        }
        catch (Exception ex)
        {
            _suppressFilter = false;
            BaoLoiCSDL(ex, "mở form");
        }

        // Gọi API bất đồng bộ - giao diện không bị đơ trong lúc chờ
        await TaiTinhAsync();
    }

    // =====================================================================
    //  Lưới
    // =====================================================================

    private void CauHinhLuoi()
    {
        var g = dgvKhach;
        g.AutoGenerateColumns = false;
        g.ReadOnly = true;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.MultiSelect = false;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        g.BackgroundColor = Color.White;
        g.RowHeadersWidth = 24;

        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTen", HeaderText = "Họ tên", FillWeight = 110 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoDienThoai", HeaderText = "SĐT", FillWeight = 80 });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenPhong", HeaderText = "Phòng", FillWeight = 45 });
        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "GiaThue", HeaderText = "Giá thuê (đ)", FillWeight = 75,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", FormatProvider = Vi, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        g.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "QueQuan", HeaderText = "Quê quán", FillWeight = 140 });
        g.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "DaDangKyTamTru", HeaderText = "Tạm trú", FillWeight = 50 });
    }

    /// <summary>JOIN KhachThue + PhongTro, áp dụng bộ lọc rồi gắn vào lưới.</summary>
    private void TaiDuLieu()
    {
        if (_suppressFilter) return;

        try
        {
            using var db = DbContextHelper.Create();

            var q = from k in db.KhachThues.AsNoTracking()
                    join p in db.PhongTros.AsNoTracking() on k.MaPhong equals p.MaPhong
                    select new { k, p };

            var tuKhoa = txtTim.Text.Trim();
            if (tuKhoa.Length > 0)
                q = q.Where(x => x.k.HoTen.Contains(tuKhoa) || x.k.SoDienThoai.Contains(tuKhoa));

            var maPhongLoc = (cboLocPhong.SelectedItem as PhongItem)?.MaPhong ?? 0;
            if (maPhongLoc > 0)
                q = q.Where(x => x.p.MaPhong == maPhongLoc);

            if (cboLocTinh.SelectedIndex > 0 && cboLocTinh.SelectedItem is string tinhLoc)
                q = q.Where(x => x.k.TenTinh == tinhLoc);

            var ds = q.OrderBy(x => x.k.MaKhach)
                      .ToList()
                      .Select(x => new KhachRow
                      {
                          MaKhach = x.k.MaKhach,
                          HoTen = x.k.HoTen,
                          GioiTinh = x.k.GioiTinh,
                          NgaySinh = x.k.NgaySinh,
                          SoDienThoai = x.k.SoDienThoai,
                          MaPhong = x.p.MaPhong,
                          TenPhong = x.p.TenPhong,
                          GiaThue = x.p.GiaThue,
                          MaTinh = x.k.MaTinh,
                          TenTinh = x.k.TenTinh,
                          MaPhuongXa = x.k.MaPhuongXa,
                          TenPhuongXa = x.k.TenPhuongXa,
                          DaDangKyTamTru = x.k.DaDangKyTamTru
                      })
                      .ToList();

            _loadingGrid = true;
            dgvKhach.DataSource = ds;
            dgvKhach.ClearSelection();
            _loadingGrid = false;
        }
        catch (Exception ex)
        {
            _loadingGrid = false;
            BaoLoiCSDL(ex, "tải danh sách khách thuê");
        }
    }

    // =====================================================================
    //  Nạp ComboBox phòng / bộ lọc
    // =====================================================================

    /// <summary>
    /// ComboBox phòng khi nhập khách: chỉ liệt kê phòng "Trống".
    /// Khi đang sửa một khách thì giữ thêm phòng hiện tại của khách đó (<paramref name="giuMaPhong"/>).
    /// </summary>
    private void TaiPhongChoCombo(int? giuMaPhong)
    {
        using var db = DbContextHelper.Create();
        var giu = giuMaPhong ?? -1;

        var ds = db.PhongTros.AsNoTracking()
            .Where(p => p.TinhTrang == Trong || p.MaPhong == giu)
            .OrderBy(p => p.TenPhong)
            .ToList()
            .Select(p => new PhongItem
            {
                MaPhong = p.MaPhong,
                Hien = $"{p.TenPhong} - {p.GiaThue.ToString("N0", Vi)} đ"
            })
            .ToList();

        var dangChon = (cboPhong.SelectedItem as PhongItem)?.MaPhong;
        cboPhong.DataSource = ds;

        if (giuMaPhong.HasValue && ds.Any(x => x.MaPhong == giuMaPhong.Value))
            cboPhong.SelectedValue = giuMaPhong.Value;
        else if (dangChon.HasValue && ds.Any(x => x.MaPhong == dangChon.Value))
            cboPhong.SelectedValue = dangChon.Value;
        else
            cboPhong.SelectedIndex = ds.Count > 0 ? 0 : -1;
    }

    /// <summary>Nạp lại hai ComboBox lọc (Phòng, Tỉnh) và giữ lựa chọn cũ nếu còn.</summary>
    private void TaiBoLoc()
    {
        var cu = _suppressFilter;
        _suppressFilter = true;
        try
        {
            using var db = DbContextHelper.Create();

            var maPhongCu = (cboLocPhong.SelectedItem as PhongItem)?.MaPhong ?? 0;
            var tinhCu = cboLocTinh.SelectedItem as string;

            var phongs = new List<PhongItem> { new PhongItem { MaPhong = 0, Hien = TatCa } };
            phongs.AddRange(db.PhongTros.AsNoTracking()
                .OrderBy(p => p.TenPhong)
                .Select(p => new PhongItem { MaPhong = p.MaPhong, Hien = p.TenPhong })
                .ToList());
            cboLocPhong.DataSource = phongs;
            cboLocPhong.SelectedIndex = 0;
            if (phongs.Any(x => x.MaPhong == maPhongCu)) cboLocPhong.SelectedValue = maPhongCu;

            var tinhs = new List<string> { TatCa };
            tinhs.AddRange(db.KhachThues.AsNoTracking()
                .Where(k => k.TenTinh != null && k.TenTinh != "")
                .Select(k => k.TenTinh)
                .Distinct()
                .OrderBy(s => s)
                .ToList());
            cboLocTinh.DataSource = tinhs;
            cboLocTinh.SelectedIndex = 0;
            if (tinhCu != null && tinhs.Contains(tinhCu)) cboLocTinh.SelectedItem = tinhCu;
        }
        finally
        {
            _suppressFilter = cu;
        }
    }

    // =====================================================================
    //  API Tỉnh/Thành - Phường/Xã (async/await, chịu lỗi mạng)
    // =====================================================================

    private async Task TaiTinhAsync()
    {
        SetStatus("Đang tải danh sách tỉnh/thành…");

        _suppressAddr = true;
        cboTinh.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTinh.DataSource = new List<Tinh> { new Tinh { Name = "Đang tải…", Code = 0 } };
        _suppressAddr = false;
        cboTinh.Enabled = false;   // khóa trong lúc chờ

        try
        {
            var ds = await DiaChiService.LayDanhSachTinhAsync();

            _suppressAddr = true;
            cboTinh.DataSource = ds;
            cboTinh.SelectedIndex = -1;
            _suppressAddr = false;

            _apiTinhOk = true;
            DatXaTrong();
            SetStatus($"Đã tải {ds.Count} tỉnh/thành từ provinces.open-api.vn");
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
        {
            _apiTinhOk = false;
            Log.Warning(ex, "Khong tai duoc danh sach tinh/thanh tu API (mat mang / timeout), chuyen sang nhap tay");

            _suppressAddr = true;
            cboTinh.DataSource = null;
            cboTinh.DropDownStyle = ComboBoxStyle.DropDown;   // cho phép gõ tay
            cboTinh.Text = string.Empty;
            _suppressAddr = false;
            DatXaTrong();

            SetStatus("Không tải được danh sách địa chỉ – hãy nhập tay Tỉnh/Thành và Phường/Xã");
            MessageBox.Show("Không tải được danh sách địa chỉ, vui lòng kiểm tra mạng",
                "Quản lý nhà trọ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        finally
        {
            cboTinh.Enabled = true;
        }
    }

    private async void cboTinh_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_suppressAddr || !_apiTinhOk) return;

        if (cboTinh.SelectedItem is Tinh t && t.Code > 0)
            await TaiPhuongXaAsync(t.Code);
        else
            DatXaTrong();
    }

    private async Task TaiPhuongXaAsync(int maTinh)
    {
        _xaCts?.Cancel();                       // hủy lần tải cũ (nếu người dùng đổi tỉnh liên tục)
        var cts = new CancellationTokenSource();
        _xaCts = cts;

        SetStatus("Đang tải danh sách phường/xã…");
        cboXa.DropDownStyle = ComboBoxStyle.DropDownList;
        cboXa.DataSource = new List<PhuongXa> { new PhuongXa { Name = "Đang tải…", Code = 0 } };
        cboXa.Enabled = false;                  // khóa trong lúc chờ

        try
        {
            var ds = await DiaChiService.LayPhuongXaAsync(maTinh, cts.Token);
            if (cts != _xaCts) return;          // đã có lần chọn mới hơn

            cboXa.DataSource = ds;
            cboXa.SelectedIndex = -1;
            SetStatus($"Đã tải {ds.Count} phường/xã");
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            // Do chính mình hủy (đổi tỉnh) - bỏ qua
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
        {
            if (cts != _xaCts) return;

            Log.Warning(ex, "Khong tai duoc danh sach phuong/xa cua tinh {MaTinh}, chuyen sang nhap tay", maTinh);
            ChuyenXaSangNhapTay();
            SetStatus("Không tải được danh sách phường/xã – hãy nhập tay");
            MessageBox.Show("Không tải được danh sách địa chỉ, vui lòng kiểm tra mạng",
                "Quản lý nhà trọ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        finally
        {
            if (cts == _xaCts) cboXa.Enabled = true;
        }
    }

    /// <summary>Cho phép gõ tay Phường/Xã (khi API lỗi).</summary>
    private void ChuyenXaSangNhapTay()
    {
        cboXa.DataSource = null;
        cboXa.DropDownStyle = ComboBoxStyle.DropDown;
        cboXa.Text = string.Empty;
        cboXa.Enabled = true;
    }

    /// <summary>Đưa ComboBox Phường/Xã về trạng thái trống (hủy lần tải đang chờ nếu có).</summary>
    private void DatXaTrong()
    {
        _xaCts?.Cancel();
        cboXa.DataSource = null;
        cboXa.DropDownStyle = _apiTinhOk ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;
        cboXa.Text = string.Empty;
        cboXa.Enabled = true;
    }

    private (int? ma, string ten) LayTinh()
    {
        if (cboTinh.SelectedItem is Tinh t && t.Code > 0) return (t.Code, t.Name);
        return (null, _apiTinhOk ? string.Empty : cboTinh.Text.Trim());
    }

    private (int? ma, string ten) LayXa()
    {
        if (cboXa.SelectedItem is PhuongXa x && x.Code > 0) return (x.Code, x.Name);
        return (null, cboXa.DropDownStyle == ComboBoxStyle.DropDown ? cboXa.Text.Trim() : string.Empty);
    }

    private void ChonXaTheoMa(int? ma, string ten)
    {
        if (cboXa.DropDownStyle == ComboBoxStyle.DropDown)   // đang ở chế độ nhập tay
        {
            cboXa.Text = ten ?? string.Empty;
            return;
        }

        if (ma.HasValue && cboXa.DataSource is List<PhuongXa> ds)
        {
            var x = ds.FirstOrDefault(p => p.Code > 0 && p.Code == ma.Value);
            if (x != null)
            {
                cboXa.SelectedItem = x;
                return;
            }
        }
        cboXa.SelectedIndex = -1;
    }

    /// <summary>Đổ quê quán của khách đang chọn lên ComboBox Tỉnh và Phường/Xã.</summary>
    private async Task ChonDiaChiAsync(KhachRow r)
    {
        if (_apiTinhOk)
        {
            var t = (cboTinh.DataSource as List<Tinh>)?
                .FirstOrDefault(x => x.Code > 0 && r.MaTinh.HasValue && x.Code == r.MaTinh.Value);

            if (t != null)
            {
                _suppressAddr = true;
                cboTinh.SelectedItem = t;
                _suppressAddr = false;

                await TaiPhuongXaAsync(t.Code);
                if (_maKhach == r.MaKhach) ChonXaTheoMa(r.MaPhuongXa, r.TenPhuongXa);
                return;
            }

            // Dữ liệu cũ nhập tay, không khớp danh sách API -> để trống cho chọn lại
            _suppressAddr = true;
            cboTinh.SelectedIndex = -1;
            _suppressAddr = false;
            DatXaTrong();
            return;
        }

        cboTinh.Text = r.TenTinh ?? string.Empty;
        DatXaTrong();
        cboXa.Text = r.TenPhuongXa ?? string.Empty;
    }

    // =====================================================================
    //  Chọn dòng -> đổ lên control
    // =====================================================================

    private async void dgvKhach_SelectionChanged(object sender, EventArgs e)
    {
        if (_loadingGrid) return;
        var row = dgvKhach.CurrentRow;
        if (row == null || !row.Selected) return;
        if (row.DataBoundItem is not KhachRow r) return;

        try
        {
            _maKhach = r.MaKhach;
            txtHoTen.Text = r.HoTen;
            rdoNam.Checked = r.GioiTinh != "Nữ";
            rdoNu.Checked = r.GioiTinh == "Nữ";
            dtpNgaySinh.Value = r.NgaySinh < dtpNgaySinh.MinDate ? dtpNgaySinh.MinDate : r.NgaySinh;
            txtSDT.Text = r.SoDienThoai;
            chkTamTru.Checked = r.DaDangKyTamTru;

            TaiPhongChoCombo(r.MaPhong);          // gồm các phòng trống + phòng hiện tại của khách
            await ChonDiaChiAsync(r);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Loi khi hien thi khach MaKhach={MaKhach} len form", r.MaKhach);
            MessageBox.Show("Không thể hiển thị thông tin khách này. Thông tin lỗi đã được ghi lại.",
                "Quản lý nhà trọ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // =====================================================================
    //  Kiểm tra hợp lệ
    // =====================================================================

    private bool KiemTra(out (int? ma, string ten) tinh, out (int? ma, string ten) xa, out int maPhong)
    {
        tinh = default;
        xa = default;
        maPhong = 0;

        if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            return Canh("Vui lòng nhập họ tên.", txtHoTen);

        if (dtpNgaySinh.Value.Date > DateTime.Today)
            return Canh("Ngày sinh không được lớn hơn hôm nay.", dtpNgaySinh);

        if (!Regex.IsMatch(txtSDT.Text.Trim(), @"^\d{9,11}$"))
            return Canh("Số điện thoại phải gồm 9–11 chữ số.", txtSDT);

        if (cboPhong.SelectedItem is not PhongItem pi)
            return Canh("Không có phòng trống để chọn. Hãy trả phòng hoặc chọn một khách để sửa.", cboPhong);
        maPhong = pi.MaPhong;

        if (!cboTinh.Enabled || !cboXa.Enabled)
            return Canh("Danh sách địa chỉ đang được tải, vui lòng đợi giây lát.", cboTinh);

        tinh = LayTinh();
        if (string.IsNullOrWhiteSpace(tinh.ten))
            return Canh("Vui lòng chọn (hoặc nhập) Tỉnh/Thành.", cboTinh);

        xa = LayXa();
        if (string.IsNullOrWhiteSpace(xa.ten))
            return Canh("Vui lòng chọn (hoặc nhập) Phường/Xã.", cboXa);

        return true;
    }

    private static bool Canh(string noiDung, Control control)
    {
        MessageBox.Show(noiDung, "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
        return false;
    }

    private static void BaoLoiCSDL(Exception ex, string hanhDong)
    {
        Log.Error(ex, "Loi CSDL khi {HanhDong}", hanhDong);
        MessageBox.Show(
            "Không thể truy cập cơ sở dữ liệu. Vui lòng kiểm tra kết nối rồi thử lại.\n(Chi tiết đã được ghi vào file log.)",
            "Quản lý nhà trọ", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void SetStatus(string text) => tslTrangThai.Text = text;

    // =====================================================================
    //  CRUD nhiều bảng: KhachThue + cập nhật PhongTro.TinhTrang
    // =====================================================================

    /// <summary>Phòng còn ít nhất một khách -> "Đang thuê"; không còn ai -> "Trống".</summary>
    private static void CapNhatTrangThaiPhong(QL_NhaTroContext db, params int[] maPhongs)
    {
        foreach (var ma in maPhongs.Distinct())
        {
            var phong = db.PhongTros.Find(ma);
            if (phong == null) continue;
            phong.TinhTrang = db.KhachThues.Any(x => x.MaPhong == ma) ? DangThue : Trong;
        }
    }

    private void btnThem_Click(object sender, EventArgs e)
    {
        if (!KiemTra(out var tinh, out var xa, out var maPhong)) return;

        try
        {
            using var db = DbContextHelper.Create();
            using var tx = db.Database.BeginTransaction();

            var phong = db.PhongTros.Find(maPhong);
            if (phong == null || phong.TinhTrang != Trong)
            {
                MessageBox.Show("Phòng này không còn trống. Vui lòng chọn phòng khác.", "Thêm khách",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiPhongChoCombo(null);
                return;
            }

            var k = new KhachThue
            {
                HoTen = txtHoTen.Text.Trim(),
                GioiTinh = rdoNam.Checked ? "Nam" : "Nữ",
                NgaySinh = dtpNgaySinh.Value.Date,
                SoDienThoai = txtSDT.Text.Trim(),
                MaTinh = tinh.ma,
                TenTinh = tinh.ten,
                MaPhuongXa = xa.ma,
                TenPhuongXa = xa.ten,
                DaDangKyTamTru = chkTamTru.Checked,
                MaPhong = maPhong
            };
            db.KhachThues.Add(k);
            db.SaveChanges();

            CapNhatTrangThaiPhong(db, maPhong);   // phòng -> "Đang thuê"
            db.SaveChanges();
            tx.Commit();

            Log.Information("Them khach MaKhach={MaKhach}, MaPhong={MaPhong}", k.MaKhach, maPhong);
            HoanTat();
        }
        catch (Exception ex)
        {
            BaoLoiCSDL(ex, "thêm khách thuê");
        }
    }

    private void btnSua_Click(object sender, EventArgs e)
    {
        if (_maKhach == 0)
        {
            MessageBox.Show("Vui lòng bấm chọn một khách trong danh sách để sửa.", "Sửa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!KiemTra(out var tinh, out var xa, out var maPhong)) return;

        try
        {
            using var db = DbContextHelper.Create();
            using var tx = db.Database.BeginTransaction();

            var k = db.KhachThues.Find(_maKhach);
            if (k == null)
            {
                MessageBox.Show("Khách này không còn tồn tại (có thể đã bị xóa).", "Sửa",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HoanTat();
                return;
            }

            var phongCu = k.MaPhong;
            if (maPhong != phongCu)
            {
                var phongMoi = db.PhongTros.Find(maPhong);
                if (phongMoi == null || phongMoi.TinhTrang != Trong)
                {
                    MessageBox.Show("Phòng được chọn không còn trống.", "Sửa",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            k.HoTen = txtHoTen.Text.Trim();
            k.GioiTinh = rdoNam.Checked ? "Nam" : "Nữ";
            k.NgaySinh = dtpNgaySinh.Value.Date;
            k.SoDienThoai = txtSDT.Text.Trim();
            k.MaTinh = tinh.ma;
            k.TenTinh = tinh.ten;
            k.MaPhuongXa = xa.ma;
            k.TenPhuongXa = xa.ten;
            k.DaDangKyTamTru = chkTamTru.Checked;
            k.MaPhong = maPhong;
            db.SaveChanges();

            CapNhatTrangThaiPhong(db, phongCu, maPhong);   // phòng cũ có thể về "Trống", phòng mới -> "Đang thuê"
            db.SaveChanges();
            tx.Commit();

            Log.Information("Sua khach MaKhach={MaKhach}, MaPhong={MaPhong}", k.MaKhach, maPhong);
            HoanTat();
        }
        catch (Exception ex)
        {
            BaoLoiCSDL(ex, "sửa khách thuê");
        }
    }

    private void btnXoa_Click(object sender, EventArgs e)
    {
        if (_maKhach == 0)
        {
            MessageBox.Show("Vui lòng bấm chọn một khách trong danh sách để xóa.", "Xóa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var hoi = MessageBox.Show(
            $"Bạn có chắc muốn xóa khách \"{txtHoTen.Text.Trim()}\" không?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (hoi != DialogResult.Yes) return;

        try
        {
            using var db = DbContextHelper.Create();
            using var tx = db.Database.BeginTransaction();

            var k = db.KhachThues.Find(_maKhach);
            if (k != null)
            {
                var maPhong = k.MaPhong;
                db.KhachThues.Remove(k);
                db.SaveChanges();

                CapNhatTrangThaiPhong(db, maPhong);   // người cuối cùng rời đi -> "Trống"
                db.SaveChanges();
                tx.Commit();

                Log.Information("Xoa khach MaKhach={MaKhach}, MaPhong={MaPhong}", _maKhach, maPhong);
            }

            HoanTat();
        }
        catch (Exception ex)
        {
            BaoLoiCSDL(ex, "xóa khách thuê");
        }
    }

    private void btnLamMoi_Click(object sender, EventArgs e)
    {
        _suppressFilter = true;
        txtTim.Clear();
        _suppressFilter = false;

        HoanTat(xoaLoc: true);

        if (!_apiTinhOk)    // lần trước mất mạng -> thử tải lại danh sách tỉnh
            _ = TaiTinhAsync();
    }

    /// <summary>Sau khi thêm/sửa/xóa: xóa form, nạp lại ComboBox và lưới.</summary>
    private void HoanTat(bool xoaLoc = false)
    {
        try
        {
            _suppressFilter = true;
            XoaForm();
            TaiPhongChoCombo(null);
            TaiBoLoc();
            if (xoaLoc)
            {
                cboLocPhong.SelectedIndex = 0;
                cboLocTinh.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            BaoLoiCSDL(ex, "làm mới dữ liệu");
        }
        finally
        {
            _suppressFilter = false;
        }
        TaiDuLieu();
    }

    private void XoaForm()
    {
        _maKhach = 0;
        txtHoTen.Clear();
        rdoNam.Checked = true;
        dtpNgaySinh.Value = DateTime.Today.AddYears(-20);
        txtSDT.Clear();
        chkTamTru.Checked = false;

        _suppressAddr = true;
        if (_apiTinhOk) cboTinh.SelectedIndex = -1;
        else if (cboTinh.DropDownStyle == ComboBoxStyle.DropDown) cboTinh.Text = string.Empty;
        _suppressAddr = false;
        DatXaTrong();
    }

    // =====================================================================
    //  Tìm kiếm / lọc
    // =====================================================================

    private void btnTim_Click(object sender, EventArgs e) => TaiDuLieu();

    private void BoLocThayDoi(object sender, EventArgs e) => TaiDuLieu();

    private void txtTim_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            TaiDuLieu();
        }
    }

    private void btnBoLoc_Click(object sender, EventArgs e)
    {
        _suppressFilter = true;
        txtTim.Clear();
        if (cboLocPhong.Items.Count > 0) cboLocPhong.SelectedIndex = 0;
        if (cboLocTinh.Items.Count > 0) cboLocTinh.SelectedIndex = 0;
        _suppressFilter = false;
        TaiDuLieu();
    }

    // =====================================================================
    //  Nút ẩn "Gây lỗi thử" (bật/tắt bằng Ctrl + Shift + E)
    // =====================================================================

    private void FormKhachThue_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control && e.Shift && e.KeyCode == Keys.E)
        {
            btnGayLoi.Visible = !btnGayLoi.Visible;
            e.Handled = true;
        }
    }

    private void btnGayLoi_Click(object sender, EventArgs e) => ThucHienLoiThu();

    private static void ThucHienLoiThu()
    {
        // Ném lỗi không bắt -> Application.ThreadException -> Program.XuLyLoiChuaBat (ghi log + hộp thoại thân thiện)
        throw new InvalidOperationException("Lỗi thử nghiệm: người dùng bấm nút \"Gây lỗi thử\".");
    }
}
