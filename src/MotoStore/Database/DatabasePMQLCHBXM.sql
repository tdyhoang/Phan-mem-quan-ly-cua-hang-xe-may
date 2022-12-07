/*Cơ sở dữ liệu cho Ứng dụng Quản Lý Cửa Hàng Bán Xe Máy*/
CREATE DATABASE QLYCHBANXEMAY

use QLYCHBANXEMAY
set dateformat dmy

Create table KhachHang
(
       MaKH uniqueidentifier DEFAULT newid(),
	   HoTenKH VARCHAR(30),
	   NgSinh smalldatetime,
	   GioiTinh VARCHAR(3),
	   DiaChi VARCHAR(40),
	   SDT  VARCHAR(10),
	   Email VARCHAR(30),
	   LoaiKH VARCHAR(10),         
	   constraint PK_MaKH primary key(MAKH)
	   /*Vip - Dc Giam Gia 10%
	     Than Quen - Dc Giam Gia 5%
	     Thuong - Ko Dc Giam Gia*/
)

alter table KhachHang add constraint Check_GT check(GioiTinh='Nam' or GioiTinh='Nu')
alter table KhachHang add constraint Check_LoaiKH check(LoaiKH='Vip' or LoaiKH='Thuong' or LoaiKH='Than quen')

Create table NhanVien
(
      MaNV uniqueidentifier DEFAULT newid(),
	  HoTenNV Varchar(30),
	  NgSinh smalldatetime,
	  GioiTinh VarChar(3),
	  DiaChi   Varchar(40),
	  SDT VARCHAR(10),
	  Email  Varchar(30),
	  ChucVu varchar(10),
	  Luong money,
	  Thuong money,
	  constraint PK_MaNV primary key(MANV)
)

alter table NhanVien add constraint CK_GTNV check(GioiTinh = 'Nam' or GioiTinh='Nu')

create table NhaSanXuat
(
     TenNSX varchar(15),
	 SDT  varchar(15),
	 Email varchar(45),
	 NuocSX Varchar(15),
	 constraint PK_NSX primary key(TenNSX,NuocSX)
)

create table MatHang
(
    MaMH uniqueidentifier DEFAULT newid(),
	TenMH varchar(15),
	GiaNhapMH money,
	GiaBanMH money,
	SoLuongTonKho int,
	HangSX /*TenNSX*/ varchar(15),
	XuatXu /*NuocSX*/ varchar(15),
	MoTa varchar(20),
	TinhTrang varchar(15),
	constraint PK_MaMH primary key(MaMH)
)

alter table MatHang add constraint FK_MH foreign key(HangSX,XuatXu) references NhaSanXuat(TenNSX,NuocSX)

Create table HoaDon
(
      MaHD  uniqueidentifier DEFAULT newid(),
	  MaMH  uniqueidentifier,
	  MaKH  uniqueidentifier,                     /*Hoá đơn bán hàng cho khách hàng nào, Do nhân viên nào phụ trách*/
	  MaNV  uniqueidentifier,
      NgayLapHD  smalldatetime,
	  SoLuong int,
	  GiamGia float,
	  ThanhTien money,
	  constraint PK_MaHD primary key(MaHD)
)

alter table HoaDon add constraint FK_MaMH foreign key(MaMH) references MatHang(MaMH)
ALTER TABLE HOADON ADD CONSTRAINT FK_MaKH foreign key(MAKH) references KhachHang(MAKH)
ALTER TABLE HOADON ADD CONSTRAINT FK_MaNV foreign key(MANV) references NhanVien(MANV)

Create table ThongTinBaoHanh
(
     MaBH  uniqueidentifier DEFAULT newid(),
	 MaMH  uniqueidentifier,
	 MaKH  uniqueidentifier,
	 MaNV  uniqueidentifier,
	 ThoiGian smalldatetime,
	 GhiChu varchar(30),
	 constraint PK_MaBH  primary key(MaBH)
)

