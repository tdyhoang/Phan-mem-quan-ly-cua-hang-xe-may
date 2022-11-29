# Phan mem quan ly cua hang xe may
*Recommended Markdown Viewer: [Markdown Editor](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor2)*
 
## How to use the newly changed Toolkit
- Replace `using Windows.Toolkit.Mvvm` with `using CommunityToolkit.Mvvm`
- Use `RelayCommand` instead of `ICommand`

## TODO
- Dashboard View
- Login Page
- Database
- Click & Navigating Commands

Vài Điểm Đúc Kết Được Từ Buổi Review Đồ Án Thứ 4 ngày 23/11/2022:

-Giải thích rõ ràng đối tượng người dùng mà ứng dụng nhắm đến, lợi ích của ứng dụng, ứng dụng có thân thiện với người dùng không là 1 điểm cộng  
-Trang dữ liệu làm dưới dạng lưới, cho người dùng thao tác trên đó cũng là 1 điểm cộng.  
-Khi dữ liệu bị thay đổi(cập nhật, xoá, sửa) thì cũng ảnh hưởng trên Database.  
-Database không cần quá chuyên sâu, cầu kì vì hầu như cả lớp đều trên tư tưởng vừa học vừa làm.  
-Kiểm tra Bố Cục rành mạch, Đẹp Mắt thì càng tốt, không được đơn giản quá.  
-Để ý các trường hợp người dùng nhập liệu sai sót(số xe tồn kho <0, blah blah ...), chú ý dữ liệu đầu vào.  
-Nếu được, hãy thêm vài tính năng đặc biệt cho ứng dụng, sẽ có điểm cộng cho phần này.  
-Clean Code cũng 1 điểm cộng, các hàm mạch lạc rõ ràng để trong tương lai có thể dễ dàng sửa chữa, bảo trì.  
-Áp dụng các kiến thức đã học như OOP, thuật toán đơn giản vào ứng dụng thì càng tốt.  
-Các thành viên nhóm ai cũng phải giải thích đc một vài tính năng của ứng dụng.  
-Thầy chấm trên tư tưởng thoải mái nhưng ứng dụng thì không đc đơn giản quá.  

## Design ideas:
- **Dashboard Page**:
	+ Báo cáo doanh thu bằng *chart*
	+ Các chức năng cho *account*: đổi mật khẩu, đăng xuất
	+ *Shortcut* tới các page khác
## Assign:
- **Commands**: `Hoàng`
- **Models**: `Hoàng`
- **Views**: `Hoàng`
- **ViewModels**: `Hoàng`
- **Database**: `Đạt`
- **Login**: `Dũng`,`Đạt`

## Building the app
*Make sure to have finished those steps before building the app*
- **Step 1**: In `Tools->NuGet Package Manager->Manage NuGet Packages for Solution...` make sure to have installed *all* those Packages:
	- `CommunityToolkit.Mvvm` by `Microsoft`
	- `LiveChartsCore.SkiaSharpView.WPF` by `BetoRodriguez`
	- `Microsoft.Data.SqlClient` by `Microsoft`
	- `Microsoft.Extensions.Hosting` by `Microsoft`
	- `WPF-UI` by `lepo.co`
- **Step 2**: Connect to your local *SQL Server* (using SSMS, remember the *Server name*), go to `File->Open->File...`, the file is located in `.\MotoStore\Databases\DatabasePMQLCHBXM.sql`. Run the code (Ctrl+A->F5) and that's it.
- **Step 3**: Build the sln, remember to enter the server name before trying to access the *Data Page* (Dữ Liệu)
