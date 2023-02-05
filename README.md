# Đồ Án Lập Trình Trực Quan - IT008 (Nhóm 12)
Tên Phần Mềm: MotoStore - Phần mềm Quản Lý Cửa Hàng Xe Máy

## Mô Tả
MotoStore là một phần mềm Windows được phát triển để hỗ trợ các cửa hàng xe máy trong việc quản lý dữ liệu cho một cửa hàng xe máy.

## Cài Đặt
### Cấu Hình Tối Thiểu
Processor: Trên 800MHz
Memory: 512MB
GPU: DirectX 9 capable
Windows Vista/7/8/8.1/10/11 or newer

### Phần mềm phải có
[.Net Desktop Runtime 6](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.13-windows-x64-installer)
[SQL Server Express](https://go.microsoft.com/fwlink/p/?linkid=2216019&clcid=0x409&culture=en-us&country=us)
[SQL Server Management Studio (SSMS)](https://aka.ms/ssmsfullsetup)

### Chạy lần đầu
Mở SQL Server Management Studio (SSMS), Vào `File` -> `Open` -> `File...` (hoặc nhấn Ctrl+O) sau đó chọn file `Main Database.sql` và Execute

Để chạy phần mềm, mở file `/src/MotoStore.sln` và build nó

Database đã được định nghĩa sẵn với dữ liệu mẫu. Có 4 tài khoản mặc định để chạy lần đầu bao gồm:
1. Tài khoản quản lý: Username: ngquanly1 Password: 123456ABCDEF
2. Tài khoản thường: Username: nhvien1 Password: ABCDEF123456
3. Tài khoản thường: Username: nhvien2 Password: Englandvodich
4. Tài khoản thường: Username: nhvien3 Password: matkhaudenho

Khi mở phần mềm lần đầu, các đường dẫn cho những ảnh avatar và mặt hàng sẽ chưa được định nghĩa sẵn. Do đó việc đầu tiên là vào trang Cài Đặt (ở dưới cùng) và chọn các đường dẫn ở đó. Trong source code cũng có sẵn 2 folder ảnh mẫu để chạy thử lần đầu, với avatar và ảnh mặt hàng lần lượt ở `/src/MotoStore/Avatars` và `/src/MotoStore/Product Images`

Ngoài ra ở trang Cài Đặt còn có nút `Báo Lỗi / Góp Ý`, nếu gặp lỗi hoặc có góp ý cải thiện phần mềm bạn có thể gửi cho chúng tôi. Mọi thông tin đóng góp đều rất quan trọng để sản phẩm hoàn thiện hơn trong tương lai và chúng tôi rất trân trọng ý kiến của bạn.