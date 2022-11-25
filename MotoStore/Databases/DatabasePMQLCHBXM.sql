﻿/*Cơ sở dữ liệu cho Ứng dụng Quản Lý Cửa Hàng Bán Xe Máy*/
CREATE DATABASE QLYCHBANXEMAY

use QLYCHBANXEMAY
set dateformat dmy

Create table KhachHang
(
       MaKH CHAR(7),
	   HoTenKH VARCHAR(30),
	   NgSinh smalldatetime,
	   GioiTinh  CHAR(3),
	   DiaChi VARCHAR(40),
	   SDT  VARCHAR(10),
	   Email VARCHAR(30),
	   LoaiKH VARCHAR(10),         /*Vip, Than Quen, ...*/
	   constraint PK_MaKH primary key(MAKH)
)

alter table KhachHang add constraint Check_GT check(GioiTinh='Nam' or GioiTinh='Nu')
alter table KhachHang add constraint Check_LoaiKH check(LoaiKH='Vip' or LoaiKH='Thuong' or LoaiKH='Than quen')

Create table NhanVien
(
      MaNV CHAR(7),
	  HoTenNV Varchar(30),
	  NgSinh smalldatetime,
	  GioiTinh Char(3),
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
    MaMH char(7),
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
      MaHD  Char(7),
	  MaMH  Char(7),
	  MaKH  Char(7),                     /*Hoá đơn bán hàng cho khách hàng nào, Do nhân viên nào phụ trách*/
	  MaNV  Char(7),
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
     MaBH  char(7),
	 MaMH  char(7),
	 MaKH  char(7),
	 MaNV  char(7),
	 ThoiGian smalldatetime,
	 GhiChu varchar(30),
	 constraint PK_MaBH  primary key(MaBH)
)

alter table ThongTinBaoHanh add constraint FK_TTBH_MaMH foreign key(MaMH) references MatHang(MaMH)
alter table ThongTinBaoHanh add constraint FK_TTBH_MaKH foreign key(MaKH) references KhachHang(MaKH)
alter table ThongTinBaoHanh add constraint FK_TTBH_MaNV foreign key(MaNV) references NhanVien(MaNV)

create table Users
(
  MaNV char(7),
  UserName varchar(15),
  Password varchar(15),
  ChucVu varchar(10),
  constraint PK_Users_MaNV primary key(MaNV)
)

alter table Users add constraint FK_Users_MaNV foreign key(MaNV) references NhanVien(MaNV)

/* Create table Admin
(
    
) */

Insert into KhachHang values('KH001','Nguyen Van A','11/9/1979','Nam','34/34B Nguyen Trai, Q1, TpHCM','0876890495','NguyenVanA79@gmail.com','Vip')
Insert into KhachHang values('KH002','Nguyen Van B','9/11/1997','Nam','873 Le Hong Phong, Q5, TpHCM','0867850847','NguyenVanB97@gmail.com','Thuong')
Insert into KhachHang values('KH003','Tran Ngoc Han','22/6/2000','Nu','23/5 Nguyen Trai, Q5, TpHCM','0876890495','HanTran2k@gmail.com','Than quen')
Insert into KhachHang values('KH004','Nguyen Van Tam','6/4/1971','Nam','50/34 Le Dai Hanh, Q10, TpHCM','0917325476','Tamnguyen71@gmail.com','Thuong')
Insert into KhachHang values('KH005','Phan Thi Thanh','31/12/1981','Nu','34 Truong Dinh, Q3, TpHCM','0948531923','ThanhTP311281@gmail.com','Than quen')

select *from KhachHang

Insert into NhanVien values('NV01','Tran Dai Nghia','1/12/1989','Nam','65/19 Cao Ba Quat, Q8, TpHCM','0911228313','NghiaTDai1289@gmail.com','NVSuaXe',7000000,3000000)
Insert into NhanVien values('NV02','Ngo Thanh Tuan','12/6/1980','Nam','45 Nguyen Canh Chan, Q1, TpHCM','0914102345','TuanNgoTh@gmail.com','NVSuaXe',8500000,4000000)
Insert into NhanVien values('NV03','Cao Thai Quy','12/1/1995','Nam','32/3 Tran Binh Trong, Q5, TpHCM','0913476343','QuyCao95@gmail.com','NVSuaXe',5000000,2000000)
Insert into NhanVien values('NV04','Le Hoai Thuong','27/7/1998','Nu','65/19 Cao Ba Quat, Q8, TpHCM','0911228313','ThuongLe98Hoai@gmail.com','NVBanHang',5500000,2500000)
Insert into NhanVien values('NV05','Nguyen Ba Sang','11/8/1996','Nam','23 Hoang Dieu, Q7, TpHCM','0856910975','SangNBa11896@gmail.com','NVBanHang',6000000,2500000)

select *from NhanVien

Insert into NhaSanXuat values('Yamaha','(30.24)38855080','YamahaMotorJapan@gmail.com','NhatBan')
Insert into NhaSanXuat values('Yamaha','(84.24)38855080','YamahaMotorVietNamCNVP@gmail.com','VietNam')
Insert into NhaSanXuat values('Suzuki','(84.89)47435209','SuzukiMotorVietNam@gmail.com','VietNam')
Insert into NhaSanXuat values('Honda','(84.2)18943170','HondaMotorVietNam@gmail.com','VietNam')
Insert into NhaSanXuat values('Honda','(30.2)18943170','HondaMotorJapan@gmail.com','NhatBan')

select *from NhaSanXuat
