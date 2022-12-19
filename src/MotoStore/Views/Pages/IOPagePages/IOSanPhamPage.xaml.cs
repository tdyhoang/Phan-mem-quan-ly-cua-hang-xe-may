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
            var products = GetProducts();
            if (products.Count > 0)
                ListViewProduct.ItemsSource = products;
        }
        private List<Product> GetProducts()
        {
            MainDatabase db = new MainDatabase();
            List<Product> products = new List<Product>();
            List<MatHang> matHang = db.MatHangs.ToList();
            foreach (MatHang matHang1 in matHang)
            {
                products.Add(new Product(matHang1.TenMh, matHang1.GiaBanMh, "/Images/AB160c.png"));
            }
            return products;
        }
        private void btnAddNewPageSP_Click(object sender, RoutedEventArgs e)
        {
            IOAddSPPage ioAddSP = new IOAddSPPage();
            NavigationService.Navigate(ioAddSP);

        }
    }
}
