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
	   DaXoa bit DEFAULT 0 not null,
	   constraint PK_MaKH primary key(MAKH)
	   /*Vip - Dc Giam Gia 15%
	     Than Quen - Dc Giam Gia 5%
	     Thuong - Ko Dc Giam Gia*/
)
alter table KhachHang add constraint Check_GT check(GioiTinh='Nam' or GioiTinh=N'Nữ')
alter table KhachHang add constraint Check_LoaiKH check(LoaiKH='Vip' or LoaiKH=N'Thường' or LoaiKH=N'Thân quen')

Insert into KhachHang values('BDA8A3CC-9116-4215-99AB-351EF43154F5',N'Nguyễn Văn A','11/9/1979','Nam',N'34/34B Nguyễn Trãi, Q1, TpHCM','0876890495','NguyenVanA79@gmail.com','Vip',0)
Insert into KhachHang values('63D8101B-5CA4-4E9E-92C5-3859A071F9E9',N'Nguyễn Văn B','9/11/1997','Nam',N'873 Lê Hồng Phong, Q5, TpHCM','0867850847','NguyenVanB97@gmail.com',N'Thường',0)
Insert into KhachHang values('1B051393-0949-4E96-959A-6B001F82B177',N'Trần Ngọc Hân','22/6/2000',N'Nữ',N'23/5 Nguyễn Trãi, Q5, TpHCM','0876890495','HanTran2k@gmail.com',N'Thân quen',0)
Insert into KhachHang values('F7EED837-719A-4AF5-A6FB-8F167847A2F7',N'Nguyễn Văn Tâm','6/4/1971','Nam',N'50/34 Lê Đại Hành, Q10, TpHCM','0917325476','Tamnguyen71@gmail.com',N'Thường',0)
Insert into KhachHang values('955A14F5-F895-4F0B-B693-CE24A0B11321',N'Phan Thị Thanh','31/12/1981',N'Nữ',N'34 Trương Định, Q3, TpHCM','0948531923','ThanhTP311281@gmail.com',N'Thân quen',0)
Insert into KhachHang values('8507F1FB-C2CC-48AF-B285-DC15AFF4EC77',N'Đỗ Ngọc Khải','4/3/2001','Nam',N'215 Nguyễn Xiển, Q9, TpHCM','0876180684','DNKSBTC@gmail.com',N'Thường',0)

Insert into KhachHang values('F60D5B81-3662-427C-B696-06DA4F99BCF8',N'Trần B','22/9/1995',N'Nữ',N'Không Rõ','0987471210','tranB@gmail.com',N'Thường',0)
Insert into KhachHang values('A7402648-6C90-4242-92CF-15EC15B43307',N'Võ Văn Đức','11/9/1989','Nam',N'TaynguyenSound','0987477640','VoVanDucTNS@gmail.com','Vip',0)
Insert into KhachHang values('249CADA6-53D5-4D9E-85B9-1AF5BFB067C0',N'Trần A','11/10/1988','Nam',N'Unknown','0987477123','TranA@gmail.com',N'Thường',0)
Insert into KhachHang values('7009D6E2-CAA8-4479-A62A-2AE40FF22305',N'Đặng Thị M','10/1/1981',N'Nữ',N'90 Nguyễn Xiển, Q9, TpHCM','0876180699','GGEZ@gmail.com',N'Thường',0)
Insert into KhachHang values('61860DF3-DD85-4998-8746-2B215D674642',N'Huỳnh E','31/12/2002','Nam',N'Không Rõ','0987479810','ebenairui@gmail.com',N'Thường',0)

Insert into KhachHang values('79B11F53-4E9E-46FA-9C0D-2B3B5617603A',N'Tofu','11/9/1994',N'Nam',N'TaynguyenSound','0987477620','TofuTNS@gmail.com',N'Vip',0)
Insert into KhachHang values('4F9C36D3-7E4A-46A9-94C0-309FE5143559',N'Chọn Cách','5/7/2003','Nam',N'Tây Nguyên Sầu Lắng','0987477511','Missu@gmail.com',N'Thường',0)
Insert into KhachHang values('B8186750-FABE-470A-ADD4-349C280FB09D0',N'Nguyễn Văn B','3/4/1999','Nam',N'200 Nguyễn Văn Tăng, Q9, TpHCM','0876180685','123@gmail.com',N'Thường',0)
Insert into KhachHang values('5A09323D-9616-48BC-93BD-3D041E14B683',N'Về Chân Trời Xa','11/5/1989','Nam',N'TaynguyenSound','0925477610','115@gmail.com',N'Thường',0)
Insert into KhachHang values('ABCEEB43-F85D-40FA-A8F0-3EE468D954F3',N'Huỳnh A','10/3/2004','Nam',N'Không Rõ','0987486610','12345@gmail.com',N'Vip',0)

