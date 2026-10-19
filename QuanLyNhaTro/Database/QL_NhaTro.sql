/* =====================================================================
   Câu 2 - Quản lý nhà trọ
   Tạo CSDL QL_NhaTro, 2 bảng PhongTro / KhachThue (có khóa ngoại),
   6 phòng và 5 khách mẫu. Có thể chạy lại nhiều lần.
   Mã tỉnh / phường-xã lấy theo API https://provinces.open-api.vn/api/v2/
   ===================================================================== */
IF DB_ID(N'QL_NhaTro') IS NULL
    CREATE DATABASE QL_NhaTro;
GO

USE QL_NhaTro;
GO

IF OBJECT_ID(N'dbo.KhachThue', N'U') IS NOT NULL DROP TABLE dbo.KhachThue;
IF OBJECT_ID(N'dbo.PhongTro',  N'U') IS NOT NULL DROP TABLE dbo.PhongTro;
GO

CREATE TABLE dbo.PhongTro
(
    MaPhong   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PhongTro PRIMARY KEY,
    TenPhong  NVARCHAR(20)  NOT NULL,
    GiaThue   DECIMAL(18,0) NOT NULL,
    TinhTrang NVARCHAR(20)  NOT NULL CONSTRAINT DF_PhongTro_TinhTrang DEFAULT (N'Trống'),
    CONSTRAINT CK_PhongTro_TinhTrang CHECK (TinhTrang IN (N'Trống', N'Đang thuê'))
);
GO

CREATE TABLE dbo.KhachThue
(
    MaKhach        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhachThue PRIMARY KEY,
    HoTen          NVARCHAR(100) NOT NULL,
    GioiTinh       NVARCHAR(5)   NOT NULL,
    NgaySinh       DATE          NOT NULL,
    SoDienThoai    VARCHAR(15)   NOT NULL,
    MaTinh         INT           NULL,
    TenTinh        NVARCHAR(100) NULL,
    MaPhuongXa     INT           NULL,
    TenPhuongXa    NVARCHAR(100) NULL,
    DaDangKyTamTru BIT           NOT NULL CONSTRAINT DF_KhachThue_TamTru DEFAULT (0),
    MaPhong        INT           NOT NULL,
    CONSTRAINT FK_KhachThue_PhongTro FOREIGN KEY (MaPhong) REFERENCES dbo.PhongTro (MaPhong)
);
GO

INSERT INTO dbo.PhongTro (TenPhong, GiaThue, TinhTrang) VALUES
(N'A101', 2500000, N'Đang thuê'),
(N'A102', 2800000, N'Đang thuê'),
(N'B201', 3200000, N'Đang thuê'),
(N'B202', 3000000, N'Trống'),
(N'C301', 3500000, N'Đang thuê'),
(N'C302', 2700000, N'Trống');
GO

INSERT INTO dbo.KhachThue
    (HoTen, GioiTinh, NgaySinh, SoDienThoai,
     MaTinh, TenTinh, MaPhuongXa, TenPhuongXa,
     DaDangKyTamTru, MaPhong)
VALUES
(N'Võ Thị Thanh Trúc', N'Nữ', '2001-04-15', '0834567120',
 79, N'Tỉnh Khánh Hòa', 26734, N'Phường Nha Trang', 1,
 (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'A101')),

(N'Đặng Quốc Bảo', N'Nam', '2000-12-22', '0923456781',
 79, N'Tỉnh Khánh Hòa', 26737, N'Phường Bắc Nha Trang', 1,
 (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'A102')),

(N'Bùi Ngọc Ánh', N'Nữ', '2002-07-09', '0867891234',
 48, N'Thành phố Đà Nẵng', 20194, N'Phường Hải Châu', 0,
 (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'B201')),

(N'Phan Đức Thịnh', N'Nam', '1999-03-18', '0978123456',
 92, N'Tỉnh Cần Thơ', 31168, N'Phường Ninh Kiều', 1,
 (SELECT MaPhong FROM dbo.PhongTro WHERE TenPhong = N'C301'));
GO

SELECT * FROM dbo.PhongTro;
SELECT * FROM dbo.KhachThue;
GO
