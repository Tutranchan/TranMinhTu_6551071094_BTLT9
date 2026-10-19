using System.Globalization;
using Serilog;
using SoChiTieu.Helpers;
using SoChiTieu.Models;

namespace SoChiTieu;

public partial class FormChiTieu : Form
{
    private static readonly CultureInfo Vi = new("vi-VN");
    private static readonly string[] DanhSachLoaiChi = { "Ăn uống", "Đi lại", "Học tập", "Giải trí", "Khác" };
    private const string TatCa = "Tất cả";

    private bool _loading = true;   // true: bỏ qua các sự kiện thay đổi trong lúc khởi tạo / nạp dữ liệu
    private int _maCT;              // MaCT của dòng đang chọn (0 = chưa chọn)

    public FormChiTieu()
    {
        InitializeComponent();
        CauHinhLuoi();
        CauHinhControl();
        Load += (s, e) => LamMoi();
    }

    // ---------------------------------------------------------------- Cấu hình giao diện

    private void CauHinhControl()
    {
        cboLoaiChi.Items.AddRange(DanhSachLoaiChi);
        cboLoaiChi.SelectedIndex = 0;

        cboLocLoai.Items.Add(TatCa);
        cboLocLoai.Items.AddRange(DanhSachLoaiChi);
        cboLocLoai.SelectedIndex = 0;
    }

