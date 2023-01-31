using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Data.SqlClient;
using MotoStore.Views.Pages.LoginPages;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IONhaSXPage.xaml
    /// </summary>
    public partial class IONhaSXPage : Page
    {
        internal List<Control> InputFields;
       
        public IONhaSXPage()
        {

            InitializeComponent();
            PageInitialization();   
        }
        private void PageInitialization()
        {
            InputFields = new()
            {
                txtTenNCC,
                txtSDTNSX,
                txtEmailNSX,
                txtDiaChi
            };
        }


        private void btnAddNewNSX_Click(object sender, RoutedEventArgs e)
        {
            foreach (var c in InputFields.Where(c => Validation.GetHasError(c)))
            {
                MessageBox.Show("Dữ liệu đang có lỗi, không thể thêm!");
                return;
            }         
            if(string.IsNullOrEmpty(txtTenNCC.Text) || string.IsNullOrEmpty(txtEmailNSX.Text))
            {
                MessageBox.Show("Các ô được đánh dấu * không được để trống!");
                return;
            }    
          
            using SqlConnection con = new(Properties.Settings.Default.ConnectionString);
            SqlCommand cmd;         
            {
                try
                {
                    con.Open();
                    using var trans = con.BeginTransaction();
                    try
                    {
                        cmd = new("Set Dateformat dmy\nInsert into NhaCungCap values(@TenNCC,@SDT,@Email,@DiaChi,0)", con, trans);
                        cmd.Parameters.Add("@TenNCC", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters["@TenNCC"].Value = txtTenNCC.Text;
                        cmd.Parameters.Add("@SDT", System.Data.SqlDbType.VarChar);
                        cmd.Parameters["@SDT"].Value = txtSDTNSX.Text;
                        cmd.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters["@Email"].Value = txtEmailNSX.Text;
                        cmd.Parameters.Add("@DiaChi", System.Data.SqlDbType.NVarChar);
                        cmd.Parameters["@DiaChi"].Value = txtDiaChi.Text;
                        cmd.ExecuteNonQuery();

                        cmd = new SqlCommand("Select top(1) MaNCC from NhaCungCap order by ID desc", con, trans);
                        SqlDataReader sda = cmd.ExecuteReader();
                        string NCCMoi = "NCC@";
                        if (sda.Read())
                        {
                            NCCMoi = (string)sda[0];
                            sda.Close();    
                        }
                        DateTime now = DateTime.Now;
                        cmd = new($"Set Dateformat dmy\nInsert into LichSuHoatDong values(NEWID(), '{PageChinh.getNV.MaNv}', '{now:dd-MM-yyyy HH:mm:ss}', N'thêm mới Nhà Cung Cấp " + NCCMoi + "')", con, trans);
                        cmd.ExecuteNonQuery();
                        trans.Commit();

                        
                        MessageBox.Show("Thêm dữ liệu thành công");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Thêm mới thất bại, Lỗi: " + ex.Message);
                        trans.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void InputField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
           
        }

        
    }
}

