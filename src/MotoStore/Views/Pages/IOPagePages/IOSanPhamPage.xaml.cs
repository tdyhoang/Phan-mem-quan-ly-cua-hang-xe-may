using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        MainDatabase mdb = new();
        public IOSanPhamPage()
        {
            InitializeComponent();
            DataContext = this;
            var products = GetProducts();
            if (products.Count > 0)
                ListViewProduct.ItemsSource = products;
            List<Product> items = new(); //Tạo Danh Sách Product phục vụ cho tìm kiếm(Filter)
            foreach (var xe in mdb.MatHangs)
            {
                items.Add(new Product(xe.TenMh, xe.GiaBanMh, "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + xe.MaMh + ".png")
                {
                    ProductId = xe.MaMh,
                    Mau = xe.Mau
                });
            }
            //Chạy vòng lặp trên để thêm từng Sản Phẩm vào Danh Sách
            ListViewProduct.ItemsSource = items;
            //Gán ItemSource của ListViewProduct chính là Danh Sách items vừa thêm ở trên
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            //biến view kiểu CollectionView dùng để gom nhóm, tìm kiếm, filter, điều hướng dữ liệu, gán nó bằng ItemsSource ở trên
            view.Filter = Filter;
            GC.Collect();   
        }
        private List<Product> GetProducts()
        {
            MainDatabase db = new();
            List<Product> products = new(); //..
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

        private bool Filter(object item)
        {
            if(string.IsNullOrWhiteSpace(txtTimKiem.Text))
                return true; //Text rỗng hoặc chứa khoảng trắng thì hiện toàn bộ item(Sản Phẩm)
            else
            {
                return (((item as Product).ProductId.IndexOf(txtTimKiem.Text, System.StringComparison.OrdinalIgnoreCase) >= 0) 
                    || ((item as Product).NameProduct.IndexOf(txtTimKiem.Text, System.StringComparison.OrdinalIgnoreCase) >= 0) 
                    || (item as Product).Mau.IndexOf(txtTimKiem.Text, System.StringComparison.OrdinalIgnoreCase) >= 0);
                //Nếu text KHÔNG RỖNG thì trả về 1 trong 3 thuộc tính đc nhập trong textbox.
            }
        }

        private void txtTimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource).Refresh();
        }
        static int solanbam = 0;
        private void btnKhac_Click(object sender, RoutedEventArgs e)
        {
            if (solanbam == 0)
            {
                menuLoc.Visibility = Visibility.Visible;
                txtTimKiem.Clear();
                txtTimKiem.Visibility = Visibility.Collapsed;
                solanbam = 1;
            }
            else
            {
                menuLoc.Visibility = Visibility.Collapsed;
                txtTimKiem.Visibility = Visibility.Visible;
                solanbam = 0;
            }
        }

        private void subItemDuoi100CC_Click(object sender, RoutedEventArgs e)
        {
            //ListViewProduct.ClearValue(ItemsControl.ItemsSourceProperty);
            List<Product> ListItems = new(); 
            foreach(var xe in mdb.MatHangs.ToList())
                if (xe.SoPhanKhoi.Value < 110)
                    ListItems.Add(new Product(xe.TenMh, xe.GiaBanMh, "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + xe.MaMh + ".png")
                    {
                        ProductId = xe.MaMh,
                        Mau = xe.Mau,
                        ValueMoney = xe.GiaBanMh
                    });
            ListViewProduct.ItemsSource = ListItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            view.Filter = Filter;
            //3 dòng trên dùng để filter sản phẩm
        }

        private void subItemTren100CC_Click(object sender, RoutedEventArgs e)
        {
            //ListViewProduct.ClearValue(ItemsControl.ItemsSourceProperty);
            List<Product> ListItems = new();
            foreach (var xe in mdb.MatHangs.ToList())
                if (xe.SoPhanKhoi.Value >= 110)
                    ListItems.Add(new Product(xe.TenMh, xe.GiaBanMh, "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + xe.MaMh + ".png")
                    {
                        ProductId = xe.MaMh,
                        Mau = xe.Mau,
                        ValueMoney = xe.GiaBanMh
                    });
            ListViewProduct.ItemsSource = ListItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            view.Filter = Filter;
        }
        static string luachon;
        private void subItemTuChon_Click(object sender, RoutedEventArgs e)
        {
            lblTu.Visibility = Visibility.Visible;
            txtTu.Visibility = Visibility.Visible;
            txtDen.Visibility = Visibility.Visible;
            lblDen.Visibility = Visibility.Visible;
            btnTim.Visibility = Visibility.Visible;
            if (subItemTuChon.IsChecked)
            {
                luachon = "PK";
                subItemTuChon.IsChecked = false;
            }
            else if (subItemTuChonGia.IsChecked)
            {
                luachon = "Gia";
                subItemTuChonGia.IsChecked = false;
            }
        }

        private void subItemQuayLai_Click(object sender, RoutedEventArgs e)
        {
            lblTu.Visibility = Visibility.Collapsed;
            txtTu.Visibility = Visibility.Collapsed;
            txtDen.Visibility = Visibility.Collapsed;
            lblDen.Visibility = Visibility.Collapsed;
            btnTim.Visibility = Visibility.Collapsed;
            List<Product> ListItems = new();
            foreach (var xe in mdb.MatHangs.ToList())
            {
                ListItems.Add(new Product(xe.TenMh, xe.GiaBanMh, "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + xe.MaMh + ".png")
                {
                    ProductId = xe.MaMh,
                    Mau=xe.Mau,
                    ValueMoney=xe.GiaBanMh
                });
            }
            ListViewProduct.ItemsSource = ListItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            view.Filter = Filter;
        }

        private void txtTu_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTu.Text))
                MessageBox.Show("Vui lòng điền đầy đủ");
        }

        private void txtDen_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTu.Text))
                MessageBox.Show("Vui lòng điền đầy đủ");
        }

        private void btnTim_Click(object sender, RoutedEventArgs e)
        {
            List<Product> ListItems = new();
            if (string.IsNullOrWhiteSpace(txtTu.Text) || string.IsNullOrWhiteSpace(txtDen.Text))
                MessageBox.Show("Vui lòng điền đầy đủ khoảng trống", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                switch(luachon)
                {
                    default: MessageBox.Show("0 co gi");
                        break;
                    case "PK":
                        foreach (var xe in mdb.MatHangs.ToList())
                        {
                            if (xe.SoPhanKhoi.Value >= int.Parse(txtTu.Text) && xe.SoPhanKhoi.Value <= int.Parse(txtDen.Text))
                                ListItems.Add(new Product(xe.TenMh, xe.GiaBanMh, "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + xe.MaMh + ".png")
                                {
                                    ProductId = xe.MaMh,
                                    Mau = xe.Mau,
                                    ValueMoney = xe.GiaBanMh
                                });
                        }
                        MessageBox.Show("PK");
                        break;
                    case "Gia":
                        foreach (var xe in mdb.MatHangs.ToList())
                        {
                            if (xe.GiaBanMh.Value >= decimal.Parse(txtTu.Text) && xe.GiaBanMh.Value <= decimal.Parse(txtDen.Text))
                                ListItems.Add(new Product(xe.TenMh, xe.GiaBanMh, "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + xe.MaMh + ".png")
                                {
                                    ProductId = xe.MaMh,
                                    Mau = xe.Mau,
                                    ValueMoney = xe.GiaBanMh
                                });
                        }
                        MessageBox.Show("Gia");
                        break;
                }
                ListViewProduct.ItemsSource = ListItems;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
                view.Filter = Filter;
            }
        }

        private void subItemDuoi30Tr_Click(object sender, RoutedEventArgs e)
        {
            List<Product> ListItems = new();
            foreach (var xe in mdb.MatHangs.ToList())
                if (xe.GiaBanMh.Value < 30000000)
                    ListItems.Add(new Product(xe.TenMh, xe.GiaBanMh, "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + xe.MaMh + ".png")
                    {
                        ProductId = xe.MaMh,
                        Mau = xe.Mau,
                        ValueMoney = xe.GiaBanMh
                    });
            ListViewProduct.ItemsSource = ListItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            view.Filter = Filter;
        }

        private void subItemTren30Tr_Click(object sender, RoutedEventArgs e)
        {
            List<Product> ListItems = new();
            foreach (var xe in mdb.MatHangs.ToList())
                if (xe.GiaBanMh.Value >= 30000000)
                    ListItems.Add(new Product(xe.TenMh, xe.GiaBanMh, "C:\\Users\\ADMIN\\Documents\\GitHub\\Phan-mem-quan-ly-cua-hang-xe-may\\src\\MotoStore\\Views\\Pages\\IO_Images\\" + xe.MaMh + ".png")
                    {
                        ProductId = xe.MaMh,
                        Mau = xe.Mau,
                        ValueMoney = xe.GiaBanMh
                    });
            ListViewProduct.ItemsSource = ListItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewProduct.ItemsSource);
            view.Filter = Filter;
        }
    }
}