Insert into KhachHang values('60E36581-2021-475B-A4A7-40EE84C6D544',N'Nguyễn Văn D','4/3/2001',N'Nam',N'215 Nguyễn Xiển, Q9, TpHCM','0876180687','789@gmail.com',N'Thường',0)
Insert into KhachHang values('2F115694-CB81-44E8-9C09-472714E8C59C',N'Trần C','7/9/1999',N'Nữ',N'Không Rõ','0980171210','tranC@gmail.com',N'Thường',0)
Insert into KhachHang values('E36C38DB-F581-45B1-9698-4E3490426863',N'Nỗi Niềm','11/12/2000',N'Nữ',N'TaynguyenSound','0987477610','buonvl@gmail.com',N'Thường',0)
Insert into KhachHang values('A9E05B08-4AAA-4113-AE84-54CAB0E54765',N'Lê C','24/12/2003','Nam',N'Tháng 12 Lạnh Quá','0987967610','Th12LanhQua@gmail.com',N'Thường',0)
Insert into KhachHang values('A3B9B7F9-2DEA-4E2A-91A9-620A4D0DC4A6',N'Lê Trần D','11/5/2003',N'Nữ',N'Không Rõ','0876180698','01i@gmail.com',N'Thường',0)

Insert into KhachHang values('7A4F27C3-FDBB-4048-A1D4-6F2402109968',N'Mây Lang Thang','15/5/2000',N'Nam',N'Góc Nhỏ Riêng Tư','0987471210','MLTTNS@gmail.com',N'Thân quen',0)
Insert into KhachHang values('11229832-8D41-4FD8-8DF4-714608BE9FC2',N'Qua Những Tiếng Ve','11/5/2000','Nam',N'TaynguyenSound','0987474610','QNTVTNS@gmail.com',N'Thân quen',0)
Insert into KhachHang values('C2B03BF4-A464-47FD-A772-7303EA0A534D',N'Tùng Tea','11/9/1989','Nam',N'TaynguyenSound','0987477610','TeaTNS@gmail.com',N'Vip',0)
Insert into KhachHang values('7E87756D-A431-4A85-92DD-76CAC797AC0A',N'Lê D','20/1/1999','Nam',N'TSM','0147477610','TSM@gmail.com',N'Thường',0)
Insert into KhachHang values('D203A5B9-F117-4099-B928-7B10831B6C13',N'Nguyễn Văn C','12/3/2001','Nam',N'201 Nguyễn Xiển, Q9, TpHCM','0876180686','456@gmail.com',N'Thường',0)

Insert into KhachHang values('7AFC81FB-BBA7-496E-9346-9FA79F292ED4',N'Lê A','23/4/1995',N'Nữ',N'TP Ban Mê','0987511610','TpBMT@gmail.com',N'Thân quen',0)
Insert into KhachHang values('5A8A190F-C6CA-4FA2-9A17-A8834B11BFF1',N'Lê F','12/1/1998','Nam',N'Tây Nguyên Chiều Lộng Gió','0987477310','LeF@gmail.com',N'Thường',0)
Insert into KhachHang values('86818A21-6C8E-498A-A2DC-AC3F76E4F9E4',N'Huỳnh C','11-5-2004',N'NAM',N'Cả Một Trời Cao Nguyên','0987478010','113@gmail.com',N'Thường',0)
Insert into KhachHang values('B2386D36-25AE-4495-B2EF-C24EA4A8D1CA',N'Huỳnh B','25/8/1989',N'Nữ',N'Lãng Duuu','0123477610','LangDuTNS@gmail.com',N'Thường',0)
Insert into KhachHang values('BC7A7504-075F-461D-8CE3-DD058730125C',N'Ngô Tất Tố','10/10/1995','Nam',N'400 Nguyễn Xiển, Q9, TpHCM','0876180100','DRX123@gmail.com',N'Thân quen',0)