    private void CauHinhLuoi()
    {
        var g = dgvChiTieu;
        g.AutoGenerateColumns = false;
        g.ReadOnly = true;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.MultiSelect = false;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        g.BackgroundColor = Color.White;
        g.RowHeadersWidth = 24;

        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "MaCT", HeaderText = "Mã", FillWeight = 40,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "000", Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "NgayChi", HeaderText = "Ngày chi", FillWeight = 70,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy", Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "NoiDung", HeaderText = "Nội dung", FillWeight = 140
        });
        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "LoaiChi", HeaderText = "Loại chi", FillWeight = 70
        });
        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "SoTien", HeaderText = "Số tiền", FillWeight = 80,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", FormatProvider = Vi, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        g.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "GhiChu", HeaderText = "Ghi chú", FillWeight = 140
        });
    }

    // ---------------------------------------------------------------- Nạp dữ liệu / tìm kiếm

    /// <summary>Đưa form về trạng thái ban đầu: xóa ô nhập, bộ lọc = tháng này, tải lại lưới.</summary>
    private void LamMoi()
    {
        _loading = true;
        XoaForm();
        txtTimNoiDung.Clear();
        cboLocLoai.SelectedIndex = 0;
        var homNay = DateTime.Today;
        dtpTuNgay.Value = new DateTime(homNay.Year, homNay.Month, 1);
        dtpDenNgay.Value = homNay;
        _loading = false;
        TaiDuLieu();
    }

    private void XoaForm()
    {
        _maCT = 0;
        dtpNgayChi.Value = DateTime.Today;
        txtNoiDung.Clear();
        cboLoaiChi.SelectedIndex = 0;
        nudSoTien.Value = 0;
        txtGhiChu.Clear();
    }

    private List<ChiTieu> TimKiem()
    {
        using var db = DbContextHelper.Create();

        var tuNgay = dtpTuNgay.Value.Date;
        var denNgay = dtpDenNgay.Value.Date;
        var tuKhoa = txtTimNoiDung.Text.Trim();

        var q = db.ChiTieus.AsQueryable()
                           .Where(x => x.NgayChi >= tuNgay && x.NgayChi <= denNgay);

        if (tuKhoa.Length > 0)
            q = q.Where(x => x.NoiDung.Contains(tuKhoa));

        if (cboLocLoai.SelectedIndex > 0)
        {
            var loai = cboLocLoai.SelectedItem.ToString();
            q = q.Where(x => x.LoaiChi == loai);
        }

        return q.OrderBy(x => x.NgayChi).ThenBy(x => x.MaCT).ToList();
    }

    /// <summary>Tải lại lưới theo bộ lọc hiện tại và cập nhật Label tổng chi.</summary>
    private void TaiDuLieu()
    {
        if (_loading) return;

        if (dtpTuNgay.Value.Date > dtpDenNgay.Value.Date)
        {
            MessageBox.Show("\"Từ ngày\" không được lớn hơn \"Đến ngày\".", "Tìm kiếm",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var ds = TimKiem();

            _loading = true;
            dgvChiTieu.DataSource = ds;
            dgvChiTieu.ClearSelection();
            _loading = false;

            var tong = ds.Sum(x => x.SoTien);
            lblTongChi.Text = $"Tổng chi: {tong.ToString("N0", Vi)} đ";
        }
        catch (Exception ex)
        {
            _loading = false;
            BaoLoiCSDL(ex, "tải danh sách chi tiêu");
        }
    }

    // ---------------------------------------------------------------- Kiểm tra hợp lệ / báo lỗi

    private bool KiemTraHopLe()
    {
        if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
            return Canh("Nội dung không được để trống.", txtNoiDung);

        if (nudSoTien.Value <= 0)
            return Canh("Số tiền phải lớn hơn 0.", nudSoTien);

        if (dtpNgayChi.Value.Date > DateTime.Today)
            return Canh("Ngày chi không được lớn hơn hôm nay.", dtpNgayChi);

        if (cboLoaiChi.SelectedIndex < 0)
            return Canh("Vui lòng chọn loại chi.", cboLoaiChi);

        return true;
    }

    private static bool Canh(string noiDung, Control control)
    {
        MessageBox.Show(noiDung, "Dữ liệu chưa hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
        return false;
    }

    /// <summary>Ghi log mức Error (kèm stack trace) và báo cho người dùng một thông báo thân thiện.</summary>
    private static void BaoLoiCSDL(Exception ex, string hanhDong)
    {
        Log.Error(ex, "Loi CSDL khi {HanhDong}", hanhDong);
        MessageBox.Show(
            "Không thể truy cập cơ sở dữ liệu. Vui lòng kiểm tra kết nối rồi thử lại.\n(Chi tiết đã được ghi vào file log.)",
            "Sổ chi tiêu cá nhân", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // ---------------------------------------------------------------- Chọn dòng -> đổ lên control

    private void dgvChiTieu_SelectionChanged(object sender, EventArgs e)
    {
        if (_loading) return;
        var row = dgvChiTieu.CurrentRow;
        if (row == null || !row.Selected) return;
        if (row.DataBoundItem is not ChiTieu ct) return;

        _maCT = ct.MaCT;
        dtpNgayChi.Value = ct.NgayChi;
        txtNoiDung.Text = ct.NoiDung;

        var i = cboLoaiChi.Items.IndexOf(ct.LoaiChi);
        cboLoaiChi.SelectedIndex = i >= 0 ? i : cboLoaiChi.Items.Count - 1;   // loại lạ -> "Khác"

        nudSoTien.Value = Math.Min(ct.SoTien, nudSoTien.Maximum);
        txtGhiChu.Text = ct.GhiChu ?? string.Empty;
    }

    // ---------------------------------------------------------------- CRUD

    private void btnThem_Click(object sender, EventArgs e)
    {
        if (!KiemTraHopLe()) return;

        try
        {
            using var db = DbContextHelper.Create();
            var ct = new ChiTieu
            {
                NgayChi = dtpNgayChi.Value.Date,
                NoiDung = txtNoiDung.Text.Trim(),
                LoaiChi = cboLoaiChi.Text,
                SoTien = nudSoTien.Value,
                GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
            };
            db.ChiTieus.Add(ct);
            db.SaveChanges();

            Log.Information("Them chi tieu MaCT={MaCT}, SoTien={SoTien}", ct.MaCT, ct.SoTien);

            XoaForm();
            TaiDuLieu();
        }
        catch (Exception ex)
        {
            BaoLoiCSDL(ex, "thêm khoản chi");
        }
    }

    private void btnSua_Click(object sender, EventArgs e)
    {
        if (_maCT == 0)
        {
            MessageBox.Show("Vui lòng bấm chọn một dòng trong danh sách để sửa.", "Sửa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (!KiemTraHopLe()) return;

        try
        {
            using var db = DbContextHelper.Create();
            var ct = db.ChiTieus.Find(_maCT);
            if (ct == null)
            {
                MessageBox.Show("Khoản chi này không còn tồn tại (có thể đã bị xóa).", "Sửa",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                XoaForm();
                TaiDuLieu();
                return;
            }

            ct.NgayChi = dtpNgayChi.Value.Date;
            ct.NoiDung = txtNoiDung.Text.Trim();
            ct.LoaiChi = cboLoaiChi.Text;
            ct.SoTien = nudSoTien.Value;
            ct.GhiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim();
            db.SaveChanges();

            Log.Information("Sua chi tieu MaCT={MaCT}, SoTien={SoTien}", ct.MaCT, ct.SoTien);

            XoaForm();
            TaiDuLieu();
        }
        catch (Exception ex)
        {
            BaoLoiCSDL(ex, "sửa khoản chi");
        }
    }

    private void btnXoa_Click(object sender, EventArgs e)
    {
        if (_maCT == 0)
        {
            MessageBox.Show("Vui lòng bấm chọn một dòng trong danh sách để xóa.", "Xóa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var hoi = MessageBox.Show(
            $"Bạn có chắc muốn xóa khoản chi \"{txtNoiDung.Text.Trim()}\" không?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        if (hoi != DialogResult.Yes) return;

        try
        {
            using var db = DbContextHelper.Create();
            var ct = db.ChiTieus.Find(_maCT);
            if (ct != null)
            {
                db.ChiTieus.Remove(ct);
                db.SaveChanges();
                Log.Information("Xoa chi tieu MaCT={MaCT}", _maCT);
            }

            XoaForm();
            TaiDuLieu();
        }
        catch (Exception ex)
        {
            BaoLoiCSDL(ex, "xóa khoản chi");
        }
    }

    private void btnLamMoi_Click(object sender, EventArgs e) => LamMoi();

    // ---------------------------------------------------------------- Sự kiện tìm kiếm / lọc

    private void btnTim_Click(object sender, EventArgs e) => TaiDuLieu();

    private void BoLocThayDoi(object sender, EventArgs e) => TaiDuLieu();

    private void txtTimNoiDung_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            TaiDuLieu();
        }
    }
}
