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
- **Login**: `Dũng`

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