Insert into KhachHang values('563FC169-63AF-4CBF-B84C-E1202C70A7A7',N'Lê E','2/10/1993',N'Nữ',N'Thiên An Môn','0987513010','TAM@gmail.com',N'Thường',0)
Insert into KhachHang values('D46E90A2-9704-408E-9262-E2F65777F9D8',N'Huỳnh D','12/1/1998',N'Nữ',N'Phố Núi','0923477610','younggirl@gmail.com',N'Thường',0)
Insert into KhachHang values('B98FC8C5-EBFA-4E50-BB08-E94CB32371C8',N'Lê B','25/2/2001',N'NAM',N'Ghé Qua','0765477610','GheQua@gmail.com',N'Thân quen',0)
Insert into KhachHang values('5E95F84F-F3E9-45AE-BA5C-EB86754A038E',N'Không Trông Thấy Rõ','11/9/2001',N'Nữ',N'Mỹ Tho, Tiền Giang','0987487610','0coemail@gmail.com',N'Thường',0)
Insert into KhachHang values('28CCB249-3091-4256-9EAB-F280873C23B8',N'Cường','11/9/1997','Nam',N'TaynguyenSound','0987477630','PeaCeeTNS@gmail.com',N'Vip',0)

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
	  DaXoa bit DEFAULT 0 not null,
	  constraint PK_MaNV primary key(MANV)	 
)
alter table NhanVien add constraint CK_GTNV check(GioiTinh = 'Nam' or GioiTinh=N'Nữ')

Insert into NhanVien values('59ACED03-FF2D-4DAA-A8EF-273B3685F468',N'Trần Đại Nghĩa','1/12/1989','Nam',N'65/19 Cao Bá Quát, Q8, TpHCM','0911228313','NghiaTDai1289@gmail.com',N'Sửa Xe','3/2/2018',7000000,0)
Insert into NhanVien values('90B6193C-0EBD-43F2-82A3-2DA131D767C6',N'Ngô Thanh Tuấn','12/6/1980','Nam',N'45 Nguyễn Cảnh Chân, Q1, TpHCM','0914102345','TuanNgoTh@gmail.com',N'Sửa Xe','10/2/2018',8500000,0)
Insert into NhanVien values('A25A94A7-6765-4D4E-BAD9-2E7A764C9742',N'Cao Thái Quý','12/1/1995','Nam',N'32/3 Trần Bình Trọng, Q5, TpHCM','0913476343','QuyCao95@gmail.com',N'Sửa Xe','11-5-2021',5000000,0)
Insert into NhanVien values('BDD24832-B4B9-4195-962C-91909DDE76C6',N'Lê Hoài Thương','27/7/1998',N'Nữ',N'65/19 Cao Bá Quát, Q8, TpHCM','0911228313','ThuongLe98Hoai@gmail.com',N'Văn Phòng','26/3/2021',5500000,0)
Insert into NhanVien values('89250069-C6CC-4D5D-BB9F-AA298DCE0D67',N'Nguyễn Bá Sang','11/8/1996','Nam',N'23 Hoàng Diệu, Q7, TpHCM','0856910975','SangNBa11896@gmail.com',N'Văn Phòng','3/2/2018',6000000,0)
Insert into NhanVien values('A5D54EA6-F80B-4A22-979B-D9FCB571FAB1',N'Phan Tấn Trung','26/2/1989','Nam',N'125/2 Hòa Hưng, Phuờng 12, Q10, TpHCM','0876701812','phantantrung3rb@gmail.com',N'Quản Lý','3/2/2018',12000000,0) /*Người Quản Lý*/
Insert into NhanVien values('02A1C26A-EE03-4D26-ADC2-E850C213EAF1',N'Hồ Thanh Hằng','19/8/2000',N'Nữ',N'Cao Ốc A Ngô Gia Tự, Q10, TpHCM','0947910521','HangTH2k@gmail.com',N'Văn Phòng','10/7/2019',5000000,0)

create table NhaSanXuat
(
     TenNSX varchar(15),
	 SDT  varchar(15),
	 Email varchar(45),
	 NuocSX NVarchar(15),
	 DaXoa bit DEFAULT 0 not null,
	 constraint PK_NSX primary key(TenNSX,NuocSX)
)

