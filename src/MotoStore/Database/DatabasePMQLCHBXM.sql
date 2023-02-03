/*Cơ sở dữ liệu cho Ứng dụng Quản Lý Cửa Hàng Bán Xe Máy*/
CREATE DATABASE QLYCHBANXEMAY
GO

select top(10) mamh, sum(soluong) from HoaDon group by MaMH order by sum(soluong) desc

use QLYCHBANXEMAY
set dateformat dmy

Create table KhachHang
(
	ID int identity(1,1),
	MaKH as 'KH' + right('000' + cast(ID as varchar(3)), 3) persisted,
	HoTenKH NVARCHAR(30) not null,
	NgSinh smalldatetime,
	GioiTinh NVARCHAR(3) not null,
	DiaChi NVARCHAR(40),
	SDT  VARCHAR(10),
	Email NVARCHAR(30),
	LoaiKH NVARCHAR(10) not null,
	DaXoa bit DEFAULT 0 not null,
	constraint PK_MaKH primary key(MAKH)
	/*Vip - Dc Giam Gia 15%
	Than Quen - Dc Giam Gia 5%
	Thuong - Ko Dc Giam Gia*/
)
alter table KhachHang add constraint Check_GT check(GioiTinh='Nam' or GioiTinh=N'Nữ')
alter table KhachHang add constraint Check_LoaiKH check(LoaiKH='Vip' or LoaiKH=N'Thường' or LoaiKH=N'Thân quen')

Insert into KhachHang values(N'Nguyễn Văn A','11/9/1979','Nam',N'34/34B Nguyễn Trãi, Q1, TpHCM','0876890495',N'NguyenVanA79@gmail.com','Vip',0)
Insert into KhachHang values(N'Nguyễn Văn B','9/11/1997','Nam',N'873 Lê Hồng Phong, Q5, TpHCM','0867850847',N'NguyenVanB97@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Trần Ngọc Hân','22/6/2000',N'Nữ',N'23/5 Nguyễn Trãi, Q5, TpHCM','0876890495',N'HanTran2k@gmail.com',N'Thân quen',0)
Insert into KhachHang values(N'Nguyễn Văn Tâm','6/4/1971','Nam',N'50/34 Lê Đại Hành, Q10, TpHCM','0917325476',N'Tamnguyen71@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Phan Thị Thanh','31/12/1981',N'Nữ',N'34 Trương Định, Q3, TpHCM','0948531923',N'ThanhTP311281@gmail.com',N'Thân quen',0)
Insert into KhachHang values(N'Đỗ Ngọc Khải','4/3/2001','Nam',N'215 Nguyễn Xiển, Q9, TpHCM','0876180684',N'DNKSBTC@gmail.com',N'Thường',0)

Insert into KhachHang values(N'Trần B','22/9/1995',N'Nữ',N'Không Rõ','0987471210',N'tranB@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Võ Văn Đức','11/9/1989','Nam',N'TaynguyenSound','0987477640',N'VoVanDucTNS@gmail.com','Vip',0)
Insert into KhachHang values(N'Trần A','11/10/1988','Nam',N'Unknown','0987477123',N'TranA@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Đặng Thị M','10/1/1981',N'Nữ',N'90 Nguyễn Xiển, Q9, TpHCM','0876180699',N'GGEZ@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Huỳnh E','31/12/2002','Nam',N'Không Rõ','0987479810',N'ebenairui@gmail.com',N'Thường',0)

Insert into KhachHang values(N'Tofu','11/9/1994',N'Nam',N'TaynguyenSound','0987477620',N'TofuTNS@gmail.com',N'Vip',0)
Insert into KhachHang values(N'Chọn Cách','5/7/2003','Nam',N'Tây Nguyên Sầu Lắng','0987477511',N'Missu@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Nguyễn Văn B','3/4/1999','Nam',N'200 Nguyễn Văn Tăng, Q9, TpHCM','0876180685',N'123@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Về Chân Trời Xa','11/5/1989','Nam',N'TaynguyenSound','0925477610',N'115@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Huỳnh A','10/3/2004','Nam',N'Không Rõ','0987486610',N'12345@gmail.com',N'Vip',0)