alter table ThongTinBaoHanh add constraint FK_TTBH_MaMH foreign key(MaMH) references MatHang(MaMH)
alter table ThongTinBaoHanh add constraint FK_TTBH_MaKH foreign key(MaKH) references KhachHang(MaKH)
alter table ThongTinBaoHanh add constraint FK_TTBH_MaNV foreign key(MaNV) references NhanVien(MaNV)

/*Chỉ người quản lý mới có tài khoản và mật khẩu*/
create table UserADMIN
(
  UserID uniqueidentifier DEFAULT newid(),
  MaNV uniqueidentifier,
  UserName varchar(15),
  Password varchar(15),
  Email  Varchar(30),
  constraint PK_UserID primary key(UserID)
)

alter table UserADMIN add constraint FK_MaNVAdmin foreign key(MaNV) references NhanVien(MaNV)

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

Insert into KhachHang values('BDA8A3CC-9116-4215-99AB-351EF43154F5','Nguyen Van A','11/9/1979','Nam','34/34B Nguyen Trai, Q1, TpHCM','0876890495','NguyenVanA79@gmail.com','Vip')
Insert into KhachHang values('63D8101B-5CA4-4E9E-92C5-3859A071F9E9','Nguyen Van B','9/11/1997','Nam','873 Le Hong Phong, Q5, TpHCM','0867850847','NguyenVanB97@gmail.com','Thuong')
Insert into KhachHang values('1B051393-0949-4E96-959A-6B001F82B177','Tran Ngoc Han','22/6/2000','Nu','23/5 Nguyen Trai, Q5, TpHCM','0876890495','HanTran2k@gmail.com','Than quen')
Insert into KhachHang values('F7EED837-719A-4AF5-A6FB-8F167847A2F7','Nguyen Van Tam','6/4/1971','Nam','50/34 Le Dai Hanh, Q10, TpHCM','0917325476','Tamnguyen71@gmail.com','Thuong')
Insert into KhachHang values('955A14F5-F895-4F0B-B693-CE24A0B11321','Phan Thi Thanh','31/12/1981','Nu','34 Truong Dinh, Q3, TpHCM','0948531923','ThanhTP311281@gmail.com','Than quen')
Insert into KhachHang values('8507F1FB-C2CC-48AF-B285-DC15AFF4EC77','Do Ngoc Khai','4/3/2001','Nam','215 Nguyen Xien, Q9, TpHCM','0876180684','DNKSBTC@gmail.com','Thuong')

select *from KhachHang

Insert into NhanVien values('59ACED03-FF2D-4DAA-A8EF-273B3685F468','Tran Dai Nghia','1/12/1989','Nam','65/19 Cao Ba Quat, Q8, TpHCM','0911228313','NghiaTDai1289@gmail.com','NVSuaXe',7000000,3000000)
Insert into NhanVien values('90B6193C-0EBD-43F2-82A3-2DA131D767C6','Ngo Thanh Tuan','12/6/1980','Nam','45 Nguyen Canh Chan, Q1, TpHCM','0914102345','TuanNgoTh@gmail.com','NVSuaXe',8500000,4000000)
Insert into NhanVien values('A25A94A7-6765-4D4E-BAD9-2E7A764C9742','Cao Thai Quy','12/1/1995','Nam','32/3 Tran Binh Trong, Q5, TpHCM','0913476343','QuyCao95@gmail.com','NVSuaXe',5000000,2000000)
Insert into NhanVien values('BDD24832-B4B9-4195-962C-91909DDE76C6','Le Hoai Thuong','27/7/1998','Nu','65/19 Cao Ba Quat, Q8, TpHCM','0911228313','ThuongLe98Hoai@gmail.com','NVBanHang',5500000,2500000)
Insert into NhanVien values('89250069-C6CC-4D5D-BB9F-AA298DCE0D67','Nguyen Ba Sang','11/8/1996','Nam','23 Hoang Dieu, Q7, TpHCM','0856910975','SangNBa11896@gmail.com','NVBanHang',6000000,2500000)
Insert into NhanVien values('A5D54EA6-F80B-4A22-979B-D9FCB571FAB1','Phan Tan Trung','26/2/1989','Nam','125/2 Hoa Hung, Phuong 12, Q10, TpHCM','0876701812','phantantrung3rb@gmail.com','NVQuanLy',12000000,3500000) /*Người quản lý*/