Insert into NhaSanXuat values('Yamaha','(30.24)38855080','YamahaMotorJapan@gmail.com',N'Nhật Bản',0)
Insert into NhaSanXuat values('Yamaha','(84.24)38855080','YamahaMotorVietNamCNVP@gmail.com',N'Việt Nam',0)
Insert into NhaSanXuat values('Suzuki','(84.89)47435209','SuzukiMotorVietNam@gmail.com',N'Việt Nam',0)
Insert into NhaSanXuat values('Honda','(84.2)18943170','HondaMotorThaiLand@gmail.com',N'Thái Lan',0)
Insert into NhaSanXuat values('Honda','(30.2)18943170','HondaMotorJapan@gmail.com',N'Nhật Bản',0)
Insert into NhaSanXuat values('Kawasaki','(0117)175','KawasakiJapan@gmail.com',N'Nhật Bản',0)
Insert into NhaSanXuat values('Kawasaki','(084)175','KawasakiJapan@gmail.com',N'Việt Nam',0)
Insert into NhaSanXuat values('Piaggio','(010)15432170','PiaggioItaly@gmail.com',N'Ý',0)
Insert into NhaSanXuat values('Sym','(123)170','SymTaiWan@gmail.com',N'Đài Loan',0)
Insert into NhaSanXuat values('Vinfast','(0631)17433170','VinfastVietNam@gmail.com',N'Việt Nam',0)
Insert into NhaSanXuat values('Sym','(084)170','SymVietNam@gmail.com',N'Việt Nam',0)

create table MatHang
(
    MaMH uniqueidentifier DEFAULT newid(),
	TenMH varchar(15),
	SoPhanKhoi int,
	GiaNhapMH money,
	GiaBanMH money,
	SoLuongTonKho int,
	HangSX /*TenNSX*/ varchar(15) not null,
	XuatXu /*NuocSX*/ Nvarchar(15) not null,
	MoTa Nvarchar(75),
	DaXoa bit DEFAULT 0 not null,
	constraint PK_MaMH primary key(MaMH)
)
alter table MatHang alter Column TenMH varchar(30)
alter table MatHang add constraint FK_MH foreign key(HangSX,XuatXu) references NhaSanXuat(TenNSX,NuocSX)

Insert into MatHang values('A626798B-7071-4710-88A8-7D31935A3019','Sirius', 110,21500000,26500000,13,'Yamaha',N'Nhật Bản',N'Vành đúc, phanh cơ, màu đỏ đen, Còn mới',0)
Insert into MatHang values('1B30FA5C-2C4E-42B1-97FE-864391FC4040','Sirius', 50,10000000,13500000,15,'Yamaha',N'Nhật Bản',N'Vành nan hoa, phanh cơ, màu đen xám, Còn mới',0)
Insert into MatHang values('01921D8D-F015-4309-A130-9390B2E8EC11','Honda Air Blade',150,33500000,40500000,7,'Honda',N'Nhật Bản',N'Vành đúc, phanh đĩa, màu vàng đen, Còn mới',0)
Insert into MatHang values('0FB80AA9-CC97-4157-8F04-BAA88F6E5266','Exciter', 150, 38500000,45000000,5,'Yamaha',N'Việt Nam',N'Vành đúc, phanh cơ, màu xanh trắng, Còn mới',0)
Insert into MatHang values('FAD9A8EF-6437-4A8F-ACB3-CC7232519EB6','Raider F150', 150, 21500000,27500000,10,'Suzuki',N'Việt Nam',N'Màu đỏ đen, Còn mới',0)

Insert into MatHang values('2BFB3B33-16E7-47B1-AFFF-02BD5B620E3A','Vision', 150,22500000,29500000,10,'Honda',N'Nhật Bản',N'Màu đỏ đen, Còn mới',0)
Insert into MatHang values('C8977C7A-526D-43ED-93F2-050865FF32DD','Lead', 110,24500000,31500000,10,'Honda',N'Nhật Bản',N'Màu trắng khói, Còn mới',0)
Insert into MatHang values('AA300482-E344-4359-A087-18ECD940116F','Kawasaki Z1000',1043,410000000,435500000,10,'Kawasaki',N'Nhật Bản',N'Màu xanh đen, Còn mới',0)
Insert into MatHang values('6CB1636D-94D4-45C5-A3C8-5BAE98D216F4','Kawasaki Ninja ZX-10R', 150, 699000000,729000000,5,'Kawasaki',N'Nhật Bản',N'Màu xanh lục đen, Còn mới',0)
Insert into MatHang values('414AC9FB-DC8E-4A5E-BDC3-6304079915EF','Attila 50', 110, 20500000,25700000,10,'Sym',N'Đài Loan',N'Màu xám khói, Còn mới',0)