Insert into KhachHang values(N'Nguyễn Văn D','4/3/2001',N'Nam',N'215 Nguyễn Xiển, Q9, TpHCM','0876180687',N'789@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Trần C','7/9/1999',N'Nữ',N'Không Rõ','0980171210',N'tranC@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Nỗi Niềm','11/12/2000',N'Nữ',N'TaynguyenSound','0987477610',N'buonvl@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Lê C','24/12/2003','Nam',N'Tháng 12 Lạnh Quá','0987967610',N'Th12LanhQua@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Lê Trần D','11/5/2003',N'Nữ',N'Không Rõ','0876180698',N'01i@gmail.com',N'Thường',0)

Insert into KhachHang values(N'Mây Lang Thang','15/5/2000',N'Nam',N'Góc Nhỏ Riêng Tư','0987471210',N'MLTTNS@gmail.com',N'Thân quen',0)
Insert into KhachHang values(N'Qua Những Tiếng Ve','11/5/2000','Nam',N'TaynguyenSound','0987474610',N'QNTVTNS@gmail.com',N'Thân quen',0)
Insert into KhachHang values(N'Tùng Tea','11/9/1989','Nam',N'TaynguyenSound','0987477610',N'TeaTNS@gmail.com',N'Vip',0)
Insert into KhachHang values(N'Lê D','20/1/1999','Nam',N'TSM','0147477610',N'TSM@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Nguyễn Văn C','12/3/2001','Nam',N'201 Nguyễn Xiển, Q9, TpHCM','0876180686',N'456@gmail.com',N'Thường',0)

Insert into KhachHang values(N'Lê A','23/4/1995',N'Nữ',N'TP Ban Mê','0987511610',N'TpBMT@gmail.com',N'Thân quen',0)
Insert into KhachHang values(N'Lê F','12/1/1998','Nam',N'Tây Nguyên Chiều Lộng Gió','0987477310',N'LeF@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Huỳnh C','11-5-2004',N'Nam',N'Cả Một Trời Cao Nguyên','0987478010',N'113@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Huỳnh B','25/8/1989',N'Nữ',N'Lãng Duuu','0123477610',N'LangDuTNS@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Ngô Tất Tố','10/10/1995','Nam',N'400 Nguyễn Xiển, Q9, TpHCM','0876180100',N'DRX123@gmail.com',N'Thân quen',0)

Insert into KhachHang values(N'Lê E','2/10/1993',N'Nữ',N'Thiên An Môn','0987513010',N'TAM@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Huỳnh D','12/1/1998',N'Nữ',N'Phố Núi','0923477610',N'younggirl@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Lê B','25/2/2001',N'Nam',N'Ghé Qua','0765477610',N'GheQua@gmail.com',N'Thân quen',0)
Insert into KhachHang values(N'Không Trông Thấy Rõ','11/9/2001',N'Nữ',N'Mỹ Tho, Tiền Giang','0987487610',N'0coemail@gmail.com',N'Thường',0)
Insert into KhachHang values(N'Cường','11/9/1997','Nam',N'TaynguyenSound','0987477630',N'PeaCeeTNS@gmail.com',N'Vip',0)

Create table NhanVien
(
	ID int identity(1,1),
	MaNV as 'NV' + right('000' + cast(ID as varchar(3)), 3) persisted,
	HoTenNV NVarchar(30),
	NgSinh smalldatetime,
	GioiTinh NVarChar(3) not null,
	DiaChi   NVarchar(40),
	SDT VARCHAR(10),
	Email  NVarchar(30),
	ChucVu Nvarchar(10),
	NgVL smalldatetime,     /*Ngày vào làm để hiện thêm vài thông tin ở trang chính*/
	Luong money,
	DaXoa bit DEFAULT 0 not null,
	constraint PK_MaNV primary key(MANV)	 
)
alter table NhanVien add constraint CK_GTNV check(GioiTinh = 'Nam' or GioiTinh=N'Nữ')

