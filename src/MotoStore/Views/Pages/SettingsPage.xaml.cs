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
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Shapes;
using MotoStore.Properties;
using Path = System.IO.Path;

namespace MotoStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
    {
        static string? fileImg = null;
        public ViewModels.SettingsViewModel ViewModel
        {
            get;
        }

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            txtDgDanAvatar.Text = Settings.Default.AvatarFilePath;
            txtDgDanMH.Text = Settings.Default.ProductFilePath;
        }

        static int solanbam = 0;
        private void btnBaoLoi_Click(object sender, RoutedEventArgs e)
        {
            if (solanbam == 0)
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

        private void btnDinhKemAnh_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog OFD = new();
            OFD.Filters.Add(new("Image File", "jpg,jpeg,png"));
            if (OFD.ShowDialog() == CommonFileDialogResult.Ok)
            {
                anhDinhKem.ImageSource = new BitmapImage(new Uri(OFD.FileName));
                fileImg = OFD.FileName;
            }
        }

        private void btnGui_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn Có Chắc Muốn Gửi ?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                GuiMail(fileImg);
        }

        private void GuiMail(string fileanh)
        {
            try
            {
                MailMessage mess = new();
                string getMail = "123";
                MainDatabase mdb = new();
                SqlConnection con = new(Settings.Default.ConnectionString);
                SqlCommand cmd = new("Select Email from UserApp where MaNV=@manv", con);
                cmd.Parameters.Add("@manv", System.Data.SqlDbType.VarChar);
                cmd.Parameters["@manv"].Value = PageChinh.getNV.MaNv;
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
                Attachment inline = new(attachmentPath);
                inline.ContentDisposition.Inline = true;
                inline.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                //inline.ContentId = contentID;
                inline.ContentType.MediaType = "image/png";
                inline.ContentType.Name = Path.GetFileName(attachmentPath);
                mess.Attachments.Add(inline);
                smtpClient1.Send(mess);
                MessageBox.Show("Chúng tôi đã ghi nhận về lỗi, đóng góp của bạn, xin cảm ơn!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gửi mail thất bại, có thể do dung lượng file ảnh hoặc một vài lý do khác, hãy thử tệp ảnh nhẹ hơn và gửi lại! Lỗi: " + ex.Message);
            }
            /*Hàm này để ứng dụng gửi mã 6 số ngẫu nhiên
            về Email*/
        }
                        
        private void btnChonFileAvatar_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog COFD = new()
            {
                IsFolderPicker = true,
                InitialDirectory = Settings.Default.AvatarFilePath
            };
            if (COFD.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    Settings.Default.AvatarFilePath = COFD.FileName;
                    Settings.Default.Save();
                    txtDgDanAvatar.Text = Settings.Default.AvatarFilePath;
                    MessageBox.Show("Thay Đổi Đường Dẫn Ảnh Nhân Viên Thành Công!");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Thay Đổi Đường Dẫn Thất Bại, Lỗi: " + ex.Message);
                }
            }
        }

        private void btnChonFileMH_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog COFD = new()
            {
                IsFolderPicker = true,
                InitialDirectory = Settings.Default.ProductFilePath
            };
            if (COFD.ShowDialog() == CommonFileDialogResult.Ok)
            {
                try
                {
                    Settings.Default.ProductFilePath = COFD.FileName;
                    Settings.Default.Save();
                    txtDgDanMH.Text = Settings.Default.ProductFilePath;
                    MessageBox.Show("Thay Đổi Đường Dẫn Ảnh Mặt Hàng Thành Công!");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Thay Đổi Đường Dẫn Thất Bại, Lỗi: " + ex.Message);
                }
            }
        }
    }
}