Insert into MatHang values('24A8470F-B36B-4711-A96E-8F47936AE0D1','Vespa Print 2020', 110,23500000,30500000,10,'Piaggio',N'Ý',N'Màu đỏ đen, Còn mới',0)
Insert into MatHang values('2396CDDA-3989-4585-8F85-B8CC0A1A6B4A','SH 150', 150,115000000,129250000,9,'Honda',N'Nhật Bản',N'Màu vàng đen, Phanh Abs, còn mới',0)
Insert into MatHang values('D4C4275E-F248-4BA1-90E4-C8D31344A44D','Wave Alpha',110,20500000,24500000,15,'Honda',N'Nhật Bản',N'Màu trắng đen, Còn mới',0)
Insert into MatHang values('88426304-6459-4C23-8817-D1B24F8B6B01','Sirius FI', 110, 22500000,27500000,10,'Yamaha',N'Việt Nam',N'Màu đen khói, Còn mới',0)
Insert into MatHang values('62D0BEF7-AE0E-4F57-9C0B-E698E9CD6D11','EVO200', 100, 23500000,22500000,15,'Vinfast',N'Việt Nam',N'Màu vàng, Còn mới',0)

Create table HoaDon
(
      MaHD  uniqueidentifier DEFAULT newid(),
	  MaMH  uniqueidentifier not null,
	  MaKH  uniqueidentifier not null,                     /*Hoá đơn bán hàng cho khách hàng nào, Do nhân viên nào phụ trách*/
	  MaNV  uniqueidentifier not null,
      NgayLapHD  smalldatetime,
	  SoLuong int,                               /*số lượng xe*/
	  ThanhTien money,
	  constraint PK_MaHD primary key(MaHD)
)

Alter TABLE HOADON add constraint FK_MaMH foreign key(MaMH) references MatHang(MaMH)
ALTER TABLE HOADON ADD CONSTRAINT FK_MaKH foreign key(MAKH) references KhachHang(MAKH)
ALTER TABLE HOADON ADD CONSTRAINT FK_MaNV foreign key(MANV) references NhanVien(MANV)

Insert into HoaDon values('A5D5C04E-FC44-4184-8919-DA9982DF6FD0','A626798B-7071-4710-88A8-7D31935A3019','1B051393-0949-4E96-959A-6B001F82B177','BDD24832-B4B9-4195-962C-91909DDE76C6','19/8/2021',1,26500000)
Insert into HoaDon values('2D044E83-E0AB-45F1-9205-40B8812D6D4F','01921D8D-F015-4309-A130-9390B2E8EC11','955A14F5-F895-4F0B-B693-CE24A0B11321','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','17/10/2022',1,40500000)
Insert into HoaDon values('CE3C7C5D-2F71-491C-8AB5-515D0BD74468','0FB80AA9-CC97-4157-8F04-BAA88F6E5266','BDA8A3CC-9116-4215-99AB-351EF43154F5','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','15/10/2022',1,45000000)
Insert into HoaDon values('E8AEBDE7-AB35-495F-8019-0F9D9F8F97B9','88426304-6459-4C23-8817-D1B24F8B6B01','2F115694-CB81-44E8-9C09-472714E8C59C','02A1C26A-EE03-4D26-ADC2-E850C213EAF1','28/10/2021',1,27500000)
Insert into HoaDon values('DA440124-6228-443C-A14D-144965B9B9D1','D4C4275E-F248-4BA1-90E4-C8D31344A44D','BDA8A3CC-9116-4215-99AB-351EF43154F5','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','10/11/2021',1,24500000)