Insert into NhanVien values(N'Trần Đại Nghĩa','1/12/1989','Nam',N'65/19 Cao Bá Quát, Q8, TpHCM','0911228313',N'NghiaTDai1289@gmail.com',N'Sửa Xe','3/2/2018',7000000,0)
Insert into NhanVien values(N'Ngô Thanh Tuấn','12/6/1980','Nam',N'45 Nguyễn Cảnh Chân, Q1, TpHCM','0914102345',N'TuanNgoTh@gmail.com',N'Sửa Xe','10/2/2018',8500000,0)
Insert into NhanVien values(N'Cao Thái Quý','12/1/1995','Nam',N'32/3 Trần Bình Trọng, Q5, TpHCM','0913476343',N'QuyCao95@gmail.com',N'Sửa Xe','11-5-2021',5000000,0)
Insert into NhanVien values(N'Lê Hoài Thương','27/7/1998',N'Nữ',N'65/19 Cao Bá Quát, Q8, TpHCM','0911228313',N'ThuongLe98Hoai@gmail.com',N'Văn Phòng','26/3/2021',5500000,0)
Insert into NhanVien values(N'Nguyễn Bá Sang','11/8/1996','Nam',N'23 Hoàng Diệu, Q7, TpHCM','0856910975',N'SangNBa11896@gmail.com',N'Văn Phòng','3/2/2018',6000000,0)
Insert into NhanVien values(N'Phan Tấn Trung','26/2/1989','Nam',N'125/2 Hòa Hưng, Phuờng 12, Q10, TpHCM','0876701812',N'phantantrung3rb@gmail.com',N'Quản Lý','3/2/2018',12000000,0) /*Người Quản Lý*/
Insert into NhanVien values(N'Hồ Thanh Hằng','19/8/2000',N'Nữ',N'Cao Ốc A Ngô Gia Tự, Q10, TpHCM','0947910521',N'HangTH2k@gmail.com',N'Văn Phòng','10/7/2019',5000000,0)

create table NhaCungCap
(
	ID int identity(1,1),
	MaNCC as 'CC' + right('000' + cast(ID as varchar(3)), 3) persisted,
	TenNCC Nvarchar(30) not null,
	SDT  varchar(15),
	Email Nvarchar(45),
	DiaChi NVarchar(75),
	DaXoa bit DEFAULT 0 not null,
	constraint PK_NNCC primary key(MaNCC)
)

Insert into NhaCungCap values(N'Yamaha','(30.24)38855080',N'YamahaMotorJapan@gmail.com',N'Fukuoka, Nhật Bản',0)
Insert into NhaCungCap values(N'Lai Hương','(84.24)38855080',N'YamahaMotorVietNamCNVP@gmail.com',N'Vĩnh Phúc, Việt Nam',0)
Insert into NhaCungCap values(N'Suzuki','(84.89)47435209',N'SuzukiMotorVietNam@gmail.com',N'Hà Nội, Việt Nam',0)
Insert into NhaCungCap values(N'Honda','(84.2)18943170',N'HondaMotorThaiLand@gmail.com',N'Bangkok, Thái Lan',0)
Insert into NhaCungCap values(N'Honda','(30.2)18943170',N'HondaMotorJapan@gmail.com',N'Chiba, Nhật Bản',0)
Insert into NhaCungCap values(N'Kawasaki','(0117)1757718',N'KawasakiJapan@gmail.com',N'Frontale, Nhật Bản',0)
Insert into NhaCungCap values(N'Lai Hương','(084)1756514',N'KawasakiJapan@gmail.com',N'Hải Phòng, Việt Nam',0)
Insert into NhaCungCap values(N'Piaggio','(010)15432170',N'PiaggioItaly@gmail.com',N'Roma, Ý',0)
Insert into NhaCungCap values(N'Sym','(123)1706762',N'SymTaiWan@gmail.com',N'Đài Loan',0)
Insert into NhaCungCap values(N'Vinfast','(0631)17433170',N'VinfastVietNam@gmail.com',N'Hà Nội, Việt Nam',0)
Insert into NhaCungCap values(N'Sym','(084)1702725',N'SymVietNam@gmail.com',N'TP. Hồ Chí Minh, Việt Nam',0)

