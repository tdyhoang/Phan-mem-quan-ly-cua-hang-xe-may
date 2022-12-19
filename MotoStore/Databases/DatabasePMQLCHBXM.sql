/*Cơ sở dữ liệu cho Ứng dụng Quản Lý Cửa Hàng Bán Xe Máy*/
CREATE DATABASE QLYCHBANXEMAY

use QLYCHBANXEMAY
set dateformat dmy

Create table KhachHang
(
       MaKH uniqueidentifier DEFAULT newid(),
	   HoTenKH NVARCHAR(30),
	   NgSinh smalldatetime,
	   GioiTinh NVARCHAR(3),
	   DiaChi NVARCHAR(40),
	   SDT  VARCHAR(10),
	   Email VARCHAR(30),
	   LoaiKH NVARCHAR(10),         
	   constraint PK_MaKH primary key(MAKH)
	   /*Vip - Dc Giam Gia 15%
	     Than Quen - Dc Giam Gia 5%
	     Thuong - Ko Dc Giam Gia*/
)

alter table KhachHang add constraint Check_GT check(GioiTinh='Nam' or GioiTinh=N'Nữ')
alter table KhachHang add constraint Check_LoaiKH check(LoaiKH='Vip' or LoaiKH=N'Thường' or LoaiKH=N'Thân quen')

Insert into KhachHang values('BDA8A3CC-9116-4215-99AB-351EF43154F5',N'Nguyễn Văn A','11/9/1979','Nam',N'34/34B Nguyễn Trãi, Q1, TpHCM','0876890495','NguyenVanA79@gmail.com','Vip')
Insert into KhachHang values('63D8101B-5CA4-4E9E-92C5-3859A071F9E9',N'Nguyễn Văn B','9/11/1997','Nam',N'873 Lê Hồng Phong, Q5, TpHCM','0867850847','NguyenVanB97@gmail.com',N'Thường')
Insert into KhachHang values('1B051393-0949-4E96-959A-6B001F82B177',N'Trần Ngọc Hân','22/6/2000',N'Nữ',N'23/5 Nguyễn Trãi, Q5, TpHCM','0876890495','HanTran2k@gmail.com',N'Thân quen')
Insert into KhachHang values('F7EED837-719A-4AF5-A6FB-8F167847A2F7',N'Nguyễn Văn Tâm','6/4/1971','Nam',N'50/34 Lê Đại Hành, Q10, TpHCM','0917325476','Tamnguyen71@gmail.com',N'Thường')
Insert into KhachHang values('955A14F5-F895-4F0B-B693-CE24A0B11321',N'Phan Thị Thanh','31/12/1981',N'Nữ',N'34 Trương Định, Q3, TpHCM','0948531923','ThanhTP311281@gmail.com',N'Thân quen')
Insert into KhachHang values('8507F1FB-C2CC-48AF-B285-DC15AFF4EC77',N'Đỗ Ngọc Khải','4/3/2001','Nam',N'215 Nguyễn Xiển, Q9, TpHCM','0876180684','DNKSBTC@gmail.com',N'Thường')


Create table NhanVien
(
      MaNV uniqueidentifier DEFAULT newid(),
	  HoTenNV NVarchar(30),
	  NgSinh smalldatetime,
	  GioiTinh NVarChar(3),
	  DiaChi   NVarchar(40),
	  SDT VARCHAR(10),
	  Email  Varchar(30),
	  ChucVu Nvarchar(10),
	  NgVL smalldatetime,     /*Ngày vào làm để hiện thêm vài thông tin ở trang chính*/
	  Luong money,
	  Thuong money,
	  constraint PK_MaNV primary key(MANV)
)

alter table NhanVien add constraint CK_GTNV check(GioiTinh = 'Nam' or GioiTinh=N'Nữ')

