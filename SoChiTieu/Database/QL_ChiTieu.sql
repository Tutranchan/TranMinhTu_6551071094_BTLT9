/* =====================================================================
   Câu 1 - Sổ chi tiêu cá nhân
   Tạo CSDL QL_ChiTieu, bảng ChiTieu và 12 dòng dữ liệu mẫu.
   Chạy trong SSMS (hoặc sqlcmd). Có thể chạy lại nhiều lần.
   ===================================================================== */
IF DB_ID(N'QL_ChiTieu') IS NULL
    CREATE DATABASE QL_ChiTieu;
GO

USE QL_ChiTieu;
GO

IF OBJECT_ID(N'dbo.ChiTieu', N'U') IS NOT NULL
    DROP TABLE dbo.ChiTieu;
GO

CREATE TABLE dbo.ChiTieu
(
    MaCT     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ChiTieu PRIMARY KEY,
    NgayChi  DATE           NOT NULL,
    NoiDung  NVARCHAR(100)  NOT NULL,
    LoaiChi  NVARCHAR(30)   NOT NULL,
    SoTien   DECIMAL(18,0)  NOT NULL CONSTRAINT CK_ChiTieu_SoTien CHECK (SoTien > 0),
    GhiChu   NVARCHAR(200)  NULL
);
GO

DECLARE @h DATE = CAST(GETDATE() AS DATE);

INSERT INTO dbo.ChiTieu (NgayChi, NoiDung, LoaiChi, SoTien, GhiChu)
VALUES
(DATEADD(DAY, -11, @h), N'Cơm trưa', N'Ăn uống', 35000, NULL),
(DATEADD(DAY, -10, @h), N'Xăng xe', N'Đi lại', 60000, N'Đổ đầy bình'),
(DATEADD(DAY, -9, @h), N'Photo giáo trình', N'Học tập', 25000, NULL),
(DATEADD(DAY, -8, @h), N'Xem phim cuối tuần', N'Giải trí', 90000, N'Đi cùng nhóm bạn'),
(DATEADD(DAY, -7, @h), N'Trà sữa', N'Ăn uống', 40000, NULL),
(DATEADD(DAY, -6, @h), N'Vé xe buýt tháng', N'Đi lại', 140000, NULL),
(DATEADD(DAY, -5, @h), N'Mua sách lập trình C#', N'Học tập', 180000, N'Sách tham khảo'),
(DATEADD(DAY, -4, @h), N'Ăn sáng', N'Ăn uống', 25000, NULL),
(DATEADD(DAY, -3, @h), N'Chơi game / nạp thẻ', N'Giải trí', 100000, NULL),
(DATEADD(DAY, -2, @h), N'Mua dụng cụ học tập', N'Khác', 75000, NULL),
(DATEADD(DAY, -1, @h), N'Cà phê với bạn', N'Ăn uống', 45000, NULL),
(@h, N'Gửi xe', N'Đi lại', 5000, NULL);
GO

SELECT * FROM dbo.ChiTieu ORDER BY NgayChi, MaCT;
GO
