using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Data;
using Wpf.Ui.Common.Interfaces;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace MotoStore.ViewModels
{
    public partial class SupplierListViewModel : ObservableObject, INavigationAware
    {
        public void OnNavigatedTo()
        {
            FillDataGrid();
        }

        public void OnNavigatedFrom()
        {
        }

        public DataView SupplierDataView;

        public void FillDataGrid()
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

            string CmdString = string.Empty;

            using (SqlConnection con = new SqlConnection(ConString))
            {
                CmdString = "SELECT * FROM NhaSanXuat";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("NhaSanXuat");
                sda.Fill(dt);
                SupplierDataView = dt.DefaultView;
                con.Close();
            }
        }
    }
}