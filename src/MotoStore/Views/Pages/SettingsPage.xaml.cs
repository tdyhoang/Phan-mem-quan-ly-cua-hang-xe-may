using Microsoft.Win32;
using System.Net.Mail;
using System.Net;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Wpf.Ui.Common.Interfaces;
using MotoStore.Database;
using Microsoft.Data.SqlClient;
using MotoStore.Views.Pages.LoginPages;
using System.Windows.Documents;
using System.Net.Mime;
using System.IO;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
    {
        static string fileImg;
        public ViewModels.SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        static int solanbam = 0;
        private void btnBaoLoi_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(solanbam == 0)
            {
                brdBaoLoi.Visibility = Visibility.Visible;
                solanbam = 1;
            }
            else
            {
                brdBaoLoi.Visibility = Visibility.Collapsed;
                solanbam = 0;
            }
        }

        private void btnDinhKemAnh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog OFD = new();
            OFD.Filter = "JPG File (*.jpg)|*.jpg|JPEG File (*.jpeg)|*.jpeg|PNG File (*.png)|*.png";
            if (OFD.ShowDialog() == true)
            {
                anhDinhKem.ImageSource=new BitmapImage(new System.Uri(OFD.FileName));
                fileImg=OFD.FileName;
            }
        }

        private void btnGui_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn Có Chắc Muốn Gửi ?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
            }
            else
            {
                GuiMail(fileImg);
            }    
        }

        private void GuiMail(string fileanh)
        {
            try
            {
                MailMessage mess = new();
                string getMail = "123";
                MainDatabase mdb = new();
                SqlConnection con = new(Properties.Settings.Default.ConnectionString);
                SqlCommand cmd = new("Select Email from UserApp where MaNV=@manv", con);
                cmd.Parameters.Add("@manv", System.Data.SqlDbType.VarChar);
                cmd.Parameters["@manv"].Value = PageChinh.getMa;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    getMail = (string)reader[0];
                con.Close();

                mess.From = new(getMail);     //Đây là Email gửi từ ứng dụng
                string richText = new TextRange(rtbLoiGopY.Document.ContentStart, rtbLoiGopY.Document.ContentEnd).Text;
                mess.Subject = "Về Việc Báo Lỗi, Góp Ý Phần Mềm:";
                mess.Body = richText;

                mess.To.Add(new("datrua3152003@gmail.com"));  //Email của ứng dụng
                SmtpClient smtpClient1 = new("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(getMail, "vxiwantwodhltrnb"),
                    EnableSsl = true,
                };

                string attachmentPath = fileanh;
                Attachment inline = new Attachment(attachmentPath);
                inline.ContentDisposition.Inline = true;
                inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                //inline.ContentId = contentID;
                inline.ContentType.MediaType = "image/png";
                inline.ContentType.Name = Path.GetFileName(attachmentPath);
                mess.Attachments.Add(inline);
                smtpClient1.Send(mess);
                MessageBox.Show("Chúng tôi đã ghi nhận về lỗi, đóng góp của bạn, xin cảm ơn!");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Gửi mail thất bại, có thể do dung lượng file ảnh hoặc một vài lý do khác, hãy thử tệp ảnh nhẹ hơn và gửi lại!");
            }
            /* Hàm này để ứng dụng gửi mã 6 số ngẫu nhiên
              về Email người Quản Lý để xác nhận thay đổi mật khẩu */
        }

    }
}