Insert into HoaDon values('15CFAEA6-FA4F-4A86-B184-15A410839A39','C8977C7A-526D-43ED-93F2-050865FF32DD','28CCB249-3091-4256-9EAB-F280873C23B8','BDD24832-B4B9-4195-962C-91909DDE76C6','20/11/2022',1,31500000)
Insert into HoaDon values('AC544BA1-DB8B-4DC8-A019-18F8C6D0955B','0FB80AA9-CC97-4157-8F04-BAA88F6E5266','BDA8A3CC-9116-4215-99AB-351EF43154F5','02A1C26A-EE03-4D26-ADC2-E850C213EAF1','8/3/2021',1,45000000) /**/
Insert into HoaDon values('AD43A732-60ED-478D-91C9-279BE10996B9','A626798B-7071-4710-88A8-7D31935A3019','11229832-8D41-4FD8-8DF4-714608BE9FC2','02A1C26A-EE03-4D26-ADC2-E850C213EAF1','14/2/2022',1,26500000)
Insert into HoaDon values('CB89344C-8079-4187-8EDA-2B4FD7CD1F2A','0FB80AA9-CC97-4157-8F04-BAA88F6E5266','28CCB249-3091-4256-9EAB-F280873C23B8','02A1C26A-EE03-4D26-ADC2-E850C213EAF1','10/4/2021',1,45000000)
Insert into HoaDon values('1178418A-7B4C-401E-B025-2FE06353451D','6CB1636D-94D4-45C5-A3C8-5BAE98D216F4','79B11F53-4E9E-46FA-9C0D-2B3B5617603A','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','14/4/2022',1,729000000)

Insert into HoaDon values('3F395068-12A6-4988-B55F-4A35C2C3363E','01921D8D-F015-4309-A130-9390B2E8EC11','C2B03BF4-A464-47FD-A772-7303EA0A534D','BDD24832-B4B9-4195-962C-91909DDE76C6','28/6/2022',1,40500000) /*8*/
Insert into HoaDon values('94825B1B-D7E9-431D-B8B5-4E10BDE4E1C2','414AC9FB-DC8E-4A5E-BDC3-6304079915EF','A3B9B7F9-2DEA-4E2A-91A9-620A4D0DC4A6','02A1C26A-EE03-4D26-ADC2-E850C213EAF1','15/2/2022',1,25700000)
Insert into HoaDon values('75DE016C-CCF6-4DE6-823A-502C45683305','1B30FA5C-2C4E-42B1-97FE-864391FC4040','F60D5B81-3662-427C-B696-06DA4F99BCF8','BDD24832-B4B9-4195-962C-91909DDE76C6','28/8/2022',1,13500000)
Insert into HoaDon values('AD9E5EE5-D760-4095-945B-5A3B0678A77B','24A8470F-B36B-4711-A96E-8F47936AE0D1','7A4F27C3-FDBB-4048-A1D4-6F2402109968','BDD24832-B4B9-4195-962C-91909DDE76C6','25/7/2021',1,30500000)
Insert into HoaDon values('DF4EE501-DB9B-49B6-8C2C-605599680011','62D0BEF7-AE0E-4F57-9C0B-E698E9CD6D11','B98FC8C5-EBFA-4E50-BB08-E94CB32371C8','02A1C26A-EE03-4D26-ADC2-E850C213EAF1','19/5/2022',1,22500000)

Insert into HoaDon values('884A4D93-BBE5-42C0-82C5-609AD05CFCBA','FAD9A8EF-6437-4A8F-ACB3-CC7232519EB6','7E87756D-A431-4A85-92DD-76CAC797AC0A','BDD24832-B4B9-4195-962C-91909DDE76C6','29/1/2021',1,27500000)  /*15*/
Insert into HoaDon values('6957A0FF-D2E5-4825-8451-645FD226B21E','414AC9FB-DC8E-4A5E-BDC3-6304079915EF','F60D5B81-3662-427C-B696-06DA4F99BCF8','02A1C26A-EE03-4D26-ADC2-E850C213EAF1','10/3/2022',1,25700000)
Insert into HoaDon values('F420B1E5-700B-4FA2-8A65-7A0B0F4298BE','FAD9A8EF-6437-4A8F-ACB3-CC7232519EB6','A7402648-6C90-4242-92CF-15EC15B43307','BDD24832-B4B9-4195-962C-91909DDE76C6','8/2/2021',1,27500000)
Insert into HoaDon values('1D7928C1-9426-4480-9F92-7BF437B69997','0FB80AA9-CC97-4157-8F04-BAA88F6E5266','ABCEEB43-F85D-40FA-A8F0-3EE468D954F3','BDD24832-B4B9-4195-962C-91909DDE76C6','11/5/2021',1,45000000)
Insert into HoaDon values('F38892C4-A21C-4E9B-B66F-9897BF3B921B','AA300482-E344-4359-A087-18ECD940116F','C2B03BF4-A464-47FD-A772-7303EA0A534D','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','31/5/2022',1,435500000)

