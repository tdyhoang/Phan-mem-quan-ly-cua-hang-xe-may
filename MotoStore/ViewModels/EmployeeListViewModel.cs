using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using Wpf.Ui.Common.Interfaces;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace MotoStore.ViewModels
{
    public partial class EmployeeListViewModel : ObservableObject, INavigationAware
    {
        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }

        public DataView EmployeeDataView;

        public void FillDataGrid()
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            
            string CmdString = string.Empty;

            using (SqlConnection con = new SqlConnection(ConString))
            {
                CmdString = "SELECT * FROM NhanVien";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("NhanVien");
                sda.Fill(dt);
                EmployeeDataView = dt.DefaultView;
                con.Close();
            }
        }
    }
}