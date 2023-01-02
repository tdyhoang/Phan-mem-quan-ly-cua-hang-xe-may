using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using System.IO;
using MotoStore.Database;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOAddSPPage.xaml
    /// </summary>
    public partial class IOAddSPPage : Page
    {
        public IOAddSPPage()
        {
            InitializeComponent();
          
        }
        private void btnLoadImageSP_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new(openFileDialog.FileName);
                ImageSP.Source = new BitmapImage(fileUri);
                File.Copy(openFileDialog.FileName, "F:\\New folder\\Phan-mem-quan-ly-cua-hang-xe-may-main\\src\\MotoStore\\Views\\Pages\\IO_Images\\Temp.png");
            }
        }
       

            private void btnAddNewSP_Click(object sender, RoutedEventArgs e)
        {

            bool check = true;
            SqlConnection con = new(System.Configuration.ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            SqlCommand cmd;
            if ((string.IsNullOrWhiteSpace(txtTenSP.Text)) || (string.IsNullOrWhiteSpace(txtGiaNhapSP.Text)) || (string.IsNullOrWhiteSpace(cmbXuatXuSP.Text)) || (string.IsNullOrWhiteSpace(cmbHangSXSP.Text)) || (string.IsNullOrWhiteSpace(txtMoTaSP.Text)) || (string.IsNullOrWhiteSpace(txtPhanKhoiSP.Text)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
            }
            else
            {
                for (int i = 0; i < txtTenSP.Text.Length; i++)//Check Tên SP
                {
                    if ((txtTenSP.Text[i] >= 48 && txtTenSP.Text[i] <= 57))
                    {
                        MessageBox.Show("Tên Sản Phẩm không được chứa các ký tự số! ");
                        check = false;
                    }
                }


                for (int i = 0; i < txtGiaNhapSP.Text.Length; i++) //Check Giá Nhập SPham
                {
                    if (!(txtGiaNhapSP.Text[i] >= 48 && txtGiaNhapSP.Text[i] <= 57))
                    {
                        MessageBox.Show("Giá Sản Phẩm không được chứa các ký tự ");
                        check = false;
                    }
                }
               
              

                if (check)
                {
                    string MaMH = Guid.NewGuid().ToString();
                    File.Move("F:\\New folder\\Phan-mem-quan-ly-cua-hang-xe-may-main\\src\\MotoStore\\Views\\Pages\\IO_Images\\Temp.png", "F:\\New folder\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + MaMH+".png");

                    con.Open();
                    cmd = new("Set Dateformat dmy\nInsert into MatHang values('" + MaMH +  "', N'" + txtTenSP.Text + "','"  + txtPhanKhoiSP.Text + "','" + txtGiaNhapSP.Text  + "','" + cmbHangSXSP.Text + "',N'" + cmbXuatXuSP.Text + "','" + txtMoTaSP.Text + " ' )", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Thêm dữ liệu thành công");
                }
            }
        }
    }
}