Insert into HoaDon values('656C9D8E-E2BE-4BD0-8742-9950416438A3','1B30FA5C-2C4E-42B1-97FE-864391FC4040','BDA8A3CC-9116-4215-99AB-351EF43154F5','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','28/12/2022',1,13500000)  /*20*/
Insert into HoaDon values('D228AA06-83CD-40F9-ACA7-9951979F6F8E','FAD9A8EF-6437-4A8F-ACB3-CC7232519EB6','E36C38DB-F581-45B1-9698-4E3490426863','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','15/1/2022',1,27500000)
Insert into HoaDon values('3A3672FF-52CC-448B-8A62-A56B5FDFCD64','0FB80AA9-CC97-4157-8F04-BAA88F6E5266','BDA8A3CC-9116-4215-99AB-351EF43154F5','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','15/10/2022',1,45000000)
Insert into HoaDon values('0F0F724E-DBCE-464A-8466-B5F42ABA3C5F','24A8470F-B36B-4711-A96E-8F47936AE0D1','BC7A7504-075F-461D-8CE3-DD058730125C','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','25/8/2021',1,30500000)
Insert into HoaDon values('BC43B285-ACD8-41DE-803D-BF1FA1C7F309','1B30FA5C-2C4E-42B1-97FE-864391FC4040','61860DF3-DD85-4998-8746-2B215D674642','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','3/9/2022',1,13500000)

Insert into HoaDon values('94A2AE8C-3CE5-48C9-A28C-C22FF9AB08CC','D4C4275E-F248-4BA1-90E4-C8D31344A44D','BDA8A3CC-9116-4215-99AB-351EF43154F5','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','12/12/2021',1,24500000)    /*25*/
Insert into HoaDon values('E5EB7285-D7BB-4B0B-BDAB-C7A838B208E0','62D0BEF7-AE0E-4F57-9C0B-E698E9CD6D11','B8186750-FABE-470A-ADD4-349C280FB09D','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','28/6/2021',1,22500000)
Insert into HoaDon values('C20703D2-76D3-4042-9D74-C885AEEF9E03','2396CDDA-3989-4585-8F85-B8CC0A1A6B4A','BDA8A3CC-9116-4215-99AB-351EF43154F5','02A1C26A-EE03-4D26-ADC2-E850C213EAF1','17/7/2021',1,129250000)
Insert into HoaDon values('44195134-1CEA-4EEC-BC6D-CA6DBF767194','C8977C7A-526D-43ED-93F2-050865FF32DD','7009D6E2-CAA8-4479-A62A-2AE40FF22305','BDD24832-B4B9-4195-962C-91909DDE76C6','25/11/2022',1,31500000)
Insert into HoaDon values('A26E4EF0-65D5-48A4-A6B5-CC26D91920D0','A626798B-7071-4710-88A8-7D31935A3019','A7402648-6C90-4242-92CF-15EC15B43307','BDD24832-B4B9-4195-962C-91909DDE76C6','5/9/2022',1,26500000)

Insert into HoaDon values('681FF8EF-4389-47AE-9F90-CC74B599DF0F','D4C4275E-F248-4BA1-90E4-C8D31344A44D','BDA8A3CC-9116-4215-99AB-351EF43154F5','BDD24832-B4B9-4195-962C-91909DDE76C6','19/1/2022',1,24500000) /*30*/
Insert into HoaDon values('2EF593AD-BF50-4660-A93E-D85AD31A7BE7','2BFB3B33-16E7-47B1-AFFF-02BD5B620E3A','A7402648-6C90-4242-92CF-15EC15B43307','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','5/9/2021',1,29500000)
Insert into HoaDon values('02B8C74E-C284-4CE5-A169-F8DE999C6769','A626798B-7071-4710-88A8-7D31935A3019','1B051393-0949-4E96-959A-6B001F82B177','BDD24832-B4B9-4195-962C-91909DDE76C6','19/8/2022',1,26500000)
Insert into HoaDon values('08292E6A-9C25-40D4-BE5C-E59311298BD3','D4C4275E-F248-4BA1-90E4-C8D31344A44D','249CADA6-53D5-4D9E-85B9-1AF5BFB067C0','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','27/7/2022',1,24500000)
Insert into HoaDon values('07C0D47C-2666-4DB7-8DDD-EC8FDFCE85A9','1B30FA5C-2C4E-42B1-97FE-864391FC4040','BDA8A3CC-9116-4215-99AB-351EF43154F5','89250069-C6CC-4D5D-BB9F-AA298DCE0D67','20/8/2022',1,13500000)