Insert into NhanVien values('59ACED03-FF2D-4DAA-A8EF-273B3685F468',N'Trần Đại Nghĩa','1/12/1989','Nam',N'65/19 Cao Bá Quát, Q8, TpHCM','0911228313','NghiaTDai1289@gmail.com',N'Sửa Xe','3/2/2018',7000000,3000000)
Insert into NhanVien values('90B6193C-0EBD-43F2-82A3-2DA131D767C6',N'Ngô Thanh Tuấn','12/6/1980','Nam',N'45 Nguyễn Cảnh Chân, Q1, TpHCM','0914102345','TuanNgoTh@gmail.com',N'Sửa Xe','10/2/2018',8500000,4000000)
Insert into NhanVien values('A25A94A7-6765-4D4E-BAD9-2E7A764C9742',N'Cao Thái Quý','12/1/1995','Nam',N'32/3 Trần Bình Trọng, Q5, TpHCM','0913476343','QuyCao95@gmail.com',N'Sửa Xe','11/5/2021',5000000,2000000)
Insert into NhanVien values('BDD24832-B4B9-4195-962C-91909DDE76C6',N'Lê Hoài Thương','27/7/1998',N'Nữ',N'65/19 Cao Bá Quát, Q8, TpHCM','0911228313','ThuongLe98Hoai@gmail.com',N'Văn Phòng','26/3/2021',5500000,2500000)
Insert into NhanVien values('89250069-C6CC-4D5D-BB9F-AA298DCE0D67',N'Nguyễn Bá Sang','11/8/1996','Nam',N'23 Hoàng Diệu, Q7, TpHCM','0856910975','SangNBa11896@gmail.com',N'Văn Phòng','3/2/2018',6000000,2500000)
Insert into NhanVien values('A5D54EA6-F80B-4A22-979B-D9FCB571FAB1',N'Phan Tấn Trung','26/2/1989','Nam',N'125/2 Hòa Hưng, Phuờng 12, Q10, TpHCM','0876701812','phantantrung3rb@gmail.com',N'Quản Lý','3/2/2018',12000000,3500000) /*Người quản lý*/


create table NhaSanXuat
(
     TenNSX varchar(15),
	 SDT  varchar(15),
	 Email varchar(45),
	 NuocSX NVarchar(15),
	 constraint PK_NSX primary key(TenNSX,NuocSX)
)


Insert into NhaSanXuat values('Yamaha','(30.24)38855080','YamahaMotorJapan@gmail.com',N'Nhật Bản')
Insert into NhaSanXuat values('Yamaha','(84.24)38855080','YamahaMotorVietNamCNVP@gmail.com',N'Việt Nam')
Insert into NhaSanXuat values('Suzuki','(84.89)47435209','SuzukiMotorVietNam@gmail.com',N'Việt Nam')
Insert into NhaSanXuat values('Honda','(84.2)18943170','HondaMotorThaiLand@gmail.com',N'Thái Lan')
Insert into NhaSanXuat values('Honda','(30.2)18943170','HondaMotorJapan@gmail.com',N'Nhật Bản')


create table MatHang
(
    MaMH uniqueidentifier DEFAULT newid(),
	TenMH varchar(15),
	GiaNhapMH money,
	GiaBanMH money,
	SoLuongTonKho int,
	HangSX /*TenNSX*/ varchar(15),
	XuatXu /*NuocSX*/ Nvarchar(15),
	MoTa Nvarchar(60),
	TinhTrang Nvarchar(15),
	constraint PK_MaMH primary key(MaMH)
)

alter table MatHang add constraint FK_MH foreign key(HangSX,XuatXu) references NhaSanXuat(TenNSX,NuocSX)

Insert into MatHang values('A626798B-7071-4710-88A8-7D31935A3019','Sirius 110cc',21500000,26500000,13,'Yamaha',N'Nhật Bản',N'Vành đúc, phanh cơ, màu đỏ đen',N'Mới')
Insert into MatHang values('1B30FA5C-2C4E-42B1-97FE-864391FC4040','Sirius 50cc',10000000,13500000,15,'Yamaha',N'Nhật Bản',N'Vành nan hoa, phanh cơ, màu đen xám',N'Mới')
Insert into MatHang values('01921D8D-F015-4309-A130-9390B2E8EC11','Honda Air Blade',33500000,40500000,7,'Honda',N'Nhật Bản',N'Vành đúc, phanh đĩa, màu vàng đen',N'Mới')
Insert into MatHang values('0FB80AA9-CC97-4157-8F04-BAA88F6E5266','Exciter 150cc',38500000,45000000,5,'Yamaha',N'Việt Nam',N'Vành đúc, phanh cơ, màu xanh trắng',N'Mới')
Insert into MatHang values('FAD9A8EF-6437-4A8F-ACB3-CC7232519EB6','Raider F150',21500000,27500000,10,'Suzuki',N'Việt Nam',N'Màu đỏ đen',N'Mới')

Create table HoaDon
(
      MaHD  uniqueidentifier DEFAULT newid(),
	  MaMH  uniqueidentifier,
	  MaKH  uniqueidentifier,                     /*Hoá đơn bán hàng cho khách hàng nào, Do nhân viên nào phụ trách*/
	  MaNV  uniqueidentifier,
	  HoTenNV Nvarchar(30),
      NgayLapHD  smalldatetime,
	  SoLuong int,
	  GiamGia float,
	  ThanhTien money,
	  constraint PK_MaHD primary key(MaHD)
)

