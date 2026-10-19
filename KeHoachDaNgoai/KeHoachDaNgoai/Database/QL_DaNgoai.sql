IF DB_ID(N'QL_DaNgoai') IS NULL CREATE DATABASE QL_DaNgoai;
GO
USE QL_DaNgoai;
GO
IF OBJECT_ID('dbo.ChuyenDi','U') IS NOT NULL DROP TABLE dbo.ChuyenDi;
CREATE TABLE ChuyenDi(MaChuyen INT IDENTITY(1,1) PRIMARY KEY,TenChuyen NVARCHAR(100) NOT NULL,DiaDiem NVARCHAR(100) NOT NULL,NgayDi DATE NOT NULL,SoNguoi INT NOT NULL CHECK(SoNguoi>=1),NganSach DECIMAL(18,0) NOT NULL CHECK(NganSach>=0),PhuongTien NVARCHAR(30) NOT NULL,DaHoanThanh BIT NOT NULL DEFAULT 0,GhiChu NVARCHAR(200) NULL);
INSERT ChuyenDi(TenChuyen,DiaDiem,NgayDi,SoNguoi,NganSach,PhuongTien,DaHoanThanh,GhiChu) VALUES
(N'Khám phá sao hỏa',N'Mars',DATEADD(day,3,CAST(GETDATE() AS date)),4,4000000,N'xe tải',0,N'Săn mây'),
(N'Nghỉ dưỡng Đà Lạt',N'Da Lat',DATEADD(day,5,CAST(GETDATE() AS date)),3,2500000,N'tàu thủy',0,N'Đi biển'),
(N'Phố Thủy Tinh',N'Mercury',DATEADD(day,8,CAST(GETDATE() AS date)),5,6000000,N'Phi Thuyền',0,NULL),
(N'Sao thổ mùa đẹp',N'Jupiter',DATEADD(day,10,CAST(GETDATE() AS date)),2,5000000,N'Tên Lửa',0,NULL),
(N'Cần Thơ',N'Can Tho',DATEADD(day,12,CAST(GETDATE() AS date)),4,3500000,N'Xe Tăng',0,NULL),
(N'Thiên Tinh Kỳ Vực',N'GI',DATEADD(day,15,CAST(GETDATE() AS date)),3,3000000,N'Xe đạp',0,NULL),
(N'Cung Trăng',N'Cung Trang',DATEADD(day,18,CAST(GETDATE() AS date)),6,7000000,N'Xe khách',0,NULL),
(N'Vùng đất linh hồn',N'Hell',DATEADD(day,20,CAST(GETDATE() AS date)),2,2800000,N'xe điện đụng',0,NULL);