create table MatHang
(
	ID int identity(1,1),
	MaMH as 'MH' + right('000' + cast(ID as varchar(3)), 3) persisted,
	TenMH Nvarchar(30) not null,
	SoPhanKhoi int not null,
	Mau Nvarchar(15),
	GiaNhapMH money,
	GiaBanMH money,
	SoLuongTonKho int not null,
	MaNCC varchar(5) not null,
	HangSX /*TenNSX*/ Nvarchar(15),
	XuatXu /*NuocSX*/ Nvarchar(15),
	MoTa Nvarchar(75),
	DaXoa bit DEFAULT 0 not null,
	constraint PK_MaMH primary key(MaMH)
)
alter table MatHang add constraint FK_MH foreign key(MaNCC) references NhaCungCap(MaNCC)

Insert into MatHang values(N'Sirius', 50, N'đen xám',10000000,13500000,15,'CC010',N'Yamaha',N'Nhật Bản',N'Xe số nhỏ, Vành nan hoa, Phanh cơ, Còn mới',0)
Insert into MatHang values(N'Sirius', 110, N'đỏ đen',21500000,26500000,13,'CC010',N'Yamaha',N'Nhật Bản',N'Xe số lớn, Vành đúc, Phanh cơ, Còn mới',0)
Insert into MatHang values(N'Honda Air Blade',150, N'vàng đen',33500000,40500000,7,'CC001',N'Honda',N'Nhật Bản',N'Xe tay ga, Vành đúc, Phanh đĩa, Còn mới',0)
Insert into MatHang values(N'Exciter', 150, N'xanh biển',38500000,45000000,5,'CC011',N'Yamaha',N'Việt Nam',N'Xe tay côn, Vành đúc, Phanh cơ, Còn mới',0)
Insert into MatHang values(N'Raider F150', 150, N'đỏ đen', 21500000,27500000,10,'CC006',N'Suzuki',N'Việt Nam',N'Xe tay côn, Vành đúc, Còn mới',0)

Insert into MatHang values(N'Vision', 150, N'đỏ đen',22500000,29500000,10,'CC001',N'Honda',N'Nhật Bản',N'Xe tay ga, Vành đúc, Còn mới',0)
Insert into MatHang values(N'Lead', 110, N'trắng khói',24500000,31500000,10,'CC001',N'Honda',N'Nhật Bản',N'Xe tay ga, Vành đúc, Còn mới',0)
Insert into MatHang values(N'Kawasaki Z1000',1043, N'xanh đen',410000000,435500000,10,'CC003',N'Kawasaki',N'Nhật Bản',N'Xe phân khối lớn, Còn mới',0)
Insert into MatHang values(N'Kawasaki Ninja ZX-10R', 150, N'xanh lục đen', 699000000,729000000,5,'CC003',N'Kawasaki',N'Nhật Bản',N'Xe phân khối lớn, Còn mới',0)
Insert into MatHang values(N'Attila 50', 110, N'đỏ', 20500000,25700000,10,'CC007',N'Sym',N'Đài Loan',N'Xe tay ga, Còn mới',0)

Insert into MatHang values(N'Vespa Print 2020', 110, N'đỏ đen',23500000,30500000,10,'CC005',N'Piaggio',N'Ý',N'Xe tay ga, Phanh Abs, Còn mới',0)
Insert into MatHang values(N'SH 150', 150, N'xanh lam',115000000,129250000,9,'CC001',N'Honda',N'Nhật Bản',N'Xe tay ga, Phanh Abs, Còn mới',0)
Insert into MatHang values(N'Wave Alpha',110, N'trắng',20500000,24500000,15,'CC001',N'Honda',N'Nhật Bản',N'Xe số lớn, Vành nan hoa, Còn mới',0)
Insert into MatHang values(N'Sirius FI', 110, N'đen khói', 22500000,27500000,10,'CC011',N'Yamaha',N'Việt Nam',N'Xe số lớn, Vành đúc, Còn mới',0)
Insert into MatHang values(N'EVO200', 100, N'vàng', 23500000,22500000,15,'CC009',N'Vinfast',N'Việt Nam',N'Xe điện, Còn mới',0)

Create table HoaDon
(
	ID int identity(1,1),
	MaHD as 'HD' + right('000' + cast(ID as varchar(3)), 3) persisted,
	MaMH  varchar(5) not null,
	MaKH  varchar(5) not null,                     /*Hoá đơn bán hàng cho khách hàng nào, Do nhân viên nào phụ trách*/
	MaNV  varchar(5) not null,
	NgayLapHD  smalldatetime,
	SoLuong int not null,                               /*số lượng xe*/
	ThanhTien money not null,
	constraint PK_MaHD primary key(MaHD)
)

