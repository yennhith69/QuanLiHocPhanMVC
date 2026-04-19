/*
Script bo sung phan cong giao vien phu trach hoc phan
Chay tren database QuanLyHocPhan
*/

USE QuanLyHocPhan;
GO

IF OBJECT_ID('dbo.PhanCongHocPhan', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhanCongHocPhan
    (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDNguoiDung INT NOT NULL,
        IDHocPhan INT NOT NULL,
        NgayPhanCong DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

        CONSTRAINT FK_PhanCongHocPhan_NguoiDung
            FOREIGN KEY (IDNguoiDung) REFERENCES dbo.NguoiDung(ID),

        CONSTRAINT FK_PhanCongHocPhan_HocPhan
            FOREIGN KEY (IDHocPhan) REFERENCES dbo.HocPhan(IDHocPhan),

        CONSTRAINT UQ_PhanCongHocPhan UNIQUE (IDNguoiDung, IDHocPhan)
    );
END
GO

/*
Vi du du lieu phan cong:
- Moi giao vien duoc phan cong nhieu hoc phan
- Moi hoc phan co the co nhieu giao vien (neu can)
*/
INSERT INTO dbo.PhanCongHocPhan (IDNguoiDung, IDHocPhan)
SELECT nd.ID, hp.IDHocPhan
FROM dbo.NguoiDung nd
CROSS JOIN dbo.HocPhan hp
WHERE nd.Role = 'GiaoVien'
  AND hp.IDHocPhan IN (1, 2)
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.PhanCongHocPhan pc
      WHERE pc.IDNguoiDung = nd.ID
        AND pc.IDHocPhan = hp.IDHocPhan
  );
GO

/*
Kiem tra ket qua
*/
SELECT nd.Username, hp.MaHocPhan, hp.TenHocPhan, pc.NgayPhanCong
FROM dbo.PhanCongHocPhan pc
INNER JOIN dbo.NguoiDung nd ON nd.ID = pc.IDNguoiDung
INNER JOIN dbo.HocPhan hp ON hp.IDHocPhan = pc.IDHocPhan
ORDER BY nd.Username, hp.MaHocPhan;
GO
