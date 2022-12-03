# Phan mem quan ly cua hang xe may
*Recommended Markdown Viewer: [Markdown Editor](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor2)*

## Lưu ý!!!
### Trước khi commit change lên github:
- Xóa các folder:
	+ `.vs`
	+ `Demo\bin`
	+ `Demo\obj`
	+ `MotoStore\bin`
	+ `MotoStore\obj`
- Chọn đúng những file mình đã sửa đổi, đôi khi có những file đc tạo tự động như file .g thì cứ tick
- Mục đích là để tránh trường hợp ghi đè file của nhau dẫn đến mất code, mất thời gian để sửa
- Ae chỉnh sửa phần nào thì tạo branch mang tên phần đó, commit lên đó trước rồi vào Pull requests -> Chọn New Pull request -> bên phần compare chọn branch vừa up lên của mình -> Chọn Create pull request  và chờ nó kiểm tra xem có conflict hay không. Nếu không thì chọn Merge pull request -> Confirm merge


### Về file Resources.resx
- Mn nhớ sử dụng file này cho các tài nguyên liên quan tới giao diện như màu sắc, hình nền, icon... hoặc những chuỗi (string) được sử dụng nhiều nơi. Có thể xem đây là 1 thư viện tổng cho chương trình.

## Yêu cầu chung
- Giải thích rõ ràng đối tượng người dùng mà ứng dụng nhắm đến, lợi ích của ứng dụng, ứng dụng có thân thiện với người dùng không là 1 điểm cộng  
- Trang dữ liệu làm dưới dạng lưới, cho người dùng thao tác trên đó cũng là 1 điểm cộng.  
- Khi dữ liệu bị thay đổi(cập nhật, xoá, sửa) thì cũng ảnh hưởng trên Database.  
- Database không cần quá chuyên sâu, cầu kì vì hầu như cả lớp đều trên tư tưởng vừa học vừa làm.  
- Kiểm tra Bố Cục rành mạch, Đẹp Mắt thì càng tốt, không được đơn giản quá.  
- Để ý các trường hợp người dùng nhập liệu sai sót(số xe tồn kho <0, blah blah ...), chú ý dữ liệu đầu vào.  
- Nếu được, hãy thêm vài tính năng đặc biệt cho ứng dụng, sẽ có điểm cộng cho phần này.  
- Clean Code cũng 1 điểm cộng, các hàm mạch lạc rõ ràng để trong tương lai có thể dễ dàng sửa chữa, bảo trì.  
- Áp dụng các kiến thức đã học như OOP, thuật toán đơn giản vào ứng dụng thì càng tốt.  
- Các thành viên nhóm ai cũng phải giải thích đc một vài tính năng của ứng dụng.  
- Thầy chấm trên tư tưởng thoải mái nhưng ứng dụng thì không đc đơn giản quá.  

## Ý tưởng thiết kế
- **Dashboard**
	+ Các chức năng cho *account*: đăng nhập, đổi mật khẩu, đăng xuất
	+ Tính năng đặt lịch, quản lý giờ giấc(Khi đến ngày đó sẽ có thông báo nhắc nhở)
	+ Tính năng hướng dẫn người dùng sử dụng phần mềm
	+ Biểu đồ quản lý doanh thu theo(ngày, tháng, năm)
	+ Biểu đồ quản lý nhân viên năng suất(có thể cần) 
	+ 
- **Data**
	+ Sử dụng bảng, có các chức năng filter, sắp xếp, thêm xóa sửa
        + Tìm kiếm có nhiều loại(tìm theo tên sp, tìm theo dạng sản phẩm(xe tay ga, xe số, xe điện, xe mô tô) tìm theo giá,...  
	+ Thêm hình ảnh mô tả sản phẩm cho sinh động(có thể cần)
	
## Phân công
- **Dashboard**:
	+ **Login**: Dũng, Đạt
- **Trang Data**: Hoàng
- **Nhập xuất hàng**:
- **Settings**:
- **Giao diện**:
