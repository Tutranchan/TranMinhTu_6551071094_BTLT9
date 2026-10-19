namespace QuanLyNhaTro
{
    partial class FormKhachThue
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpThongTin = new System.Windows.Forms.GroupBox();
            this.lblHoTen = new System.Windows.Forms.Label();
            this.txtHoTen = new System.Windows.Forms.TextBox();
            this.grpGioiTinh = new System.Windows.Forms.GroupBox();
            this.rdoNam = new System.Windows.Forms.RadioButton();
            this.rdoNu = new System.Windows.Forms.RadioButton();
            this.lblNgaySinh = new System.Windows.Forms.Label();
            this.dtpNgaySinh = new System.Windows.Forms.DateTimePicker();
            this.lblSDT = new System.Windows.Forms.Label();
            this.txtSDT = new System.Windows.Forms.TextBox();
            this.lblPhong = new System.Windows.Forms.Label();
            this.cboPhong = new System.Windows.Forms.ComboBox();
            this.lblTinh = new System.Windows.Forms.Label();
            this.cboTinh = new System.Windows.Forms.ComboBox();
            this.lblXa = new System.Windows.Forms.Label();
            this.cboXa = new System.Windows.Forms.ComboBox();
            this.chkTamTru = new System.Windows.Forms.CheckBox();
            this.btnThem = new System.Windows.Forms.Button();
            this.btnSua = new System.Windows.Forms.Button();
            this.btnXoa = new System.Windows.Forms.Button();
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.btnGayLoi = new System.Windows.Forms.Button();
            this.txtTim = new System.Windows.Forms.TextBox();
            this.btnTim = new System.Windows.Forms.Button();
            this.lblLocPhong = new System.Windows.Forms.Label();
            this.cboLocPhong = new System.Windows.Forms.ComboBox();
            this.lblLocTinh = new System.Windows.Forms.Label();
            this.cboLocTinh = new System.Windows.Forms.ComboBox();
            this.btnBoLoc = new System.Windows.Forms.Button();
            this.dgvKhach = new System.Windows.Forms.DataGridView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tslTrangThai = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpThongTin.SuspendLayout();
            this.grpGioiTinh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKhach)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // grpThongTin
            //
            this.grpThongTin.Controls.Add(this.lblHoTen);
            this.grpThongTin.Controls.Add(this.txtHoTen);
            this.grpThongTin.Controls.Add(this.grpGioiTinh);
            this.grpThongTin.Controls.Add(this.lblNgaySinh);
            this.grpThongTin.Controls.Add(this.dtpNgaySinh);
            this.grpThongTin.Controls.Add(this.lblSDT);
            this.grpThongTin.Controls.Add(this.txtSDT);
            this.grpThongTin.Controls.Add(this.lblPhong);
            this.grpThongTin.Controls.Add(this.cboPhong);
            this.grpThongTin.Controls.Add(this.lblTinh);
            this.grpThongTin.Controls.Add(this.cboTinh);
            this.grpThongTin.Controls.Add(this.lblXa);
            this.grpThongTin.Controls.Add(this.cboXa);
            this.grpThongTin.Controls.Add(this.chkTamTru);
            this.grpThongTin.Controls.Add(this.btnThem);
            this.grpThongTin.Controls.Add(this.btnSua);
            this.grpThongTin.Controls.Add(this.btnXoa);
            this.grpThongTin.Controls.Add(this.btnLamMoi);
            this.grpThongTin.Controls.Add(this.btnGayLoi);
            this.grpThongTin.Location = new System.Drawing.Point(10, 8);
            this.grpThongTin.Name = "grpThongTin";
            this.grpThongTin.Size = new System.Drawing.Size(400, 450);
            this.grpThongTin.TabIndex = 0;
            this.grpThongTin.TabStop = false;
            this.grpThongTin.Text = "Thông tin khách thuê";
            //
            // lblHoTen
            //
            this.lblHoTen.AutoSize = true;
            this.lblHoTen.Location = new System.Drawing.Point(15, 35);
            this.lblHoTen.Name = "lblHoTen";
            this.lblHoTen.Text = "Họ tên";
            //
            // txtHoTen
            //
            this.txtHoTen.Location = new System.Drawing.Point(130, 31);
            this.txtHoTen.MaxLength = 100;
            this.txtHoTen.Name = "txtHoTen";
            this.txtHoTen.Size = new System.Drawing.Size(250, 23);
            this.txtHoTen.TabIndex = 0;
            //
            // grpGioiTinh
            //
            this.grpGioiTinh.Controls.Add(this.rdoNam);
            this.grpGioiTinh.Controls.Add(this.rdoNu);
            this.grpGioiTinh.Location = new System.Drawing.Point(15, 62);
            this.grpGioiTinh.Name = "grpGioiTinh";
            this.grpGioiTinh.Size = new System.Drawing.Size(365, 46);
            this.grpGioiTinh.TabIndex = 1;
            this.grpGioiTinh.TabStop = false;
            this.grpGioiTinh.Text = "Giới tính";
            //
            // rdoNam
            //
            this.rdoNam.AutoSize = true;
            this.rdoNam.Checked = true;
            this.rdoNam.Location = new System.Drawing.Point(115, 18);
            this.rdoNam.Name = "rdoNam";
            this.rdoNam.TabIndex = 0;
            this.rdoNam.TabStop = true;
            this.rdoNam.Text = "Nam";
            this.rdoNam.UseVisualStyleBackColor = true;
            //
            // rdoNu
            //
            this.rdoNu.AutoSize = true;
            this.rdoNu.Location = new System.Drawing.Point(215, 18);
            this.rdoNu.Name = "rdoNu";
            this.rdoNu.TabIndex = 1;
            this.rdoNu.Text = "Nữ";
            this.rdoNu.UseVisualStyleBackColor = true;
            //
            // lblNgaySinh
            //
            this.lblNgaySinh.AutoSize = true;
            this.lblNgaySinh.Location = new System.Drawing.Point(15, 125);
            this.lblNgaySinh.Name = "lblNgaySinh";
            this.lblNgaySinh.Text = "Ngày sinh";
            //
            // dtpNgaySinh
            //
            this.dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
            this.dtpNgaySinh.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpNgaySinh.Location = new System.Drawing.Point(130, 121);
            this.dtpNgaySinh.Name = "dtpNgaySinh";
            this.dtpNgaySinh.Size = new System.Drawing.Size(150, 23);
            this.dtpNgaySinh.TabIndex = 2;
            //
            // lblSDT
            //
            this.lblSDT.AutoSize = true;
            this.lblSDT.Location = new System.Drawing.Point(15, 163);
            this.lblSDT.Name = "lblSDT";
            this.lblSDT.Text = "Số điện thoại";
            //
            // txtSDT
            //
            this.txtSDT.Location = new System.Drawing.Point(130, 159);
            this.txtSDT.MaxLength = 15;
            this.txtSDT.Name = "txtSDT";
            this.txtSDT.Size = new System.Drawing.Size(250, 23);
            this.txtSDT.TabIndex = 3;
            //
            // lblPhong
            //
            this.lblPhong.AutoSize = true;
            this.lblPhong.Location = new System.Drawing.Point(15, 201);
            this.lblPhong.Name = "lblPhong";
            this.lblPhong.Text = "Phòng";
            //
            // cboPhong
            //
            this.cboPhong.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPhong.Location = new System.Drawing.Point(130, 197);
            this.cboPhong.Name = "cboPhong";
            this.cboPhong.Size = new System.Drawing.Size(250, 23);
            this.cboPhong.TabIndex = 4;
            //
            // lblTinh
            //
            this.lblTinh.AutoSize = true;
            this.lblTinh.Location = new System.Drawing.Point(15, 239);
            this.lblTinh.Name = "lblTinh";
            this.lblTinh.Text = "Tỉnh/Thành";
            //
            // cboTinh
            //
            this.cboTinh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTinh.Location = new System.Drawing.Point(130, 235);
            this.cboTinh.MaxLength = 100;
            this.cboTinh.Name = "cboTinh";
            this.cboTinh.Size = new System.Drawing.Size(250, 23);
            this.cboTinh.TabIndex = 5;
            this.cboTinh.SelectedIndexChanged += new System.EventHandler(this.cboTinh_SelectedIndexChanged);
            //
            // lblXa
            //
            this.lblXa.AutoSize = true;
            this.lblXa.Location = new System.Drawing.Point(15, 277);
            this.lblXa.Name = "lblXa";
            this.lblXa.Text = "Phường/Xã";
            //
            // cboXa
            //
            this.cboXa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboXa.Location = new System.Drawing.Point(130, 273);
            this.cboXa.MaxLength = 100;
            this.cboXa.Name = "cboXa";
            this.cboXa.Size = new System.Drawing.Size(250, 23);
            this.cboXa.TabIndex = 6;
            //
            // chkTamTru
            //
            this.chkTamTru.AutoSize = true;
            this.chkTamTru.Location = new System.Drawing.Point(130, 313);
            this.chkTamTru.Name = "chkTamTru";
            this.chkTamTru.TabIndex = 7;
            this.chkTamTru.Text = "Đã đăng ký tạm trú";
            this.chkTamTru.UseVisualStyleBackColor = true;
            //
            // btnThem
            //
            this.btnThem.Location = new System.Drawing.Point(15, 355);
            this.btnThem.Name = "btnThem";
            this.btnThem.Size = new System.Drawing.Size(85, 32);
            this.btnThem.TabIndex = 8;
            this.btnThem.Text = "Thêm";
            this.btnThem.UseVisualStyleBackColor = true;
            this.btnThem.Click += new System.EventHandler(this.btnThem_Click);
            //
            // btnSua
            //
            this.btnSua.Location = new System.Drawing.Point(105, 355);
            this.btnSua.Name = "btnSua";
            this.btnSua.Size = new System.Drawing.Size(85, 32);
            this.btnSua.TabIndex = 9;
            this.btnSua.Text = "Sửa";
            this.btnSua.UseVisualStyleBackColor = true;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            //
            // btnXoa
            //
            this.btnXoa.Location = new System.Drawing.Point(195, 355);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(85, 32);
            this.btnXoa.TabIndex = 10;
            this.btnXoa.Text = "Xóa";
            this.btnXoa.UseVisualStyleBackColor = true;
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            //
            // btnLamMoi
            //
            this.btnLamMoi.Location = new System.Drawing.Point(285, 355);
            this.btnLamMoi.Name = "btnLamMoi";
            this.btnLamMoi.Size = new System.Drawing.Size(85, 32);
            this.btnLamMoi.TabIndex = 11;
            this.btnLamMoi.Text = "Làm mới";
            this.btnLamMoi.UseVisualStyleBackColor = true;
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            //
            // btnGayLoi
            //
            this.btnGayLoi.ForeColor = System.Drawing.Color.Firebrick;
            this.btnGayLoi.Location = new System.Drawing.Point(15, 405);
            this.btnGayLoi.Name = "btnGayLoi";
            this.btnGayLoi.Size = new System.Drawing.Size(355, 28);
            this.btnGayLoi.TabIndex = 12;
            this.btnGayLoi.Text = "Gây lỗi thử";
            this.btnGayLoi.UseVisualStyleBackColor = true;
            this.btnGayLoi.Visible = false;
            this.btnGayLoi.Click += new System.EventHandler(this.btnGayLoi_Click);
            //
            // txtTim
            //
            this.txtTim.Location = new System.Drawing.Point(425, 12);
            this.txtTim.Name = "txtTim";
            this.txtTim.PlaceholderText = "Tìm theo tên hoặc SĐT";
            this.txtTim.Size = new System.Drawing.Size(270, 23);
            this.txtTim.TabIndex = 1;
            this.txtTim.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTim_KeyDown);
            //
            // btnTim
            //
            this.btnTim.Location = new System.Drawing.Point(705, 10);
            this.btnTim.Name = "btnTim";
            this.btnTim.Size = new System.Drawing.Size(80, 27);
            this.btnTim.TabIndex = 2;
            this.btnTim.Text = "Tìm";
            this.btnTim.UseVisualStyleBackColor = true;
            this.btnTim.Click += new System.EventHandler(this.btnTim_Click);
            //
            // lblLocPhong
            //
            this.lblLocPhong.AutoSize = true;
            this.lblLocPhong.Location = new System.Drawing.Point(425, 52);
            this.lblLocPhong.Name = "lblLocPhong";
            this.lblLocPhong.Text = "Phòng:";
            //
            // cboLocPhong
            //
            this.cboLocPhong.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLocPhong.Location = new System.Drawing.Point(480, 48);
            this.cboLocPhong.Name = "cboLocPhong";
            this.cboLocPhong.Size = new System.Drawing.Size(120, 23);
            this.cboLocPhong.TabIndex = 3;
            this.cboLocPhong.SelectedIndexChanged += new System.EventHandler(this.BoLocThayDoi);
            //
            // lblLocTinh
            //
            this.lblLocTinh.AutoSize = true;
            this.lblLocTinh.Location = new System.Drawing.Point(615, 52);
            this.lblLocTinh.Name = "lblLocTinh";
            this.lblLocTinh.Text = "Tỉnh/Thành:";
            //
            // cboLocTinh
            //
            this.cboLocTinh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLocTinh.Location = new System.Drawing.Point(695, 48);
            this.cboLocTinh.Name = "cboLocTinh";
            this.cboLocTinh.Size = new System.Drawing.Size(200, 23);
            this.cboLocTinh.TabIndex = 4;
            this.cboLocTinh.SelectedIndexChanged += new System.EventHandler(this.BoLocThayDoi);
            //
            // btnBoLoc
            //
            this.btnBoLoc.Location = new System.Drawing.Point(905, 46);
            this.btnBoLoc.Name = "btnBoLoc";
            this.btnBoLoc.Size = new System.Drawing.Size(90, 27);
            this.btnBoLoc.TabIndex = 5;
            this.btnBoLoc.Text = "Bỏ lọc";
            this.btnBoLoc.UseVisualStyleBackColor = true;
            this.btnBoLoc.Click += new System.EventHandler(this.btnBoLoc_Click);
            //
            // dgvKhach
            //
            this.dgvKhach.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvKhach.Location = new System.Drawing.Point(425, 84);
            this.dgvKhach.Name = "dgvKhach";
            this.dgvKhach.Size = new System.Drawing.Size(685, 500);
            this.dgvKhach.TabIndex = 6;
            this.dgvKhach.SelectionChanged += new System.EventHandler(this.dgvKhach_SelectionChanged);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.tslTrangThai });
            this.statusStrip.Location = new System.Drawing.Point(0, 598);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1120, 22);
            this.statusStrip.TabIndex = 7;
            //
            // tslTrangThai
            //
            this.tslTrangThai.Name = "tslTrangThai";
            this.tslTrangThai.Text = "Sẵn sàng";
            //
            // FormKhachThue
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 620);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.dgvKhach);
            this.Controls.Add(this.btnBoLoc);
            this.Controls.Add(this.cboLocTinh);
            this.Controls.Add(this.lblLocTinh);
            this.Controls.Add(this.cboLocPhong);
            this.Controls.Add(this.lblLocPhong);
            this.Controls.Add(this.btnTim);
            this.Controls.Add(this.txtTim);
            this.Controls.Add(this.grpThongTin);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1136, 659);
            this.Name = "FormKhachThue";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý nhà trọ - Khách thuê";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormKhachThue_KeyDown);
            this.grpThongTin.ResumeLayout(false);
            this.grpThongTin.PerformLayout();
            this.grpGioiTinh.ResumeLayout(false);
            this.grpGioiTinh.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKhach)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpThongTin;
        private System.Windows.Forms.Label lblHoTen;
        private System.Windows.Forms.TextBox txtHoTen;
        private System.Windows.Forms.GroupBox grpGioiTinh;
        private System.Windows.Forms.RadioButton rdoNam;
        private System.Windows.Forms.RadioButton rdoNu;
        private System.Windows.Forms.Label lblNgaySinh;
        private System.Windows.Forms.DateTimePicker dtpNgaySinh;
        private System.Windows.Forms.Label lblSDT;
        private System.Windows.Forms.TextBox txtSDT;
        private System.Windows.Forms.Label lblPhong;
        private System.Windows.Forms.ComboBox cboPhong;
        private System.Windows.Forms.Label lblTinh;
        private System.Windows.Forms.ComboBox cboTinh;
        private System.Windows.Forms.Label lblXa;
        private System.Windows.Forms.ComboBox cboXa;
        private System.Windows.Forms.CheckBox chkTamTru;
        private System.Windows.Forms.Button btnThem;
        private System.Windows.Forms.Button btnSua;
        private System.Windows.Forms.Button btnXoa;
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Button btnGayLoi;
        private System.Windows.Forms.TextBox txtTim;
        private System.Windows.Forms.Button btnTim;
        private System.Windows.Forms.Label lblLocPhong;
        private System.Windows.Forms.ComboBox cboLocPhong;
        private System.Windows.Forms.Label lblLocTinh;
        private System.Windows.Forms.ComboBox cboLocTinh;
        private System.Windows.Forms.Button btnBoLoc;
        private System.Windows.Forms.DataGridView dgvKhach;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tslTrangThai;
    }
}