Alter TABLE HoaDon add constraint FK_MaMH foreign key(MaMH) references MatHang(MaMH)
ALTER TABLE HOADON ADD CONSTRAINT FK_MaKH foreign key(MAKH) references KhachHang(MAKH)
ALTER TABLE HOADON ADD CONSTRAINT FK_MaNV foreign key(MANV) references NhanVien(MANV)

Insert into HoaDon values(newid(),'A626798B-7071-4710-88A8-7D31935A3019','1B051393-0949-4E96-959A-6B001F82B177','BDD24832-B4B9-4195-962C-91909DDE76C6','19/8/2021',1,0,26500000)
Insert into HoaDon values(newid(),'01921D8D-F015-4309-A130-9390B2E8EC11','955A14F5-F895-4F0B-B693-CE24A0B11321','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','17/10/2022',1,5,40500000*0.95)
Insert into HoaDon values(newid(),'0FB80AA9-CC97-4157-8F04-BAA88F6E5266','BDA8A3CC-9116-4215-99AB-351EF43154F5','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','15/10/2022',1,10,45000000*0.85)

Create table ThongTinBaoHanh
(
     MaBH  uniqueidentifier DEFAULT newid(),
	 MaMH  uniqueidentifier,
	 MaKH  uniqueidentifier,
	 MaNV  uniqueidentifier,
	 ThoiGian smalldatetime,
	 GhiChu nvarchar(30),
	 constraint PK_MaBH  primary key(MaBH)
)

alter table ThongTinBaoHanh add constraint FK_TTBH_MaMH foreign key(MaMH) references MatHang(MaMH)
alter table ThongTinBaoHanh add constraint FK_TTBH_MaKH foreign key(MaKH) references KhachHang(MaKH)
alter table ThongTinBaoHanh add constraint FK_TTBH_MaNV foreign key(MaNV) references NhanVien(MaNV)

Insert into ThongTinBaoHanh values(newid(),'A626798B-7071-4710-88A8-7D31935A3019','1B051393-0949-4E96-959A-6B001F82B177','BDD24832-B4B9-4195-962C-91909DDE76C6','10/7/2021','Lỗi từ NSX, hư tay côn')
Insert into ThongTinBaoHanh values(newid(),'1B30FA5C-2C4E-42B1-97FE-864391FC4040','F7EED837-719A-4AF5-A6FB-8F167847A2F7','90B6193C-0EBD-43F2-82A3-2DA131D767C6','12/7/2021','Thay nhớt định kỳ')


/*Tài khoản mà Mật khẩu đăng nhập dành cho NV Quản Lý và NV Văn Phòng*/
create table UserApp
(
  UserID uniqueidentifier DEFAULT newid(),
  MaNV uniqueidentifier,
  UserName varchar(15),
  Password varchar(20),
  Email  Varchar(30),   /*Email để khôi phục mật khẩu nếu cần*/
  constraint PK_UserID primary key(UserID)
)


Insert into UserApp values(newid(),'A5D54EA6-F80B-4A22-979B-D9FCB571FAB1','Ngquanly1','123456ABCDEF','phantantrung3rb@gmail.com') /*NV Quản Lý*/ 
Insert into UserApp values(newid(),'BDD24832-B4B9-4195-962C-91909DDE76C6','Nhvien1','ABCDEF123456','ThuongLe98Hoai@gmail.com')  /*NV Văn Phòng*/
Insert into UserApp values(newid(),'89250069-C6CC-4D5D-BB9F-AA298DCE0D67','Nhvien2', 'Englandvodich','SangNBa11896@gmail.com')   /*NV Văn Phòng*/


create table DonDatHang
( 
  MaDonDH uniqueidentifier DEFAULT newid(),
  SoDonDH int,
  MaMH  uniqueidentifier,
  SoLuongHang int,
  MaKH uniqueidentifier,
  MaNV uniqueidentifier,
  NGDH smalldatetime, /*Ngày Đặt Hàng*/
  constraint PK_MaDonDH primary key(MaDonDH)
)

alter table DonDatHang add constraint FK_MaMHDDH foreign key(MaMH) references MatHang(MaMH)
alter table DonDatHang add constraint FK_MaKHDDH foreign key(MaKH) references KHACHHANG(MaKH)
alter table DonDatHang add constraint FK_MaNVDDH foreign key(MaNV) references NHANVIEN(MaNV)

Insert into DonDatHang values(newid(),1,'01921D8D-F015-4309-A130-9390B2E8EC11',1,'8507F1FB-C2CC-48AF-B285-DC15AFF4EC77','90B6193C-0EBD-43F2-82A3-2DA131D767C6','25/11/2022')
