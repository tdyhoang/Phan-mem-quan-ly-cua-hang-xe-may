# Phan mem quan ly cua hang xe may

## Lưu ý khi commit
- Nếu đã lỡ commit lên github trước khi pull phần của ng khác về thì ae có thể vào phần history, sẽ thấy có 2 commit mới chưa đc push lên. Trong đó 1 là commit của mình và 2 là cái merge tự động của github. Backup file lại phòng khi làm sai sau đó thực hiện các bước để khắc phục:
	+ B1: Undo commit merge tự động
	+ B2: Qua phần changes và discard tất cả các file ở đó
	+ B3: Undo commit phần mình vừa làm
	+ B4: Thực hiện pull cập nhật sau đó commit và push phần của mình như bình thường

## CustomValidationRule
### Cách dùng
- Nằm ở namespace helpers, file này sẽ tổng hợp toàn bộ các ValidationRule và sẽ có thuộc tính ValidationMode để chọn loại ValidationRule muốn áp dụng

## TextBoxInputBehavior
### Tác dụng
- Ngăn các ký tự không hợp lệ ở các textbox, hợp lệ hay không do tự mình quy định trong class này, tiện để dùng chung và kế thừa
- Cụ thể hơn: dùng trong textbox nhập số lượng, số tiền, số điện thoại, v.v. trong 2 trang `Nhập xuất` và `Danh mục`

### Cách dùng
- Ở file xaml thêm 2 namespace là `xmlns:i="http://schemas.microsoft.com/xaml/behaviors"` và `xmlns:helpers="clr-namespace:MotoStore.Helpers"`
- Trong textbox mở thêm 1 control như sau:  
	<TextBox ...>
		<i:Interaction.Behaviors>
			<helpers:TextBoxInputBehavior InputMode="..." JustPositiveDecimalInput="..." />
		</i:Interaction.Behaviors>
	</TextBox>
- Có 9 InputMode:
    + `None`: Textbox gõ như bình thường, không có điều kiện
	+ `NonSpecialInput`: chỉ cho phép chữ, số hoặc khoảng trắng, không cho phép các ký tự đặc biệt khác
	+ `DigitInput`: Textbox chỉ cho phép nhập các ký tự là chữ số
	+ `LetterInput`: Textbox chỉ cho phép nhập các ký tự là chữ cái
	+ `WordsInput`: Textbox chỉ cho phép nhập các ký tự là chữ cái hoặc khoảng trắng (dùng cho các loại tên, loại khách hàng,...)
	+ `LetterOrDigitInput`: Textbox không cho phép nhập các ký tự đặc biệt (để nhập mã...)
	+ `DecimalInput`: Textbox chỉ cho phép nhập số thập phân
	+ `IntegerInput`: Textbox chỉ cho phép nhập số nguyên
	+ `DateInput`: Textbox chỉ cho phép nhập các ký tự là chữ số hoặc dấu chéo `/` (để nhập ngày tháng)
	+ Ae có thể vào file `Helpers\TextBoxInputBehavior.cs` để tự định nghĩa thêm InputMode nếu cần
- Thuộc tính bool `JustPositiveDecimalInput` chỉ dùng khi InputMode=DecimalInput hoặc InputMode=IntegerInput, nếu là `True` thì không cho phép nhập dấu trừ `-`, ngược lại nếu là `False` thì cho phép (để nhập số âm). Giá trị mặc định là false.

## DateTimeConverter
### Tác dụng
- Trong textbox: Nhận diện chuỗi nếu không phải là 1 ngày theo định dạng d/M/yyyy thì sẽ tự xóa
- Trong các control có hiển thị chuỗi: Tự động chuyển chuỗi DateTime gốc thành định dạng dd/MM/yyyy để hiển thị ngày tháng đúng định dạng

### Cách dùng
- Ở file xaml thêm namespace `xmlns:helpers="clr-namespace:MotoStore.Helpers"`
- Thêm 1 Page.Resource (tham khảo google) như sau: <helpers:DateTimeConverter x:Key="DateTimeConverter" />
- Khi binding bất kỳ control nào với 1 biến DateTime thì binding dưới dạng: Binding="{Binding `Biến cần binding`, Converter={StaticResource DateTimeConverter}}"

## Việc cần làm
- Danh sách đã đc sắp xếp theo thứ tự ưu tiên
- Danh sách không bao gồm các việc cần làm ở phần danh mục vì nó cũng khá phức tạp và t sẽ tự quyết định nên thay đổi gì ở đó
- Những việc nằm ngoài danh sách này gần như chỉ là những yếu tố phụ (cộng thêm điểm) và sẽ không nên được ưu tiên vào thời điểm này
- Nếu ai cảm thấy trang của mình ổn rồi thì nên qua giúp những trang khác trước khi quyết định phát triển 1 tính năng thêm

### Thay đổi chung
1. Cho phép người dùng custom ID ở phần cài đặt
2. Đồng bộ lại các connection string theo như chuỗi ở mục `Connection string`
3. Sửa lại toàn bộ đường dẫn file thành đường dẫn tương đối (relative)

### Nhập xuất
1. Ở mục sản phẩm khi click vào 1 sản phẩm sẽ hiện thông tin cơ bản của sp đó, có 2 nút là nhập thêm hàng và tạo hóa đơn
2. Xóa 2 textbox số lượng và giá bán ở mục thêm mới sản phẩm
3. Xử lý kiểm tra điều kiện số lượng ở hóa đơn không được phép lớn hơn số lượng hàng tồn kho, và mỗi lần tạo hóa đơn mới là tự động trừ đi lượng hàng tồn
4. Các textbox nhập ID sửa lại thành combobox, hiển thị thành từng thẻ có vài thông tin quan trọng và cũng cho phép người dùng nhập vào để tìm kiểm (filter)
5. Xử lý lostfocus và keydown ở các textbox phần nhập xuất, đánh dấu * vào những ô bắt buộc phải có dữ liệu (not null)

### Báo cáo
1. Cải thiện thêm phần đồ thị, hiển thị các cột thưa ra, cho phép zoom theo trục hoành

### Dashboard
1. Xem lại phần thay đổi avatar, lên lịch và lịch sử hoạt động

## Về file Resources.resx
- Mn nhớ sử dụng file này cho các tài nguyên liên quan tới giao diện như màu sắc, hình nền, icon... hoặc những chuỗi (string) được sử dụng nhiều nơi. Có thể xem đây là 1 thư viện tổng cho chương trình.

## Connection string
- Sử dụng string dưới đây thay cho 1 connection string thông thường sẽ có những lợi ích sau:
    + Tiết kiệm thời gian code
	+ Code sẽ được đồng bộ, khi test tránh trường hợp connection string khác nhau dẫn đến chỗ có bug chỗ không
	+ Code gọn và dễ nhìn, dễ hiểu hơn
- `Properties.Settings.Default.ConnectionString`

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