Create table ThongTinBaoHanh
(
     MaBH  uniqueidentifier DEFAULT newid(),
	 MaMH  uniqueidentifier not null,
	 MaKH  uniqueidentifier not null,
	 MaNV  uniqueidentifier not null,
	 ThoiGian smalldatetime,
	 GhiChu nvarchar(60),
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
  MaNV uniqueidentifier,
  UserName varchar(15),
  Password varchar(20),
  Email  Varchar(30),   /*Email để khôi phục mật khẩu nếu cần*/
  constraint PK_UserName primary key(UserName)
)

alter table UserApp add constraint FK_UA_MaNV foreign key(MaNV) references NhanVien(MaNV)

Insert into UserApp values('A5D54EA6-F80B-4A22-979B-D9FCB571FAB1','Ngquanly1','123456ABCDEF','phantantrung3rb@gmail.com') /*NV Quản Lý*/ 
Insert into UserApp values('BDD24832-B4B9-4195-962C-91909DDE76C6','Nhvien1','ABCDEF123456','ThuongLe98Hoai@gmail.com')  /*NV Văn Phòng*/
Insert into UserApp values('89250069-C6CC-4D5D-BB9F-AA298DCE0D67','Nhvien2', 'Englandvodich','SangNBa11896@gmail.com')   /*NV Văn Phòng*/
Insert into UserApp values('02A1C26A-EE03-4D26-ADC2-E850C213EAF1','Nhvien3', 'matkhaudenho','HangTH2k@gmail.com')   /*NV Văn Phòng*/

create table DonDatHang
( 
  MaDonDH uniqueidentifier DEFAULT newid(),
  MaMH  uniqueidentifier not null,
  SoLuongHang int,
  MaKH uniqueidentifier not null,
  MaNV uniqueidentifier not null,
  NGDH smalldatetime,                                          /*Ngày Đặt Hàng*/                     
  constraint PK_MaDonDH primary key(MaDonDH)                  /*Phần này để nhân viên tương tác với Khách, chỉ nhân viên tương tác với khách mới có quyền thêm xoá sửa*/
)

alter table DonDatHang add constraint FK_MaMHDDH foreign key(MaMH) references MatHang(MaMH)
alter table DonDatHang add constraint FK_MaKHDDH foreign key(MaKH) references KHACHHANG(MaKH)
alter table DonDatHang add constraint FK_MaNVDDH foreign key(MaNV) references NHANVIEN(MaNV)

Insert into DonDatHang values(newid(),'01921D8D-F015-4309-A130-9390B2E8EC11',1,'8507F1FB-C2CC-48AF-B285-DC15AFF4EC77','90B6193C-0EBD-43F2-82A3-2DA131D767C6','25/11/2022')

Create Table LenLich
(
  LenLichID uniqueidentifier DEFAULT newid(),
  MaNV uniqueidentifier not null,          /*Mã Nhân Viên để biết ai đã lên lịch, tiện cho tính năng xem lịch sử hoạt động sau này*/
  NgLenLichBD smalldatetime,      /*Ngày, giờ bắt đầu*/
  NgLenLichKT smalldatetime,      /*Ngày, giờ kết thúc*/
  NoiDungLenLich nvarchar(200),   /*Nội dung lên lịch hôm đó*/
  CONSTRAINT PK_LenLichID primary key(LenLichID)
)
alter table LenLich add constraint FKLL_MaNV foreign key(MaNV) references NhanVien(MaNV)

Create Table LichSuHoatDong
(
  LshdID uniqueidentifier DEFAULT newid(),
  MaNV uniqueidentifier not null,
  ThoiGian datetime,
  HoatDong nvarchar(200),
  CONSTRAINT PK_LshdID primary key(LshdID)
)
alter table LichSuHoatDong add constraint FKLSHD_MaNV foreign key(MaNV) references NhanVien(MaNV)