Alter TABLE HOADON add constraint FK_MaMH foreign key(MaMH) references MatHang(MaMH)
ALTER TABLE HOADON ADD CONSTRAINT FK_MaKH foreign key(MAKH) references KhachHang(MAKH)
ALTER TABLE HOADON ADD CONSTRAINT FK_MaNV foreign key(MANV) references NhanVien(MANV)

Insert into HoaDon values('MH013','KH024','NV004','19/8/2021',1,20825000)
Insert into HoaDon values('MH006','KH001','NV004','17/10/2022',1,25075000)
Insert into HoaDon values('MH009','KH034','NV007','15/10/2022',1,692550000)
Insert into HoaDon values('MH008','KH034','NV005','28/10/2021',1,413725000)
Insert into HoaDon values('MH004','KH031','NV004','10/11/2021',1,42750000)

Insert into HoaDon values('MH005','KH027','NV007','20/11/2022',1,26125000)
Insert into HoaDon values('MH014','KH031','NV006','8/3/2021',1,26125000) /**/
Insert into HoaDon values('MH004','KH021','NV004','14/2/2022',1,45000000)
Insert into HoaDon values('MH005','KH003','NV005','10/4/2021',1,26125000)
Insert into HoaDon values('MH004','KH015','NV007','14/4/2022',1,45000000)

Insert into HoaDon values('MH005','KH002','NV006','28/6/2022',1,27500000) /*8*/
Insert into HoaDon values('MH013','KH030','NV007','15/2/2022',1,24500000)
Insert into HoaDon values('MH001','KH025','NV007','28/8/2022',1,26500000)
Insert into HoaDon values('MH011','KH031','NV007','25/7/2021',1,28975000)
Insert into HoaDon values('MH008','KH004','NV005','19/5/2022',1,435500000)

Insert into HoaDon values('MH015','KH006','NV007','29/1/2021',1,22500000)  /*15*/
Insert into HoaDon values('MH006','KH002','NV007','10/3/2022',1,29500000)
Insert into HoaDon values('MH011','KH028','NV005','8/2/2021',1,30500000)
Insert into HoaDon values('MH008','KH015','NV007','11/5/2021',1,435500000)
Insert into HoaDon values('MH013','KH014','NV005','31/5/2022',1,24500000)

Insert into HoaDon values('MH015','KH025','NV005','28/12/2022',1,22500000)  /*20*/
Insert into HoaDon values('MH013','KH008','NV004','15/1/2022',1,20825000)
Insert into HoaDon values('MH009','KH026','NV004','15/10/2022',1,729000000)
Insert into HoaDon values('MH001','KH009','NV005','25/8/2021',1,26500000)
Insert into HoaDon values('MH007','KH028','NV006','3/9/2022',1,31500000)

Insert into HoaDon values('MH006','KH003','NV007','12/12/2021',1,28025000)    /*25*/
Insert into HoaDon values('MH015','KH018','NV006','28/6/2021',1,22500000)
Insert into HoaDon values('MH004','KH011','NV004','17/7/2021',1,45000000)
Insert into HoaDon values('MH006','KH025','NV005','25/11/2022',1,29500000)
Insert into HoaDon values('MH011','KH022','NV005','5/9/2022',1,28975000)

Insert into HoaDon values('MH008','KH026','NV005','19/1/2022',1,435500000) /*30*/
Insert into HoaDon values('MH003','KH017','NV005','5/9/2021',1,40500000)
Insert into HoaDon values('MH001','KH019','NV004','19/8/2022',1,26500000)
Insert into HoaDon values('MH011','KH036','NV007','27/7/2022',1,25925000)
Insert into HoaDon values('MH007','KH007','NV007','20/8/2022',1,31500000)

Insert into HoaDon values('MH005','KH016','NV006','11/12/2022',1,23375000)
Insert into HoaDon values('MH004','KH010','NV007','1/1/2023',1,45000000)
Insert into HoaDon values('MH014','KH006','NV006','1/1/2023',1,27500000)

