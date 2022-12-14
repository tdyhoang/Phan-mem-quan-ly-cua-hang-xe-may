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
using MotoStore.Database;
using MotoStore.Views.Windows;

namespace MotoStore.Views.Pages.IOPagePages
{
    /// <summary>
    /// Interaction logic for IOSanPhamPage.xaml
    /// </summary>
    public partial class IOSanPhamPage : Page
    {
       
        public IOSanPhamPage()
        {
            InitializeComponent();
            this.DataContext = this;
            var products = GetProducts();
            if (products.Count > 0)
                ListViewProduct.ItemsSource = products;
          
        }
        private List<Product> GetProducts()
        {
            MainDatabase db = new();
            List<Product> products = new();
            List<MatHang> matHang = db.MatHangs.ToList();

            string AnhXE;
            foreach (MatHang matHang1 in matHang)
            {
                AnhXE = "/Views/Pages/IO_Images/" + matHang1.MaMh + ".png"; //Ảnh của từng xe(gán theo mã xe có sẵn )
                products.Add(new(matHang1.TenMh, matHang1.GiaBanMh, AnhXE));
            }
            return products;
        }

        private void btnAddNewPageSP_Click(object sender, RoutedEventArgs e)
        {
            IOAddSPPage ioAddSP = new();
            NavigationService.Navigate(ioAddSP);

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
           WindowInformation windowInformation = new();
            windowInformation.ShowDialog();

        }
    }
}