select *from NhanVien

Insert into NhaSanXuat values('Yamaha','(30.24)38855080','YamahaMotorJapan@gmail.com','NhatBan')
Insert into NhaSanXuat values('Yamaha','(84.24)38855080','YamahaMotorVietNamCNVP@gmail.com','VietNam')
Insert into NhaSanXuat values('Suzuki','(84.89)47435209','SuzukiMotorVietNam@gmail.com','VietNam')
Insert into NhaSanXuat values('Honda','(84.2)18943170','HondaMotorVietNam@gmail.com','VietNam')
Insert into NhaSanXuat values('Honda','(30.2)18943170','HondaMotorJapan@gmail.com','NhatBan')

select *from NhaSanXuat

Insert into MatHang values('A626798B-7071-4710-88A8-7D31935A3019','Sirius 110cc',21500000,26500000,13,'Yamaha','NhatBan','Mau Do Den','Moi')
Insert into MatHang values('1B30FA5C-2C4E-42B1-97FE-864391FC4040','Sirius 50cc',10000000,13500000,15,'Yamaha','NhatBan','Mau Den Nham','Moi')
Insert into MatHang values('01921D8D-F015-4309-A130-9390B2E8EC11','Honda Air Blade',33500000,40500000,7,'Honda','NhatBan','Mau Vang Den','Moi')
Insert into MatHang values('0FB80AA9-CC97-4157-8F04-BAA88F6E5266','Exciter 150cc',38500000,45000000,5,'Yamaha','VietNam','Mau Den','Moi')
Insert into MatHang values('FAD9A8EF-6437-4A8F-ACB3-CC7232519EB6','Raider F150',21500000,27500000,10,'Suzuki','VietNam','Mau Do Den','Moi')

select *from MatHang

Insert into HoaDon values(newid(),'A626798B-7071-4710-88A8-7D31935A3019','1B051393-0949-4E96-959A-6B001F82B177','BDD24832-B4B9-4195-962C-91909DDE76C6','19/8/2021',1,0,26500000)
Insert into HoaDon values(newid(),'01921D8D-F015-4309-A130-9390B2E8EC11','955A14F5-F895-4F0B-B693-CE24A0B11321','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','17/10/2022',1,5,40500000*0.95)
Insert into HoaDon values(newid(),'0FB80AA9-CC97-4157-8F04-BAA88F6E5266','BDA8A3CC-9116-4215-99AB-351EF43154F5','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','15/10/2022',1,10,45000000*0.9)

Insert into ThongTinBaoHanh values(newid(),'A626798B-7071-4710-88A8-7D31935A3019','1B051393-0949-4E96-959A-6B001F82B177','BDD24832-B4B9-4195-962C-91909DDE76C6','10/7/2021','Loi tu NSX, Hu tay con')
Insert into ThongTinBaoHanh values(newid(),'1B30FA5C-2C4E-42B1-97FE-864391FC4040','F7EED837-719A-4AF5-A6FB-8F167847A2F7','90B6193C-0EBD-43F2-82A3-2DA131D767C6','12/7/2021','Thay nhot dinh ki')

Insert into UserADMIN values(newid(),'A5D54EA6-F80B-4A22-979B-D9FCB571FAB1','Ngquanly1','123456ABCDEF','phantantrung3rb@gmail.com')

Insert into DonDatHang values(newid(),1,'01921D8D-F015-4309-A130-9390B2E8EC11',1,'8507F1FB-C2CC-48AF-B285-DC15AFF4EC77','90B6193C-0EBD-43F2-82A3-2DA131D767C6','25/11/2022')