Create table ThongTinBaoHanh
(
	ID int identity(1,1),
	MaBH as 'BH' + right('000' + cast(ID as varchar(3)), 3) persisted,
	MaHD  varchar(5) not null,
	ThoiGian smalldatetime,
	GhiChu nvarchar(60),
	constraint PK_MaBH  primary key(MaBH)
)

alter table ThongTinBaoHanh add constraint FK_TTBH_MaHD foreign key(MaHD) references HoaDon(MaHD)

Insert into ThongTinBaoHanh values('HD006','22/11/2022',N'Lỗi từ NSX, hư tay côn')
Insert into ThongTinBaoHanh values('HD011','12/7/2022',N'Thay nhớt định kỳ')


/*Tài khoản mà Mật khẩu đăng nhập dành cho NV Quản Lý và NV Văn Phòng*/
create table UserApp
(
	MaNV varchar(5),
	UserName Nvarchar(15) unique not null,
	Password Nvarchar(20) not null,
	Email NVarchar(30) not null,   /*Email để khôi phục mật khẩu nếu cần*/
	constraint PK_UA_MaNV primary key(MaNV)
)

alter table UserApp add constraint FK_UA_MaNV foreign key(MaNV) references NhanVien(MaNV)

Insert into UserApp values('NV006',N'Ngquanly1',N'123456ABCDEF',N'phantantrung3rb@gmail.com') /*NV Quản Lý*/ 
Insert into UserApp values('NV004',N'Nhvien1',N'ABCDEF123456',N'ThuongLe98Hoai@gmail.com')  /*NV Văn Phòng*/
Insert into UserApp values('NV005',N'Nhvien2', N'Englandvodich',N'SangNBa11896@gmail.com')   /*NV Văn Phòng*/
Insert into UserApp values('NV007',N'Nhvien3', N'matkhaudenho',N'HangTH2k@gmail.com')   /*NV Văn Phòng*/

create table DonDatHang
(
	ID int identity(1,1),
	MaDDH as 'DH' + right('000' + cast(ID as varchar(3)), 3) persisted,
	MaMH  varchar(5) not null,
	SoLuongHang int,
	MaKH varchar(5) not null,
	MaNV varchar(5) not null,
	NGDH smalldatetime,                                          /*Ngày Đặt Hàng*/                     
	constraint PK_MaDonDH primary key(MaDDH)                  /*Phần này để nhân viên tương tác với Khách, chỉ nhân viên tương tác với khách mới có quyền thêm xoá sửa*/
)

alter table DonDatHang add constraint FK_MaMHDDH foreign key(MaMH) references MatHang(MaMH)
alter table DonDatHang add constraint FK_MaKHDDH foreign key(MaKH) references KHACHHANG(MaKH)
alter table DonDatHang add constraint FK_MaNVDDH foreign key(MaNV) references NHANVIEN(MaNV)

Insert into DonDatHang values('MH008',1,'KH011','NV007','25/11/2022')

Create Table LenLich
(
	LenLichID uniqueidentifier DEFAULT newid(),
	MaNV varchar(5) not null,          /*Mã Nhân Viên để biết ai đã lên lịch, tiện cho tính năng xem lịch sử hoạt động sau này*/
	NgLenLichBD smalldatetime not null,      /*Ngày, giờ bắt đầu*/
	NgLenLichKT smalldatetime not null,      /*Ngày, giờ kết thúc*/
	NoiDungLenLich nvarchar(200),   /*Nội dung lên lịch hôm đó*/
	CONSTRAINT PK_LenLichID primary key(LenLichID)
)
alter table LenLich add constraint FKLL_MaNV foreign key(MaNV) references NhanVien(MaNV)

Create Table LichSuHoatDong
(
	LshdID uniqueidentifier DEFAULT newid(),
	MaNV varchar(5) not null,
	ThoiGian datetime not null,
	HoatDong nvarchar(200),
	CONSTRAINT PK_LshdID primary key(LshdID)
)
alter table LichSuHoatDong add constraint FKLSHD_MaNV foreign key(MaNV) references NhanVien(MaNV)

Select top(1) MaMH from MatHang order by ID desc